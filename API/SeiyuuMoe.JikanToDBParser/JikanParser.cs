﻿using JikanDotNet;
using JikanDotNet.Exceptions;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Infrastructure.Configuration;
using SeiyuuMoe.Infrastructure.Logger;
using SeiyuuMoe.Infrastructure.Repositories;
using SeiyuuMoe.Infrastructure.Utilities;
using SeiyuuMoe.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Anime = SeiyuuMoe.Domain.Entities.Anime;
using Character = SeiyuuMoe.Domain.Entities.Character;
using Season = JikanDotNet.Season;

namespace SeiyuuMoe.JikanToDBParser
{
	public class JikanParser : IJikanParser
	{
		private readonly IAnimeRepository animeRepository;
		private readonly IAnimeStatusRepository animeStatusRepository;
		private readonly IAnimeTypeRepository animeTypeRepository;
		private readonly IBlacklistedIdRepository blacklistedIdRepository;
		private readonly ICharacterRepository characterRepository;

		private readonly IJikan jikan;
		private readonly ILoggingService logger;
		private readonly IAnimeRoleRepository animeRoleRepository;
		private readonly ISeiyuuRoleRepository seiyuuRoleRepository;
		private readonly ISeasonRepository seasonRepository;
		private readonly ISeiyuuRepository seiyuuRepository;

		public JikanParser(
			SeiyuuMoeConfiguration seiyuuMoeConfiguration,
			ILoggingService loggingService,
			IAnimeRepository animeRepository,
			IAnimeStatusRepository animeStatusRepository,
			IAnimeTypeRepository animeTypeRepository,
			IBlacklistedIdRepository blacklistedIdRepository,
			ICharacterRepository characterRepository,
			IAnimeRoleRepository animeRoleRepository,
			ISeiyuuRoleRepository seiyuuRoleRepository,
			ISeasonRepository seasonRepository,
			ISeiyuuRepository seiyuuRepository
		)
		{
			jikan = new Jikan(seiyuuMoeConfiguration.JikanUrl);
			logger = loggingService;
			this.animeRepository = animeRepository;
			this.seasonRepository = seasonRepository;
			this.seiyuuRepository = seiyuuRepository;
			this.animeTypeRepository = animeTypeRepository;
			this.animeStatusRepository = animeStatusRepository;
			this.blacklistedIdRepository = blacklistedIdRepository;
			this.characterRepository = characterRepository;
			this.animeRoleRepository = animeRoleRepository;
			this.seiyuuRoleRepository = seiyuuRoleRepository;
		}

		private void BlacklistId(long id, string type, string reason = null)
		{
			var blacklistedId = new BlacklistedId
			{
				MalId = id,
				EntityType = type,
				Reason = reason
			};

			blacklistedIdRepository.AddAsync(blacklistedId);
		}

		private bool IsJapanese(string japaneseName)
		{
			return !string.IsNullOrWhiteSpace(japaneseName) &&
				   japaneseName.All(x =>
					   x >= 0x4E00 && x <= 0x9FBF || // kanji
					   x >= 0x3040 && x <= 0x309F || // hiragana
					   x >= 0x30A0 && x <= 0x30FF || // katakana
					   x == ' ');
		}

		private string EmptyStringIfPlaceholder(string imageUrl)
		{
			if (string.IsNullOrEmpty(imageUrl) ||
				imageUrl.Equals("https://cdn.myanimelist.net/images/questionmark_23.gif") ||
				imageUrl.Equals("https://cdn.myanimelist.net/img/sp/icon/apple-touch-icon-256.png"))
				return string.Empty;

			return imageUrl;
		}

		#region Interface Implementation

		public async Task InsertNewSeiyuu()
		{
			logger.Log("Started InsertNewSeiyuu job.");

			var lastSeiyuu =
				await seiyuuRepository.GetOrderedPageAsync(PredicateBuilder.True<Seiyuu>(), 0, 1);

			var lastId = lastSeiyuu.Results.First().MalId;

			for (var malId = lastId + 1; malId < lastId + 100; malId++)
			{
				var seiyuu = await SendSinglePersonRequest(malId, 0);

				if (seiyuu != null)
				{
					logger.Log($"Parsed id:{seiyuu.MalId}: {seiyuu.Name}");
				}
				else
				{
					logger.Log($"Omitted {malId} - not found");
					continue;
				}

				var japaneseName = string.Empty;

				if (!string.IsNullOrWhiteSpace(seiyuu.FamilyName))
					japaneseName += seiyuu.FamilyName;

				if (!string.IsNullOrWhiteSpace(seiyuu.GivenName))
					japaneseName += string.IsNullOrEmpty(japaneseName) ? seiyuu.GivenName : " " + seiyuu.GivenName;

				if (!IsJapanese(japaneseName))
				{
					logger.Log($"Omitted {seiyuu.Name} - not Japanese");
					BlacklistId(malId, "Seiyuu", "Not Japanese");
					continue;
				}

				if (seiyuu.VoiceActingRoles.Count <= 0)
				{
					logger.Log($"Omitted {seiyuu.Name} - not a seiyuu");
					BlacklistId(malId, "Seiyuu", "Not a seiyuu");
					continue;
				}

				await seiyuuRepository.AddAsync(
					new Seiyuu
					{
						Name = seiyuu.Name,
						MalId = seiyuu.MalId,
						ImageUrl = EmptyStringIfPlaceholder(seiyuu.ImageURL),
						About = seiyuu.More,
						Birthday = seiyuu.Birthday.HasValue
							? seiyuu.Birthday.Value.ToString("dd-MM-yyyy")
							: string.Empty,
						Popularity = seiyuu.MemberFavorites,
						JapaneseName = japaneseName
					}
				);

				logger.Log($"Inserted {seiyuu.Name} with MalId: {seiyuu.MalId}");

				foreach (var role in seiyuu.VoiceActingRoles) await InsertRole(seiyuu.MalId, role, new List<Role>());
			}

			logger.Log("Finished InsertNewSeiyuu job.");
		}

		public async Task ParseRoles()
		{
			logger.Log("Started ParseRoles job.");

			var seiyuuIdCollection = await seiyuuRepository.GetAllIdsAsync();

			foreach (var seiyuuMalId in seiyuuIdCollection)
				try
				{
					var seiyuuRoles = await seiyuuRoleRepository.GetAllSeiyuuRolesAsync(seiyuuMalId, false);

					var seiyuuFullData = await SendSinglePersonRequest(seiyuuMalId, 0);

					logger.Log($"Parsing seiyuu with id {seiyuuMalId}");

					foreach (var role in seiyuuFullData.VoiceActingRoles)
						await InsertRole(seiyuuMalId, role, seiyuuRoles);
				}
				catch (Exception ex)
				{
					logger.Log($"Error during parsing seiyuu with id {seiyuuMalId}: {ex.Message}");
				}

			logger.Log("Finished ParseRoles job.");
		}

		public async Task UpdateAllAnime()
		{
			logger.Log("Started UpdateAllAnime job.");

			var page = 0;
			var pageSize = 100;
			var totalAnimeCount = await animeRepository.GetAnimeCountAsync();

			while (page * pageSize < totalAnimeCount)
			{
				var animeCollection =
					await animeRepository.GetOrderedPageAsync(PredicateBuilder.True<Anime>(), page, pageSize);

				foreach (var anime in animeCollection.Results)
				{
					var animeFullData = await SendSingleAnimeRequest(anime.MalId, 0);

					if (animeFullData != null)
					{
						logger.Log($"Parsed anime with id {anime.MalId}: {anime.Title}");

						await UpdateAnime(anime, animeFullData);
					}
					else
					{
						logger.Log($"Error on {anime.MalId} - not found");
					}
				}

				page++;
			}

			logger.Log("Finished UpdateAllAnime job.");
		}

		public async Task UpdateAllCharacters()
		{
			try
			{
				logger.Log("Started UpdateAllCharacters job.");

				var page = 0;
				var pageSize = 100;
				var totalCharacterCount = await characterRepository.GetCountAsync();

				while (page * pageSize < totalCharacterCount)
				{
					var characterCollection = await characterRepository.GetPageAsync(page, pageSize);

					foreach (var character in characterCollection.Results)
					{
						var characterFullData = await SendSingleCharacterRequest(character.MalId, 0);

						if (characterFullData != null)
						{
							logger.Log($"Parsed character with id {character.MalId}: {character.Name}");

							await UpdateCharacter(character, characterFullData);
						}
						else
						{
							logger.Log($"Error on {character.MalId} - not found");
						}
					}

					page++;
				}

				logger.Log("Finished UpdateAllCharacters job.");
			}
			catch (Exception ex)
			{
				logger.Log($"Exception: {ex.Message}");
			}
		}

		public async Task UpdateAllSeiyuu()
		{
			logger.Log("Started UpdateAllSeiyuu job.");

			var page = 0;
			var pageSize = 100;
			var totalSeiyuuCount = await seiyuuRepository.GetSeiyuuCountAsync();

			while (page * pageSize < totalSeiyuuCount)
			{
				var seiyuuCollection =
					await seiyuuRepository.GetOrderedPageAsync(PredicateBuilder.True<Seiyuu>(), page, pageSize);

				foreach (var seiyuu in seiyuuCollection.Results)
				{
					var seiyuuFullData = await SendSinglePersonRequest(seiyuu.MalId, 0);

					if (seiyuuFullData != null)
					{
						logger.Log($"Parsed id:{seiyuu.MalId}, {seiyuu.Name}");

						await UpdateSeiyuu(seiyuu, seiyuuFullData);
					}
					else
					{
						logger.Error($"Error on {seiyuu.MalId} - not found");
					}
				}

				page++;
			}

			logger.Log("Finished UpdateAllSeiyuu job.");
		}

		public async Task UpdateSeasons()
		{
			logger.Log("Started UpdateSeasons job.");

			var yearInNextSixMonths = DateTime.Now.AddMonths(6).Year;

			var seasonArchives = await jikan.GetSeasonArchive();

			foreach (var season in seasonArchives.Archives.First(x => x.Year.Equals(yearInNextSixMonths)).Season)
			{
				var insertedSeason = await seasonRepository.GetAsync(x => x.Name.Equals(season.ToString()) &&
																		  x.Year.Equals(yearInNextSixMonths));

				if (insertedSeason == null)
				{
					await seasonRepository.AddAsync(new Domain.Entities.Season
					{
						Name = season.ToString(),
						Year = yearInNextSixMonths
					});

				}
			}

			logger.Log("Finished UpdateSeasons job.");
		}

		public async Task InsertOldSeiyuu()
		{
			logger.Log("Started InsertOldSeiyuu job.");

			var seiyuuCollection = await seiyuuRepository.GetAllIdsAsync();

			for (long malId = 1; malId < seiyuuCollection.Last(); malId++)
			{
				if (seiyuuCollection.Contains(malId))
				{
					logger.Log($"Omitted {malId} - already in database.");
					continue;
				}

				var seiyuu = await SendSinglePersonRequest(malId, 0);

				if (seiyuu != null)
				{
					logger.Log($"Parsed id:{seiyuu.MalId}: {seiyuu.Name}");
				}
				else
				{
					logger.Log($"Omitted {malId} - not found");
					continue;
				}

				var japaneseName = string.Empty;

				if (!string.IsNullOrWhiteSpace(seiyuu.FamilyName))
					japaneseName += seiyuu.FamilyName;

				if (!string.IsNullOrWhiteSpace(seiyuu.GivenName))
					japaneseName += string.IsNullOrEmpty(japaneseName) ? seiyuu.GivenName : " " + seiyuu.GivenName;

				if (!IsJapanese(japaneseName))
				{
					logger.Log($"Omitted {seiyuu.Name} - not Japanese");
					BlacklistId(malId, "Seiyuu", "Not Japanese");
					continue;
				}

				if (seiyuu.VoiceActingRoles.Count <= 0)
				{
					logger.Log($"Omitted {seiyuu.Name} - not a seiyuu");
					BlacklistId(malId, "Seiyuu", "Not a seiyuu");
					continue;
				}

				await seiyuuRepository.AddAsync(
					new Seiyuu
					{
						Name = seiyuu.Name,
						MalId = seiyuu.MalId,
						ImageUrl = EmptyStringIfPlaceholder(seiyuu.ImageURL),
						About = seiyuu.More,
						Birthday = seiyuu.Birthday.HasValue
							? seiyuu.Birthday.Value.ToString("dd-MM-yyyy")
							: string.Empty,
						Popularity = seiyuu.MemberFavorites,
						JapaneseName = japaneseName
					}
				);

				logger.Log($"Inserted {seiyuu.Name} with MalId: {seiyuu.MalId}");

				foreach (var role in seiyuu.VoiceActingRoles) await InsertRole(seiyuu.MalId, role, new List<Role>());
			}

			logger.Log("Finished InsertOldSeiyuu job.");
		}

		#endregion Interface Implementation

		#region Updating Entities

		private async Task UpdateAnime(Anime anime, JikanDotNet.Anime animeParsedData)
		{
			anime.Title = animeParsedData.Title;
			anime.About = animeParsedData.Synopsis;
			anime.EnglishTitle = animeParsedData.TitleEnglish;
			anime.JapaneseTitle = animeParsedData.TitleJapanese;
			anime.Popularity = animeParsedData.Members;

			anime.ImageUrl = EmptyStringIfPlaceholder(animeParsedData.ImageURL);

			if (animeParsedData.Aired.From.HasValue)
				anime.AiringDate = animeParsedData.Aired.From.Value.ToString("dd-MM-yyyy");

			if (animeParsedData.TitleSynonyms.Count > 0)
				anime.TitleSynonyms = string.Join(';', animeParsedData.TitleSynonyms);

			anime.TypeId = await MatchAnimeType(animeParsedData.Type);
			anime.StatusId = await MatchAnimeStatus(animeParsedData.Status);
			anime.SeasonId = string.IsNullOrEmpty(animeParsedData.Premiered)
				? await MatchSeason(animeParsedData.Aired.From)
				: await MatchSeason(animeParsedData.Premiered);

			await animeRepository.UpdateAsync(anime);
		}

		private async Task UpdateCharacter(Character character, JikanDotNet.Character characterParsedData)
		{
			character.Name = characterParsedData.Name;
			character.About = characterParsedData.About;
			character.ImageUrl = characterParsedData.ImageURL;
			character.NameKanji = characterParsedData.NameKanji;
			character.Popularity = characterParsedData.MemberFavorites;

			character.ImageUrl = EmptyStringIfPlaceholder(characterParsedData.ImageURL);

			if (characterParsedData.Nicknames.Any())
				character.Nicknames = string.Join(';', characterParsedData.Nicknames.ToArray());

			await characterRepository.UpdateAsync(character);
		}

		private async Task UpdateSeiyuu(Seiyuu seiyuu, Person seiyuuParsedData)
		{
			var japaneseName = string.Empty;

			seiyuu.Name = seiyuuParsedData.Name;
			seiyuu.Popularity = seiyuuParsedData.MemberFavorites;
			seiyuu.About = seiyuuParsedData.More;

			if (seiyuuParsedData.Birthday.HasValue)
				seiyuu.Birthday = seiyuuParsedData.Birthday.Value.ToString("dd-MM-yyyy");

			seiyuu.ImageUrl = EmptyStringIfPlaceholder(seiyuuParsedData.ImageURL);

			if (!string.IsNullOrWhiteSpace(seiyuuParsedData.FamilyName))
				japaneseName += seiyuuParsedData.FamilyName;

			if (!string.IsNullOrWhiteSpace(seiyuuParsedData.GivenName))
				japaneseName += string.IsNullOrEmpty(japaneseName)
					? seiyuuParsedData.GivenName
					: " " + seiyuuParsedData.GivenName;

			seiyuu.JapaneseName = japaneseName;

			await seiyuuRepository.UpdateAsync(seiyuu);
		}

		#endregion Updating Entities

		#region Requests

		private async Task<JikanDotNet.Anime> SendSingleAnimeRequest(long malId, short retryCount)
		{
			JikanDotNet.Anime anime = null;
			await Task.Delay(3000 + retryCount * 10000);

			try
			{
				anime = await jikan.GetAnime(malId);
			}
			catch (Exception ex)
			{
				if (retryCount < 10)
					if (ex.InnerException is JikanRequestException &&
						(ex.InnerException as JikanRequestException).ResponseCode == HttpStatusCode.TooManyRequests)
					{
						retryCount++;
						return await SendSingleAnimeRequest(malId, retryCount);
					}
			}

			return anime;
		}

		private async Task<AnimeCharactersStaff> SendSingleAnimeCharactersStaffRequest(long malId, short retryCount)
		{
			AnimeCharactersStaff animeCharactersStaff = null;
			await Task.Delay(3000 + retryCount * 10000);

			try
			{
				animeCharactersStaff = await jikan.GetAnimeCharactersStaff(malId);
			}
			catch (Exception ex)
			{
				if (retryCount < 10)
					if (ex.InnerException is JikanRequestException &&
						(ex.InnerException as JikanRequestException).ResponseCode == HttpStatusCode.TooManyRequests)
					{
						retryCount++;
						return await SendSingleAnimeCharactersStaffRequest(malId, retryCount);
					}
			}

			return animeCharactersStaff;
		}

		private async Task<Person> SendSinglePersonRequest(long malId, short retryCount)
		{
			Person seiyuu = null;
			await Task.Delay(3000 + retryCount * 10000);

			try
			{
				seiyuu = await jikan.GetPerson(malId);
			}
			catch (Exception ex)
			{
				if (retryCount < 10)
				{
					if (ex.InnerException is JikanRequestException &&
						(ex.InnerException as JikanRequestException).ResponseCode == HttpStatusCode.TooManyRequests)
					{
						retryCount++;
						return await SendSinglePersonRequest(malId, retryCount);
					}

					if (ex.InnerException is JikanRequestException)
					{
						var responseCode = (ex.InnerException as JikanRequestException).ResponseCode;

						switch (responseCode)
						{
							case HttpStatusCode.NotFound:
								BlacklistId(malId, "Seiyuu", "404 Not Found");
								break;

							case HttpStatusCode.InternalServerError:
								//	BlacklistId(malId, "Seiyuu", "Not exist");
								break;

							case HttpStatusCode.TooManyRequests:
								BlacklistId(malId, "Seiyuu", "429 Too much request");
								break;

							default:
								BlacklistId(malId, "Seiyuu", "Other");
								break;
						}
					}
				}
			}

			return seiyuu;
		}

		private async Task<JikanDotNet.Character> SendSingleCharacterRequest(long malId, short retryCount)
		{
			JikanDotNet.Character character = null;
			await Task.Delay(3000 + retryCount * 10000);

			try
			{
				character = await jikan.GetCharacter(malId);
			}
			catch (Exception ex)
			{
				if (retryCount < 10)
				{
					if (ex.InnerException is JikanRequestException &&
						(ex.InnerException as JikanRequestException).ResponseCode == HttpStatusCode.TooManyRequests)
					{
						retryCount++;
						return await SendSingleCharacterRequest(malId, retryCount);
					}

					if (ex.InnerException is JikanRequestException)
					{
						var responseCode = (ex.InnerException as JikanRequestException).ResponseCode;

						switch (responseCode)
						{
							case HttpStatusCode.NotFound:
								BlacklistId(malId, "Character", "404 Not Found");
								break;

							case HttpStatusCode.InternalServerError:
								BlacklistId(malId, "Character", "Not exist");
								break;

							case HttpStatusCode.TooManyRequests:
								BlacklistId(malId, "Character", "429 Too much request");
								break;

							default:
								BlacklistId(malId, "Character", "Other");
								break;
						}
					}
				}
			}

			return character;
		}

		private async Task<Season> SendSingleSeasonRequest(int year, Seasons seasonName, short retryCount)
		{
			Season season = null;
			await Task.Delay(3000 + retryCount * 10000);

			try
			{
				season = await jikan.GetSeason(year, seasonName);
			}
			catch (Exception ex)
			{
				if (retryCount < 5)
					if (ex.InnerException is JikanRequestException)
						if ((ex.InnerException as JikanRequestException).ResponseCode == HttpStatusCode.TooManyRequests)
						{
							retryCount++;
							await SendSingleSeasonRequest(year, seasonName, retryCount);
						}
			}

			return season;
		}

		#endregion Requests

		#region ForeignKeyMatching

		private async Task<long?> MatchAnimeType(string typeName)
		{
			var foundType = await animeTypeRepository.GetByNameAsync(typeName);

			return foundType?.Id;
		}

		private async Task<long?> MatchAnimeStatus(string statusName)
		{
			var foundStatus = await animeStatusRepository.GetByNameAsync(statusName);

			return foundStatus?.Id;
		}

		private async Task<long?> MatchSeason(string seasonName)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(seasonName))
				{
					var seasonParts = seasonName.Split(' ');

					if (seasonParts.Length > 1)
					{
						var year = int.Parse(seasonParts[1]);
						var season = seasonParts[0];

						var foundSeason = await seasonRepository.GetAsync(x =>
							x.Name.ToLower().Equals(season.ToLower()) && x.Year.Equals(year));

						return foundSeason?.Id;
					}

					return null;
				}

				return null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		private async Task<long?> MatchSeason(DateTime? airingFrom)
		{
			try
			{
				if (!airingFrom.HasValue)
					return null;

				var airingDate = airingFrom.Value.Date;

				Seasons seasonEnumValue;
				var yearOfSeason = airingFrom.Value.Year;

				if (airingDate.DayOfYear > 349 || airingDate.DayOfYear < 75)
					seasonEnumValue = Seasons.Winter;
				else if (airingDate.DayOfYear >= 75 && airingDate.DayOfYear < 166)
					seasonEnumValue = Seasons.Spring;
				else if (airingDate.DayOfYear >= 167 && airingDate.DayOfYear < 258)
					seasonEnumValue = Seasons.Summer;
				else
					seasonEnumValue = Seasons.Fall;

				if (airingDate.DayOfYear > 349)
					yearOfSeason++;

				return await MatchSeason(yearOfSeason, seasonEnumValue);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private async Task<long?> MatchSeason(int year, Seasons season)
		{
			try
			{
				var foundSeason =
					await seasonRepository.GetAsync(x => x.Name.Equals(season.ToString()) && x.Year.Equals(year));

				return foundSeason?.Id;
			}
			catch (Exception)
			{
				return null;
			}
		}

		#endregion ForeignKeyMatching

		#region InsertingRoleRelatedEntities

		private async Task InsertRole(long seiyuuMalId, VoiceActingRole voiceActingRole,
			IReadOnlyList<Role> seiyuuRoles)
		{
			try
			{
				if (!seiyuuRoles.Any(x =>
					x.AnimeId.Equals(voiceActingRole.Anime.MalId) &&
					x.CharacterId.Equals(voiceActingRole.Character.MalId)))
				{
					var isCharacterInDatabase = true;
					var isAnimeInDatabase = true;

					if (await animeRepository.GetAsync(voiceActingRole.Anime.MalId) == null)
						isAnimeInDatabase = await InsertAnime(voiceActingRole);
					if (await characterRepository.GetAsync(voiceActingRole.Character.MalId) == null)
						isCharacterInDatabase = await InsertCharacter(voiceActingRole);

					if (isAnimeInDatabase && isCharacterInDatabase)
					{
						await animeRoleRepository.AddAsync(new Role
						{
							LanguageId = 1, // Always japanese for now
							RoleTypeId = voiceActingRole.Role.Equals("Main") ? 1 : 2,
							AnimeId = voiceActingRole.Anime.MalId,
							CharacterId = voiceActingRole.Character.MalId,
							SeiyuuId = seiyuuMalId
						});

						logger.Log($"Inserted {voiceActingRole.Character.Name} in {voiceActingRole.Anime.Name}");
					}
				}
			}
			catch (Exception ex)
			{
				logger.Log(
					$"Error during inserting with anime {voiceActingRole.Anime.Name}, character {voiceActingRole.Character.Name}: {ex.Message}");
			}
		}

		private async Task<bool> InsertAnime(VoiceActingRole voiceActingRole)
		{
			var existingAnime = await animeRepository.GetAsync(voiceActingRole.Anime.MalId);

			if (existingAnime == null)
				try
				{
					var animeFullData = await SendSingleAnimeRequest(voiceActingRole.Anime.MalId, 0);
					var titleSynonym = string.Empty;

					if (animeFullData != null)
					{
						logger.Log($"Parsed anime with id:{animeFullData.MalId}");

						if (animeFullData.TitleSynonyms.Any())
							titleSynonym = string.Join(';', animeFullData.TitleSynonyms.ToArray());

						await animeRepository.AddAsync(
							new Anime
							{
								MalId = animeFullData.MalId,
								ImageUrl = EmptyStringIfPlaceholder(animeFullData.ImageURL),
								Title = animeFullData.Title,
								Popularity = animeFullData.Members,
								About = animeFullData.Synopsis,
								JapaneseTitle = animeFullData.TitleJapanese,
								EnglishTitle = animeFullData.TitleEnglish,
								TitleSynonyms = titleSynonym,
								AiringDate = animeFullData.Aired.From?.ToString("dd-MM-yyyy"),
								StatusId = await MatchAnimeStatus(animeFullData.Status),
								TypeId = await MatchAnimeType(animeFullData.Type),
								SeasonId = await MatchSeason(animeFullData.Premiered)
							}
						);

						return true;
					}

					return false;
				}
				catch (Exception ex)
				{
					logger.Log(
						$"Error during inserting anime {voiceActingRole.Anime.Name} with id {voiceActingRole.Character.MalId}: {ex.Message}");
					return false;
				}

			return true; //already inserted
		}

		private async Task<bool> InsertCharacter(VoiceActingRole voiceActingRole)
		{
			var existingCharacter = await characterRepository.GetAsync(voiceActingRole.Character.MalId);

			if (existingCharacter == null)
				try
				{
					var characterFullData = await SendSingleCharacterRequest(voiceActingRole.Character.MalId, 0);
					var nicknames = string.Empty;

					if (characterFullData != null)
					{
						logger.Log($"Parsed id:{characterFullData.MalId}");

						if (characterFullData.Nicknames.Any())
							nicknames = string.Join(';', characterFullData.Nicknames.ToArray());

						await characterRepository.AddAsync(
							new Character
							{
								MalId = characterFullData.MalId,
								ImageUrl = EmptyStringIfPlaceholder(characterFullData.ImageURL),
								Name = characterFullData.Name,
								Popularity = characterFullData.MemberFavorites,
								About = characterFullData.About,
								NameKanji = characterFullData.NameKanji,
								Nicknames = nicknames
							}
						);

						return true;
					}

					return false;
				}
				catch (Exception ex)
				{
					logger.Log(
						$"Error during inserting character {voiceActingRole.Anime.Name} with id {voiceActingRole.Anime.MalId}: {ex.Message}");
					return false;
				}

			return true; //already inserted
		}

		#endregion InsertingRoleRelatedEntities
	}
}
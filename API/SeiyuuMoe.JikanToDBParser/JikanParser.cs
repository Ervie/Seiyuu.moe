using JikanDotNet;
using JikanDotNet.Exceptions;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Infrastructure.Configuration;
using SeiyuuMoe.Infrastructure.Logger;
using SeiyuuMoe.Infrastructure.Utilities;
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
		private readonly IAnimeRepository _animeRepository;
		private readonly IAnimeStatusRepository _animeStatusRepository;
		private readonly IAnimeTypeRepository _animeTypeRepository;
		private readonly IBlacklistedIdRepository _blacklistedIdRepository;
		private readonly ICharacterRepository _characterRepository;

		private readonly IJikan _jikan;
		private readonly ILoggingService _logger;
		private readonly IAnimeRoleRepository _animeRoleRepository;
		private readonly ISeiyuuRoleRepository _seiyuuRoleRepository;
		private readonly ISeasonRepository _seasonRepository;
		private readonly ISeiyuuRepository _seiyuuRepository;

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
			_jikan = new Jikan(seiyuuMoeConfiguration.JikanUrl);
			_logger = loggingService;
			_animeRepository = animeRepository;
			_seasonRepository = seasonRepository;
			_seiyuuRepository = seiyuuRepository;
			_animeTypeRepository = animeTypeRepository;
			_animeStatusRepository = animeStatusRepository;
			_blacklistedIdRepository = blacklistedIdRepository;
			_characterRepository = characterRepository;
			_animeRoleRepository = animeRoleRepository;
			_seiyuuRoleRepository = seiyuuRoleRepository;
		}

		private void BlacklistId(long id, string type, string reason = null)
		{
			var blacklistedId = new BlacklistedId
			{
				MalId = id,
				EntityType = type,
				Reason = reason
			};

			_blacklistedIdRepository.AddAsync(blacklistedId);
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
			_logger.Log("Started InsertNewSeiyuu job.");

			var lastSeiyuu = await _seiyuuRepository.GetLastSeiyuuMalId();

			var lastId = lastSeiyuu ?? 0;

			for (var malId = lastId + 1; malId < lastId + 100; malId++)
			{
				var seiyuu = await SendSinglePersonRequest(malId, 0);

				if (seiyuu != null)
				{
					_logger.Log($"Parsed person id: {seiyuu.MalId}: {seiyuu.Name}");
				}
				else
				{
					_logger.Log($"Omitted person {malId} - not found");
					continue;
				}

				var japaneseName = string.Empty;

				if (!string.IsNullOrWhiteSpace(seiyuu.FamilyName))
				{
					japaneseName += seiyuu.FamilyName;
				}

				if (!string.IsNullOrWhiteSpace(seiyuu.GivenName))
				{
					japaneseName += string.IsNullOrEmpty(japaneseName)
						? seiyuu.GivenName
						: " " + seiyuu.GivenName;
				}

				if (!IsJapanese(japaneseName))
				{
					_logger.Log($"Omitted person {seiyuu.Name} - not Japanese");
					BlacklistId(malId, "Seiyuu", "Not Japanese");
					continue;
				}

				if (seiyuu.VoiceActingRoles.Any())
				{
					_logger.Log($"Omitted person {seiyuu.Name} - not a seiyuu");
					BlacklistId(malId, "Seiyuu", "Not a seiyuu");
					continue;
				}

				await _seiyuuRepository.AddAsync(
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

				_logger.Log($"Inserted {seiyuu.Name} with MalId: {seiyuu.MalId}");

				foreach (var role in seiyuu.VoiceActingRoles)
				{
					await InsertRole(seiyuu.MalId, role, new List<Role>());
				}
			}

			_logger.Log("Finished InsertNewSeiyuu job.");
		}

		public async Task ParseRoles()
		{
			_logger.Log("Started ParseRoles job.");

			var seiyuuIdCollection = await _seiyuuRepository.GetAllIdsAsync();

			foreach (var seiyuuMalId in seiyuuIdCollection)
			{
				try
				{
					var seiyuuRoles = await _seiyuuRoleRepository.GetAllSeiyuuRolesAsync(seiyuuMalId, false);

					var seiyuuFullData = await SendSinglePersonRequest(seiyuuMalId, 0);

					_logger.Log($"Parsing seiyuu with id {seiyuuMalId}");

					foreach (var role in seiyuuFullData.VoiceActingRoles)
					{
						await InsertRole(seiyuuMalId, role, seiyuuRoles);
					}
				}
				catch (Exception ex)
				{
					_logger.Log($"Error during parsing seiyuu with id {seiyuuMalId}: {ex.Message}");
				}
			}

			_logger.Log("Finished ParseRoles job.");
		}

		public async Task UpdateAllAnime()
		{
			_logger.Log("Started UpdateAllAnime job.");

			var page = 0;
			var pageSize = 100;
			var totalAnimeCount = await _animeRepository.GetAnimeCountAsync();

			while (page * pageSize < totalAnimeCount)
			{
				var animeCollection =
					await _animeRepository.GetOrderedPageByAsync(PredicateBuilder.True<Anime>(), page, pageSize);

				foreach (var anime in animeCollection.Results)
				{
					var animeFullData = await SendSingleAnimeRequest(anime.MalId, 0);

					if (animeFullData != null)
					{
						_logger.Log($"Parsed anime with id {anime.MalId}: {anime.Title}");

						await UpdateAnime(anime, animeFullData);
					}
					else
					{
						_logger.Log($"Error on {anime.MalId} - not found");
					}
				}

				page++;
			}

			_logger.Log("Finished UpdateAllAnime job.");
		}

		public async Task UpdateAllCharacters()
		{
			try
			{
				_logger.Log("Started UpdateAllCharacters job.");

				var page = 0;
				var pageSize = 100;
				var totalCharacterCount = await _characterRepository.GetCountAsync();

				while (page * pageSize < totalCharacterCount)
				{
					var characterCollection = await _characterRepository.GetPageAsync(page, pageSize);

					foreach (var character in characterCollection.Results)
					{
						var characterFullData = await SendSingleCharacterRequest(character.MalId, 0);

						if (characterFullData != null)
						{
							_logger.Log($"Parsed character with id {character.MalId}: {character.Name}");

							await UpdateCharacter(character, characterFullData);
						}
						else
						{
							_logger.Log($"Error on {character.MalId} - not found");
						}
					}

					page++;
				}

				_logger.Log("Finished UpdateAllCharacters job.");
			}
			catch (Exception ex)
			{
				_logger.Log($"Exception: {ex.Message}");
			}
		}

		public async Task UpdateAllSeiyuu()
		{
			_logger.Log("Started UpdateAllSeiyuu job.");

			var page = 0;
			var pageSize = 100;
			var totalSeiyuuCount = await _seiyuuRepository.GetSeiyuuCountAsync();

			while (page * pageSize < totalSeiyuuCount)
			{
				var seiyuuCollection =
					await _seiyuuRepository.GetOrderedPageAsync(PredicateBuilder.True<Seiyuu>(), page, pageSize);

				foreach (var seiyuu in seiyuuCollection.Results)
				{
					var seiyuuFullData = await SendSinglePersonRequest(seiyuu.MalId, 0);

					if (seiyuuFullData != null)
					{
						_logger.Log($"Parsed id:{seiyuu.MalId}, {seiyuu.Name}");

						await UpdateSeiyuu(seiyuu, seiyuuFullData);
					}
					else
					{
						_logger.Error($"Error on {seiyuu.MalId} - not found");
					}
				}

				page++;
			}

			_logger.Log("Finished UpdateAllSeiyuu job.");
		}

		public async Task UpdateSeasons()
		{
			_logger.Log("Started UpdateSeasons job.");

			var yearInNextSixMonths = DateTime.Now.AddMonths(6).Year;

			var seasonArchives = await _jikan.GetSeasonArchive();

			foreach (var season in seasonArchives.Archives.First(x => x.Year.Equals(yearInNextSixMonths)).Season)
			{
				var insertedSeason = await _seasonRepository.GetAsync(x => x.Name.Equals(season.ToString()) &&
																		  x.Year.Equals(yearInNextSixMonths));

				if (insertedSeason == null)
				{
					await _seasonRepository.AddAsync(new Domain.Entities.Season
					{
						Name = season.ToString(),
						Year = yearInNextSixMonths
					});
				}
			}

			_logger.Log("Finished UpdateSeasons job.");
		}

		public async Task InsertOldSeiyuu()
		{
			_logger.Log("Started InsertOldSeiyuu job.");

			var seiyuuCollection = await _seiyuuRepository.GetAllIdsAsync();

			for (long malId = 1; malId < seiyuuCollection.Last(); malId++)
			{
				if (seiyuuCollection.Contains(malId))
				{
					_logger.Log($"Omitted {malId} - already in database.");
					continue;
				}

				var seiyuu = await SendSinglePersonRequest(malId, 0);

				if (seiyuu != null)
				{
					_logger.Log($"Parsed id:{seiyuu.MalId}: {seiyuu.Name}");
				}
				else
				{
					_logger.Log($"Omitted {malId} - not found");
					continue;
				}

				var japaneseName = string.Empty;

				if (!string.IsNullOrWhiteSpace(seiyuu.FamilyName))
					japaneseName += seiyuu.FamilyName;

				if (!string.IsNullOrWhiteSpace(seiyuu.GivenName))
					japaneseName += string.IsNullOrEmpty(japaneseName) ? seiyuu.GivenName : " " + seiyuu.GivenName;

				if (!IsJapanese(japaneseName))
				{
					_logger.Log($"Omitted {seiyuu.Name} - not Japanese");
					BlacklistId(malId, "Seiyuu", "Not Japanese");
					continue;
				}

				if (seiyuu.VoiceActingRoles.Count <= 0)
				{
					_logger.Log($"Omitted {seiyuu.Name} - not a seiyuu");
					BlacklistId(malId, "Seiyuu", "Not a seiyuu");
					continue;
				}

				await _seiyuuRepository.AddAsync(
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

				_logger.Log($"Inserted {seiyuu.Name} with MalId: {seiyuu.MalId}");

				foreach (var role in seiyuu.VoiceActingRoles) await InsertRole(seiyuu.MalId, role, new List<Role>());
			}

			_logger.Log("Finished InsertOldSeiyuu job.");
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
			{
				anime.AiringDate = animeParsedData.Aired.From.Value.ToString("dd-MM-yyyy");
			}

			if (animeParsedData.TitleSynonyms.Any())
			{
				anime.TitleSynonyms = string.Join(';', animeParsedData.TitleSynonyms);
			}

			anime.TypeId = await MatchAnimeType(animeParsedData.Type);
			anime.StatusId = await MatchAnimeStatus(animeParsedData.Status);
			anime.SeasonId = string.IsNullOrEmpty(animeParsedData.Premiered)
				? await MatchSeason(animeParsedData.Aired.From)
				: await MatchSeason(animeParsedData.Premiered);

			await _animeRepository.UpdateAsync(anime);
		}

		private async Task UpdateCharacter(Character character, JikanDotNet.Character characterParsedData)
		{
			character.Name = characterParsedData.Name;
			character.About = characterParsedData.About;
			character.NameKanji = characterParsedData.NameKanji;
			character.Popularity = characterParsedData.MemberFavorites;

			character.ImageUrl = EmptyStringIfPlaceholder(characterParsedData.ImageURL);

			if (characterParsedData.Nicknames.Any())
			{
				character.Nicknames = string.Join(';', characterParsedData.Nicknames.ToArray());
			}

			await _characterRepository.UpdateAsync(character);
		}

		private async Task UpdateSeiyuu(Seiyuu seiyuu, Person seiyuuParsedData)
		{
			var japaneseName = string.Empty;

			seiyuu.Name = seiyuuParsedData.Name;
			seiyuu.Popularity = seiyuuParsedData.MemberFavorites;
			seiyuu.About = seiyuuParsedData.More;

			if (seiyuuParsedData.Birthday.HasValue)
			{
				seiyuu.Birthday = seiyuuParsedData.Birthday.Value.ToString("dd-MM-yyyy");
			}

			seiyuu.ImageUrl = EmptyStringIfPlaceholder(seiyuuParsedData.ImageURL);

			if (!string.IsNullOrWhiteSpace(seiyuuParsedData.FamilyName))
			{
				japaneseName += seiyuuParsedData.FamilyName;
			}

			if (!string.IsNullOrWhiteSpace(seiyuuParsedData.GivenName))
			{
				japaneseName += string.IsNullOrEmpty(japaneseName)
					? seiyuuParsedData.GivenName
					: " " + seiyuuParsedData.GivenName;

				seiyuu.JapaneseName = japaneseName;
			}

			await _seiyuuRepository.UpdateAsync(seiyuu);
		}

		#endregion Updating Entities

		#region Requests

		private async Task<JikanDotNet.Anime> SendSingleAnimeRequest(long malId, short retryCount)
		{
			JikanDotNet.Anime anime = null;
			await Task.Delay(3000 + retryCount * 10000);

			try
			{
				anime = await _jikan.GetAnime(malId);
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
				animeCharactersStaff = await _jikan.GetAnimeCharactersStaff(malId);
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
			Person person = null;
			await Task.Delay(3000 + retryCount * 10000);

			try
			{
				person = await _jikan.GetPerson(malId);
			}
			catch (Exception ex)
			{
				if (retryCount < 5)
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

			return person;
		}

		private async Task<JikanDotNet.Character> SendSingleCharacterRequest(long malId, short retryCount)
		{
			JikanDotNet.Character character = null;
			await Task.Delay(3000 + retryCount * 10000);

			try
			{
				character = await _jikan.GetCharacter(malId);
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
				season = await _jikan.GetSeason(year, seasonName);
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
			var foundType = await _animeTypeRepository.GetByNameAsync(typeName);

			return foundType?.Id;
		}

		private async Task<long?> MatchAnimeStatus(string statusName)
		{
			var foundStatus = await _animeStatusRepository.GetByNameAsync(statusName);

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

						var foundSeason = await _seasonRepository.GetAsync(x =>
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
					await _seasonRepository.GetAsync(x => x.Name.Equals(season.ToString()) && x.Year.Equals(year));

				return foundSeason?.Id;
			}
			catch (Exception)
			{
				return null;
			}
		}

		#endregion ForeignKeyMatching

		#region InsertingRoleRelatedEntities

		private async Task InsertRole(
			long seiyuuMalId,
			VoiceActingRole voiceActingRole,
			IReadOnlyList<Role> seiyuuRoles
		)
		{
			try
			{
				if (!seiyuuRoles.Any(x =>
					x.AnimeId.Equals(voiceActingRole.Anime.MalId) &&
					x.CharacterId.Equals(voiceActingRole.Character.MalId)))
				{
					var isCharacterInDatabase = true;
					var isAnimeInDatabase = true;

					if (await _animeRepository.GetAsync(voiceActingRole.Anime.MalId) == null)
					{
						isAnimeInDatabase = await InsertAnime(voiceActingRole);
					}

					if (await _characterRepository.GetAsync(voiceActingRole.Character.MalId) == null)
					{
						isCharacterInDatabase = await InsertCharacter(voiceActingRole);
					}

					if (isAnimeInDatabase && isCharacterInDatabase)
					{
						await _animeRoleRepository.AddAsync(new Role
						{
							LanguageId = 1, // Always japanese for now
							RoleTypeId = voiceActingRole.Role.Equals("Main") ? 1 : 2,
							AnimeId = voiceActingRole.Anime.MalId,
							CharacterId = voiceActingRole.Character.MalId,
							SeiyuuId = seiyuuMalId
						});

						_logger.Log($"Inserted {voiceActingRole.Character.Name} in {voiceActingRole.Anime.Name}");
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Log(
					$"Error during inserting with anime {voiceActingRole.Anime.Name}, character {voiceActingRole.Character.Name}: {ex.Message}");
			}
		}

		private async Task<bool> InsertAnime(VoiceActingRole voiceActingRole)
		{
			var existingAnime = await _animeRepository.GetAsync(voiceActingRole.Anime.MalId);

			if (existingAnime == null)
				try
				{
					var animeFullData = await SendSingleAnimeRequest(voiceActingRole.Anime.MalId, 0);
					var titleSynonym = string.Empty;

					if (animeFullData != null)
					{
						_logger.Log($"Parsed anime with id:{animeFullData.MalId}");

						if (animeFullData.TitleSynonyms.Any())
							titleSynonym = string.Join(';', animeFullData.TitleSynonyms.ToArray());

						await _animeRepository.AddAsync(
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
					_logger.Log(
						$"Error during inserting anime {voiceActingRole.Anime.Name} with id {voiceActingRole.Character.MalId}: {ex.Message}");
					return false;
				}

			return true; //already inserted
		}

		private async Task<bool> InsertCharacter(VoiceActingRole voiceActingRole)
		{
			var existingCharacter = await _characterRepository.GetAsync(voiceActingRole.Character.MalId);

			if (existingCharacter == null)
				try
				{
					var characterFullData = await SendSingleCharacterRequest(voiceActingRole.Character.MalId, 0);
					var nicknames = string.Empty;

					if (characterFullData != null)
					{
						_logger.Log($"Parsed id:{characterFullData.MalId}");

						if (characterFullData.Nicknames.Any())
							nicknames = string.Join(';', characterFullData.Nicknames.ToArray());

						await _characterRepository.AddAsync(
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
					_logger.Log(
						$"Error during inserting character {voiceActingRole.Anime.Name} with id {voiceActingRole.Anime.MalId}: {ex.Message}");
					return false;
				}

			return true; //already inserted
		}

		#endregion InsertingRoleRelatedEntities
	}
}
using JikanDotNet;
using JikanDotNet.Exceptions;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Logger;
using SeiyuuMoe.Repositories.Repositories;
using SeiyuuMoe.Repositories.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.JikanToDBParser
{
	public class JikanParser : IJikanParser
	{
		private ILoggingService logger;

		private IJikan jikan;

		private IAnimeRepository animeRepository;
		private ISeasonRepository seasonRepository;
		private ISeiyuuRepository seiyuuRepository;
		private IAnimeTypeRepository animeTypeRepository;
		private IAnimeStatusRepository animeStatusRepository;
		private IBlacklistedIdRepository blacklistedIdRepository;
		private IRoleRepository roleRepository;
		private ICharacterRepository characterRepository;

		public JikanParser(
			string endpointUrl,
			ILoggingService loggingService,
			IAnimeRepository animeRepository,
			IAnimeStatusRepository animeStatusRepository,
			IAnimeTypeRepository animeTypeRepository,
			IBlacklistedIdRepository blacklistedIdRepository,
			ICharacterRepository characterRepository,
			IRoleRepository roleRepository,
			ISeasonRepository seasonRepository,
			ISeiyuuRepository seiyuuRepository
			)
		{
			this.jikan = new Jikan(endpointUrl);
			this.logger = loggingService;
			this.animeRepository = animeRepository;
			this.seasonRepository = seasonRepository;
			this.seiyuuRepository = seiyuuRepository;
			this.animeTypeRepository = animeTypeRepository;
			this.animeStatusRepository = animeStatusRepository;
			this.blacklistedIdRepository = blacklistedIdRepository;
			this.characterRepository = characterRepository;
			this.roleRepository = roleRepository;
		}

		#region Interface Implementation

		public async Task InsertNewSeiyuu()
		{
			logger.Log("Started InsertNewSeiyuu job.");

			var lastSeiyuu = await seiyuuRepository.GetOrderedPageAsync(PredicateBuilder.True<Seiyuu>(), "MalId DESC", 0, 1);

			long lastId = lastSeiyuu.Results.First().MalId;

			string japaneseName = string.Empty;

			for (long malId = lastId + 1; malId < lastId + 100; malId++)
			{
				Person seiyuu = await SendSinglePersonRequest(malId, 0);

				if (seiyuu != null)
				{
					logger.Log($"Parsed id:{seiyuu.MalId}: {seiyuu.Name}");
				}
				else
				{
					logger.Log($"Omitted {malId} - not found");
					continue;
				}

				japaneseName = string.Empty;

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

				seiyuuRepository.Add(
					new Seiyuu
					{
						Name = seiyuu.Name,
						MalId = seiyuu.MalId,
						ImageUrl = EmptyStringIfPlaceholder(seiyuu.ImageURL),
						About = seiyuu.More,
						Birthday = seiyuu.Birthday.HasValue ? seiyuu.Birthday.Value.ToString("dd-MM-yyyy") : string.Empty,
						Popularity = seiyuu.MemberFavorites,
						JapaneseName = japaneseName
					}
				);

				await seiyuuRepository.CommitAsync();

				logger.Log($"Inserted {seiyuu.Name} with MalId: {seiyuu.MalId}");

				foreach (VoiceActingRole role in seiyuu.VoiceActingRoles)
				{
					await InsertRole(seiyuu.MalId, role, new List<Role>());
				}
			}

			logger.Log("Finished InsertNewSeiyuu job.");
		}

		public async Task ParseRoles()
		{
			logger.Log("Started ParseRoles job.");

			ICollection<long> seiyuuIdCollection = (await seiyuuRepository.GetAllAsync()).Select(x => x.MalId).ToList();

			foreach (long seiyuuMalId in seiyuuIdCollection)
			{
				try
				{
					var seiyuuRoles = await roleRepository.GetAllAsync(x => x.SeiyuuId.Equals(seiyuuMalId));

					Person seiyuuFullData = await SendSinglePersonRequest(seiyuuMalId, 0);

					logger.Log($"Parsing seiyuu with id {seiyuuMalId}");

					foreach (VoiceActingRole role in seiyuuFullData.VoiceActingRoles)
					{
						await InsertRole(seiyuuMalId, role, seiyuuRoles);
					}
				}
				catch (Exception ex)
				{
					logger.Log($"Error during parsing seiyuu with id {seiyuuMalId}: {ex.Message}");
					continue;
				}
			}
			logger.Log("Finished ParseRoles job.");
		}

		public async Task UpdateAllAnime()
		{
			logger.Log("Started UpdateAllAnime job.");

			int page = 0;
			int pageSize = 100;
			long totalAnimeCount = await animeRepository.CountAsync(x => true);

			while (page * pageSize < totalAnimeCount)
			{
				var animeCollection = await animeRepository.GetOrderedPageAsync(PredicateBuilder.True<Data.Model.Anime>(), "MalId ASC", page, pageSize);

				foreach (Data.Model.Anime anime in animeCollection.Results)
				{
					JikanDotNet.Anime animeFullData = await SendSingleAnimeRequest(anime.MalId, 0);

					if (animeFullData != null)
					{
						logger.Log($"Parsed anime with id {anime.MalId}: {anime.Title}");

						await UpdateAnime(anime, animeFullData);
					}
					else
					{
						logger.Log($"Error on {anime.MalId} - not found");
						continue;
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

				int page = 0;
				int pageSize = 100;
				long totalCharacterCount = await characterRepository.CountAsync(x => true);

				while (page * pageSize < totalCharacterCount)
				{
					var characterCollection = await characterRepository.GetOrderedPageAsync(PredicateBuilder.True<Data.Model.Character>(), "MalId ASC", page, pageSize);

					foreach (Data.Model.Character character in characterCollection.Results)
					{
						JikanDotNet.Character characterFullData = await SendSingleCharacterRequest(character.MalId, 0);

						if (characterFullData != null)
						{
							logger.Log($"Parsed character with id {character.MalId}: {character.Name}");

							await UpdateCharacter(character, characterFullData);
						}
						else
						{
							logger.Log($"Error on {character.MalId} - not found");
							continue;
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

			int page = 0;
			int pageSize = 100;
			long totalSeiyuuCount = await seiyuuRepository.CountAsync(x => true);

			while (page * pageSize < totalSeiyuuCount)
			{
				var seiyuuCollection = await seiyuuRepository.GetOrderedPageAsync(PredicateBuilder.True<Data.Model.Seiyuu>(), "MalId ASC", page, pageSize);

				foreach (Seiyuu seiyuu in seiyuuCollection.Results)
				{
					Person seiyuuFullData = await SendSinglePersonRequest(seiyuu.MalId, 0);

					if (seiyuuFullData != null)
					{
						logger.Log($"Parsed id:{seiyuu.MalId}, {seiyuu.Name}");

						await UpdateSeiyuu(seiyuu, seiyuuFullData);
					}
					else
					{
						logger.Error($"Error on {seiyuu.MalId} - not found");
						continue;
					}
				}

				page++;
			}
			logger.Log("Finished UpdateAllSeiyuu job.");
		}

		public async Task UpdateSeasons()
		{
			logger.Log("Started UpdateSeasons job.");

			int YearinNextSixMonths = DateTime.Now.AddMonths(6).Year;

			SeasonArchives seasonArchives = await jikan.GetSeasonArchive();

			foreach (var season in seasonArchives.Archives.First(x => x.Year.Equals(YearinNextSixMonths)).Season)
			{
				var insertedSeason = await seasonRepository.GetAsync(x => x.Name.Equals(season.ToString()) &&
					x.Year.Equals(YearinNextSixMonths));

				if (insertedSeason == null)
				{
					seasonRepository.Add(new Data.Model.Season()
					{
						Name = season.ToString(),
						Year = YearinNextSixMonths
					});
					await seasonRepository.CommitAsync();
				}
			}

			logger.Log("Finished UpdateSeasons job.");
		}

		public async Task InsertOldSeiyuu()
		{
			logger.Log("Started InsertOldSeiyuu job.");

			var seiyuuCollection = (await seiyuuRepository.GetAllAsync()).Select(x => x.MalId).OrderBy(x => x);

			string japaneseName = string.Empty;

			for (long malId = 1; malId < seiyuuCollection.Last(); malId++)
			{
				if (seiyuuCollection.Contains(malId))
				{
					logger.Log($"Omitted {malId} - already in database.");
					continue;
				}

				Person seiyuu = await SendSinglePersonRequest(malId, 0);

				if (seiyuu != null)
				{
					logger.Log($"Parsed id:{seiyuu.MalId}: {seiyuu.Name}");
				}
				else
				{
					logger.Log($"Omitted {malId} - not found");
					continue;
				}

				japaneseName = string.Empty;

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

				seiyuuRepository.Add(
					new Seiyuu
					{
						Name = seiyuu.Name,
						MalId = seiyuu.MalId,
						ImageUrl = EmptyStringIfPlaceholder(seiyuu.ImageURL),
						About = seiyuu.More,
						Birthday = seiyuu.Birthday.HasValue ? seiyuu.Birthday.Value.ToString("dd-MM-yyyy") : string.Empty,
						Popularity = seiyuu.MemberFavorites,
						JapaneseName = japaneseName
					}
				);

				await seiyuuRepository.CommitAsync();

				logger.Log($"Inserted {seiyuu.Name} with MalId: {seiyuu.MalId}");

				foreach (VoiceActingRole role in seiyuu.VoiceActingRoles)
				{
					await InsertRole(seiyuu.MalId, role, new List<Role>());
				}
			}

			logger.Log("Finished InsertOldSeiyuu job.");
		}

		#endregion Interface Implementation

		#region Additional parsing methods

		public async Task FilterNonJapanese()
		{
			IReadOnlyCollection<Seiyuu> seiyuuCollection = await seiyuuRepository.GetAllAsync();

			foreach (Seiyuu seiyuu in seiyuuCollection.OrderBy(x => x.MalId))
			{
				try
				{
					if (IsJapanese(seiyuu.JapaneseName))
					{
						logger.Log($"Parsed {seiyuu.MalId}: {seiyuu.Name}");
					}
					else
					{
						logger.Log($"Removing {seiyuu.MalId}: {seiyuu.Name}");
						seiyuuRepository.Delete(seiyuu);
						await seiyuuRepository.CommitAsync();
					}
				}
				catch (Exception ex)
				{
					logger.Error($"Error on {seiyuu.MalId} - {ex.Message}");
					continue;
				}
			}
		}

		#endregion Additional parsing methods

		#region Updating Entities

		private async Task UpdateAnime(Data.Model.Anime anime, JikanDotNet.Anime animeParsedData)
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
			anime.SeasonId = string.IsNullOrEmpty(animeParsedData.Premiered) ?
				await MatchSeason(animeParsedData.Aired.From) :
				await MatchSeason(animeParsedData.Premiered);

			animeRepository.Update(anime);

			await animeRepository.CommitAsync();
		}

		private async Task UpdateCharacter(Data.Model.Character character, JikanDotNet.Character characterParsedData)
		{
			character.Name = characterParsedData.Name;
			character.About = characterParsedData.About;
			character.ImageUrl = characterParsedData.ImageURL;
			character.NameKanji = characterParsedData.NameKanji;
			character.Popularity = characterParsedData.MemberFavorites;

			character.ImageUrl = EmptyStringIfPlaceholder(characterParsedData.ImageURL);

			if (characterParsedData.Nicknames.Any())
				character.Nicknames = string.Join(';', characterParsedData.Nicknames.ToArray());

			characterRepository.Update(character);

			await characterRepository.CommitAsync();
		}

		private async Task UpdateSeiyuu(Seiyuu seiyuu, Person seiyuuParsedData)
		{
			string japaneseName = string.Empty;

			seiyuu.Name = seiyuuParsedData.Name;
			seiyuu.Popularity = seiyuuParsedData.MemberFavorites;
			seiyuu.About = seiyuuParsedData.More;

			if (seiyuuParsedData.Birthday.HasValue)
				seiyuu.Birthday = seiyuuParsedData.Birthday.Value.ToString("dd-MM-yyyy");

			seiyuu.ImageUrl = EmptyStringIfPlaceholder(seiyuuParsedData.ImageURL);

			if (!string.IsNullOrWhiteSpace(seiyuuParsedData.FamilyName))
				japaneseName += seiyuuParsedData.FamilyName;

			if (!string.IsNullOrWhiteSpace(seiyuuParsedData.GivenName))
				japaneseName += string.IsNullOrEmpty(japaneseName) ? seiyuuParsedData.GivenName : " " + seiyuuParsedData.GivenName;

			seiyuu.JapaneseName = japaneseName;

			seiyuuRepository.Update(seiyuu);

			await seiyuuRepository.CommitAsync();
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
				{
					if (ex.InnerException is JikanRequestException && (ex.InnerException as JikanRequestException).ResponseCode == System.Net.HttpStatusCode.TooManyRequests)
					{
						retryCount++;
						return await SendSingleAnimeRequest(malId, retryCount);
					}
				}
			}

			return anime;
		}

		private async Task<JikanDotNet.AnimeCharactersStaff> SendSingleAnimeCharactersStaffRequest(long malId, short retryCount)
		{
			JikanDotNet.AnimeCharactersStaff animeCharactersStaff = null;
			await Task.Delay(3000 + retryCount * 10000);

			try
			{
				animeCharactersStaff = await jikan.GetAnimeCharactersStaff(malId);
			}
			catch (Exception ex)
			{
				if (retryCount < 10)
				{
					if (ex.InnerException is JikanRequestException && (ex.InnerException as JikanRequestException).ResponseCode == System.Net.HttpStatusCode.TooManyRequests)
					{
						retryCount++;
						return await SendSingleAnimeCharactersStaffRequest(malId, retryCount);
					}
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
					if (ex.InnerException is JikanRequestException && (ex.InnerException as JikanRequestException).ResponseCode == System.Net.HttpStatusCode.TooManyRequests)
					{
						retryCount++;
						return await SendSinglePersonRequest(malId, retryCount);
					}
					else
					{
						if (ex.InnerException is JikanRequestException)
						{
							System.Net.HttpStatusCode responseCode = (ex.InnerException as JikanRequestException).ResponseCode;

							switch (responseCode)
							{
								case (System.Net.HttpStatusCode.NotFound):
									BlacklistId(malId, "Seiyuu", "404 Not Found");
									break;

								case (System.Net.HttpStatusCode.InternalServerError):
									//	BlacklistId(malId, "Seiyuu", "Not exist");
									break;

								case (System.Net.HttpStatusCode.TooManyRequests):
									BlacklistId(malId, "Seiyuu", "429 Too much request");
									break;

								default:
									BlacklistId(malId, "Seiyuu", "Other");
									break;
							}
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
					if (ex.InnerException is JikanRequestException && (ex.InnerException as JikanRequestException).ResponseCode == System.Net.HttpStatusCode.TooManyRequests)
					{
						retryCount++;
						return await SendSingleCharacterRequest(malId, retryCount);
					}
					else
					{
						if (ex.InnerException is JikanRequestException)
						{
							System.Net.HttpStatusCode responseCode = (ex.InnerException as JikanRequestException).ResponseCode;

							switch (responseCode)
							{
								case (System.Net.HttpStatusCode.NotFound):
									BlacklistId(malId, "Character", "404 Not Found");
									break;

								case (System.Net.HttpStatusCode.InternalServerError):
									BlacklistId(malId, "Character", "Not exist");
									break;

								case (System.Net.HttpStatusCode.TooManyRequests):
									BlacklistId(malId, "Character", "429 Too much request");
									break;

								default:
									BlacklistId(malId, "Character", "Other");
									break;
							}
						}
					}
				}
			}

			return character;
		}

		private async Task<JikanDotNet.Season> SendSingleSeasonRequest(int year, Seasons seasonName, short retryCount)
		{
			JikanDotNet.Season season = null;
			await Task.Delay(3000 + retryCount * 10000);

			try
			{
				season = await jikan.GetSeason(year, seasonName);
			}
			catch (Exception ex)
			{
				if (retryCount < 5)
				{
					if (ex.InnerException is JikanRequestException)
					{
						if ((ex.InnerException as JikanRequestException).ResponseCode == System.Net.HttpStatusCode.TooManyRequests)
						{
							retryCount++;
							await SendSingleSeasonRequest(year, seasonName, retryCount);
						}
					}
				}
			}

			return season;
		}

		#endregion Requests

		#region ForeignKeyMatching

		private async Task<long?> MatchAnimeType(string typeName)
		{
			Data.Model.AnimeType foundType = await animeTypeRepository.GetAsync(x => x.Name.ToLower().Equals(typeName.ToLower()));

			return (foundType != null) ? foundType.Id : (long?)null;
		}

		private async Task<long?> MatchAnimeStatus(string statusName)
		{
			Data.Model.AnimeStatus foundStatus = await animeStatusRepository.GetAsync(x => x.Name.ToLower().Equals(statusName.ToLower()));

			return (foundStatus != null) ? foundStatus.Id : (long?)null;
		}

		private async Task<long?> MatchSeason(string seasonName)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(seasonName))
				{
					string[] seasonParts = seasonName.Split(' ');

					if (seasonParts.Length > 1)
					{
						int year = Int32.Parse(seasonParts[1]);
						string season = seasonParts[0];

						Data.Model.Season foundSeason = await seasonRepository.GetAsync(x => x.Name.ToLower().Equals(season.ToLower()) && x.Year.Equals(year));

						return (foundSeason != null) ? foundSeason.Id : (long?)null;
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
				if (airingFrom.HasValue)
				{
					DateTime airingDate = airingFrom.Value.Date;

					Seasons seasonEnumValue;
					int yearOfSeason = airingFrom.Value.Year;

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
				else
					return null;
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
				Data.Model.Season foundSeason = await seasonRepository.GetAsync(x => x.Name.Equals(season.ToString()) && x.Year.Equals(year));

				return (foundSeason != null) ? foundSeason.Id : (long?)null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		#endregion ForeignKeyMatching

		#region InsertingRoleRelatedEntities

		private async Task InsertRole(long seiyuuMalId, VoiceActingRole voiceActingRole, IReadOnlyList<Role> seiyuuRoles)
		{
			try
			{
				if (!seiyuuRoles.Any(x => x.AnimeId.Equals(voiceActingRole.Anime.MalId) && x.CharacterId.Equals(voiceActingRole.Character.MalId)))
				{
					bool isCharacterInDatabase = true;
					bool isAnimeInDatabase = true;

					if (await animeRepository.GetAsync(voiceActingRole.Anime.MalId) == null)
					{
						isAnimeInDatabase = await InsertAnime(voiceActingRole);
					}
					if (await characterRepository.GetAsync(voiceActingRole.Character.MalId) == null)
					{
						isCharacterInDatabase = await InsertCharacter(voiceActingRole);
					}

					if (isAnimeInDatabase && isCharacterInDatabase)
					{
						roleRepository.Add(new Role()
						{
							LanguageId = 1, // Always japanese for now
							RoleTypeId = voiceActingRole.Role.Equals("Main") ? 1 : 2,
							AnimeId = voiceActingRole.Anime.MalId,
							CharacterId = voiceActingRole.Character.MalId,
							SeiyuuId = seiyuuMalId
						});
						await roleRepository.CommitAsync();

						logger.Log($"Inserted {voiceActingRole.Character.Name} in {voiceActingRole.Anime.Name}");
					}
				}
			}
			catch (Exception ex)
			{
				logger.Log($"Error during inserting with anime {voiceActingRole.Anime.Name}, character {voiceActingRole.Character.Name}: {ex.Message}");
			}
		}

		private async Task<bool> InsertAnime(VoiceActingRole voiceActingRole)
		{
			var existingAnime = await animeRepository.GetAsync(voiceActingRole.Anime.MalId);

			if (existingAnime == null)
			{
				try
				{
					JikanDotNet.Anime animeFullData = await SendSingleAnimeRequest(voiceActingRole.Anime.MalId, 0);
					string titleSynonym = string.Empty;

					if (animeFullData != null)
					{
						logger.Log($"Parsed anime with id:{animeFullData.MalId}");

						if (animeFullData.TitleSynonyms.Any())
							titleSynonym = string.Join(';', animeFullData.TitleSynonyms.ToArray());

						animeRepository.Add(
							new Data.Model.Anime
							{
								MalId = animeFullData.MalId,
								ImageUrl = EmptyStringIfPlaceholder(animeFullData.ImageURL),
								Title = animeFullData.Title,
								Popularity = animeFullData.Members,
								About = animeFullData.Synopsis,
								JapaneseTitle = animeFullData.TitleJapanese,
								EnglishTitle = animeFullData.TitleEnglish,
								AiringDate = animeFullData.Aired.From.HasValue ? animeFullData.Aired.From.Value.ToString() : null,
								StatusId = await MatchAnimeStatus(animeFullData.Status),
								TypeId = await MatchAnimeType(animeFullData.Type),
								SeasonId = await MatchSeason(animeFullData.Premiered)
							}
						);
						await animeRepository.CommitAsync();
						return true;
					}
					return false;
				}
				catch (Exception ex)
				{
					logger.Log($"Error during inserting anime {voiceActingRole.Anime.Name} with id {voiceActingRole.Character.MalId}: {ex.Message}");
					return false;
				}
			}
			else
				return true; //already inserted
		}

		private async Task<bool> InsertCharacter(VoiceActingRole voiceActingRole)
		{
			var existingCharacter = await characterRepository.GetAsync(voiceActingRole.Character.MalId);

			if (existingCharacter == null)
			{
				try
				{
					JikanDotNet.Character characterFullData = await SendSingleCharacterRequest(voiceActingRole.Character.MalId, 0);
					string nicknames = string.Empty;

					if (characterFullData != null)
					{
						logger.Log($"Parsed id:{characterFullData.MalId}");

						if (characterFullData.Nicknames.Any())
							nicknames = string.Join(';', characterFullData.Nicknames.ToArray());

						characterRepository.Add(
							new Data.Model.Character
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
						await characterRepository.CommitAsync();
						return true;
					}
					return false;
				}
				catch (Exception ex)
				{
					logger.Log($"Error during inserting character {voiceActingRole.Anime.Name} with id {voiceActingRole.Anime.MalId}: {ex.Message}");
					return false;
				}
			}
			else
				return true; //already inserted
		}

		#endregion InsertingRoleRelatedEntities

		private void BlacklistId(long id, string type, string reason = null)
		{
			BlacklistedId blacklistedId = new BlacklistedId()
			{
				MalId = id,
				EntityType = type,
				Reason = reason
			};

			blacklistedIdRepository.Add(blacklistedId);
			blacklistedIdRepository.CommitAsync();
		}

		private bool IsJapanese(string japaneseName)
		{
			return !string.IsNullOrWhiteSpace(japaneseName) && 
				japaneseName.All(x =>
				(x >= 0x4E00 && x <= 0x9FBF) || // kanji
				(x >= 0x3040 && x <= 0x309F) || // hiragana
				(x >= 0x30A0 && x <= 0x30FF) || // katakana
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
	}
}
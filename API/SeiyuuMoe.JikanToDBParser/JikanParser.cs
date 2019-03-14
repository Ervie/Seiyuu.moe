using JikanDotNet;
using JikanDotNet.Exceptions;
using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Data.Context;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Logger;
using SeiyuuMoe.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SeiyuuMoe.JikanToDBParser
{
	public static class JikanParser
	{
		private static SeiyuuMoeContext dbContext = new SeiyuuMoeContext("SeiyuuMoeDB.db");

		private static LoggingService logger = new LoggingService();

		private static IJikan jikan;
		private static IAnimeRepository animeRepository;
		private static ISeasonRepository seasonRepository;
		private static ISeiyuuRepository seiyuuRepository;
		private static IAnimeTypeRepository animeTypeRepository;
		private static IAnimeStatusRepository animeStatusRepository;
		private static IBlacklistedIdRepository blacklistedIdRepository;
		private static IRoleRepository roleRepository;
		private static ICharacterRepository characterRepository;

		static JikanParser()
		{
			jikan = new Jikan(true, false);

			dbContext.Database.EnsureCreated();
			dbContext.Database.Migrate();

			animeRepository = new AnimeRepository(dbContext);
			seasonRepository = new SeasonRepository(dbContext);
			seiyuuRepository = new SeiyuuRepository(dbContext);
			animeTypeRepository = new AnimeTypeRepository(dbContext);
			animeStatusRepository = new AnimeStatusRepository(dbContext);
			blacklistedIdRepository = new BlacklistedIdRepository(dbContext);
			characterRepository = new CharacterRepository(dbContext);
			roleRepository = new RoleRepository(dbContext);
		}

		public static void ParseAnime()
		{
			JikanDotNet.Season seasonToParse;
			var archiveSeasons = jikan.GetSeasonArchive().Result;

			if (archiveSeasons != null)
			{
				foreach (SeasonArchive year in archiveSeasons.Archives.Reverse())
				{
					foreach (var season in year.Season)
					{
						Thread.Sleep(2010);

						logger.Log($"Currently parsing {year.Year} {season}");

						seasonToParse = jikan.GetSeason(year.Year, season).Result;

						if (seasonToParse == null)
						{
							Thread.Sleep(15000);

							seasonToParse = jikan.GetSeason(year.Year, season).Result;
						}

						foreach (var anime in seasonToParse.SeasonEntries)
						{
							if (dbContext.Anime.FirstOrDefault(x => x.MalId == anime.MalId) == null)
							{
								dbContext.Anime.Add(
									new Data.Model.Anime
									{
										MalId = anime.MalId,
										ImageUrl = anime.ImageURL,
										Title = anime.Title,
										Popularity = anime.Members.Value
									}
								);

								logger.Log($"Inserted anime with Id {anime.MalId}: {anime.Title} with MalId = {anime.MalId}");
							}
							dbContext.SaveChanges();
						}
					}
				}
			}
		}

		public static void ParseSeiyuu()
		{
			const int minId = 1;
			const int maxId = 48000;

			dbContext.Database.ExecuteSqlCommand("DELETE FROM Seiyuus;");
			dbContext.SaveChanges();
			logger.Log("Cleared database");

			for (int malId = minId; malId < maxId; malId++)
			{
				Person seiyuu = SendSinglePersonRequest(malId, 0).Result;

				if (seiyuu != null)
				{
					logger.Log($"Parsed id:{seiyuu.MalId}");
				}
				else
				{
					logger.Log($"Omitted {malId} - not found");
					continue;
				}

				if (string.IsNullOrWhiteSpace(seiyuu.GivenName) && string.IsNullOrWhiteSpace(seiyuu.FamilyName))
				{
					logger.Log($"Omitted {seiyuu.Name} - not Japanese");
					continue;
				}

				if (seiyuu.VoiceActingRoles.Count <= 0)
				{
					logger.Log($"Omitted {seiyuu.Name} - not a seiyuu");
					continue;
				}

				if (dbContext.Seiyuu.FirstOrDefault(x => x.MalId == seiyuu.MalId) != null)
				{
					logger.Log($"Omitted {seiyuu.Name} - already in database");
					continue;
				}

				dbContext.Seiyuu.Add(
					new Seiyuu
					{
						Name = seiyuu.Name,
						MalId = seiyuu.MalId,
						ImageUrl = seiyuu.ImageURL
					}
				);
				dbContext.SaveChanges();

				logger.Log($"Inserted {seiyuu.Name} with MalId: {seiyuu.MalId}");
			}
		}

		public static void ParseCharacter()
		{
			const int minId = 1;
			const int maxId = 166416;

			ICollection<long> blacklistedIds = blacklistedIdRepository.GetAllAsync(x => x.EntityType == "Character").Result.Select(x => x.MalId).ToList();
			ICollection<long> alreadyInsertedIds = characterRepository.GetAllAsync().Result.Select(x => x.MalId).ToList();

			for (int malId = minId; malId < maxId; malId++)
			{
				if (blacklistedIds.Contains(malId))
				{
					logger.Log($"Ommitting id {malId} - blacklisted.");
					continue;
				}

				if (alreadyInsertedIds.Contains(malId))
				{
					logger.Log($"Ommitting id {malId} - already inserted.");
					continue;
				}

				JikanDotNet.Character characterFullData = SendSingleCharacterRequest(malId, 0).Result;
				string nicknames = string.Empty;

				if (characterFullData != null)
				{
					logger.Log($"Parsed id:{characterFullData.MalId}");
				}
				else
				{
					logger.Log($"Omitted {malId} - not found");
					continue;
				}


				if (characterFullData.Nicknames.Any())
					nicknames = string.Join(';', characterFullData.Nicknames.ToArray());

				characterRepository.Add(
					new Data.Model.Character
					{
						MalId = malId,
						ImageUrl = characterFullData.ImageURL,
						Name = characterFullData.Name,
						Popularity = characterFullData.MemberFavorites,
						About = characterFullData.About,
						NameKanji = characterFullData.NameKanji,
						Nicknames = nicknames
					}
				);
				dbContext.SaveChanges();
			}
		}

		public static void ParseSeason()
		{
			SeasonArchives seasonArchives = jikan.GetSeasonArchive().Result;

			foreach (SeasonArchive archive in seasonArchives.Archives.Reverse())
			{
				foreach (var season in archive.Season)
				{
					seasonRepository.Add(new Data.Model.Season()
					{
						Name = season.ToString(),
						Year = archive.Year
					});
					seasonRepository.Commit();
				}
			}
		}

		public async static Task ParseRole()
		{
			ICollection<long> seiyuuIdCollection = dbContext.Seiyuu.Select(x => x.MalId).ToList();

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
		}

		public static void ParseSeiyuuAdditional()
		{
			ICollection<Seiyuu> seiyuuCollection = dbContext.Seiyuu.Where(x => x.Birthday == null).ToList();
			string japaneseName = string.Empty;

			foreach (Seiyuu seiyuu in seiyuuCollection)
			{
				Person seiyuuFullData = SendSinglePersonRequest(seiyuu.MalId, 0).Result;
				japaneseName = string.Empty;

				if (seiyuuFullData != null)
				{
					logger.Log($"Parsed id:{seiyuu.MalId}");

					seiyuu.Name = seiyuuFullData.Name;
					seiyuu.ImageUrl = seiyuuFullData.ImageURL;
					seiyuu.Popularity = seiyuuFullData.MemberFavorites;
					seiyuu.About = seiyuuFullData.More;
					seiyuu.Birthday = seiyuuFullData.Birthday.ToString();

					if (!string.IsNullOrWhiteSpace(seiyuuFullData.FamilyName))
						japaneseName += seiyuuFullData.FamilyName;

					if (!string.IsNullOrWhiteSpace(seiyuuFullData.GivenName))
						japaneseName += string.IsNullOrEmpty(japaneseName) ? seiyuuFullData.GivenName : " " + seiyuuFullData.GivenName;

					seiyuu.JapaneseName = japaneseName;

					dbContext.SaveChanges();
				}
				else
				{
					logger.Log($"Error on {seiyuu.MalId} - not found");
					continue;
				}
			}
		}

		public async static Task ParseAnimeAdditional()
		{
			ICollection<Data.Model.Anime> animeCollection = dbContext.Anime.Where(x => !x.StatusId.HasValue).ToList();
			string japaneseName = string.Empty;

			foreach (Data.Model.Anime anime in animeCollection)
			{
				JikanDotNet.Anime animeFullData = await SendSingleAnimeRequest(anime.MalId, 0);
				japaneseName = string.Empty;

				if (animeFullData != null)
				{
					logger.Log($"Parsed id:{anime.MalId}");

					anime.Title = animeFullData.Title;
					anime.About = animeFullData.Synopsis;
					anime.ImageUrl = animeFullData.ImageURL;
					anime.JapaneseTitle = animeFullData.TitleJapanese;
					anime.Popularity = animeFullData.Members;

					if (animeFullData.Aired.From.HasValue)
						anime.AiringDate = animeFullData.Aired.From.ToString();

					if (animeFullData.TitleSynonyms.Count > 0)
						anime.TitleSynonyms = string.Join(';', animeFullData.TitleSynonyms);

					anime.TypeId = MatchAnimeType(animeFullData.Type);
					anime.StatusId = MatchAnimeStatus(animeFullData.Status);
					anime.SeasonId = MatchSeason(animeFullData.Premiered);

					dbContext.SaveChanges();
				}
				else
				{
					logger.Log($"Error on {anime.MalId} - not found");
					continue;
				}
			}
		}

		public static void ParseSeasonAdditional()
		{
			var archiveSeasons = jikan.GetSeasonArchive().Result;

			if (archiveSeasons != null)
			{
				foreach (SeasonArchive year in archiveSeasons.Archives.Reverse())
				{
					foreach (var season in year.Season)
					{
						JikanDotNet.Season seasonToParse = SendSingleSeasonRequest(year.Year, season, 0);

						if (seasonToParse != null)
						{
							logger.Log($"Currently parsing {year.Year} {season}");

							foreach (var anime in seasonToParse.SeasonEntries)
							{
								Data.Model.Anime animeToUpdate = animeRepository.GetAsync(x => x.MalId == anime.MalId).Result;
								if (animeToUpdate != null && (anime.Continued.HasValue && !anime.Continued.Value))
								{
									animeToUpdate.SeasonId = MatchSeason(year.Year, season);

								}
								dbContext.SaveChanges();
							}
						}
						else
						{
							logger.Log($"Error on {year.Year} {season} - not found");
							continue;
						}
					}
				}
			}
		}

		#region Requests

		private async static Task<JikanDotNet.Anime> SendSingleAnimeRequest(long malId, short retryCount)
		{
			JikanDotNet.Anime anime = null;
			Thread.Sleep(3000 + retryCount * 10000);

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

		private async static Task<Person> SendSinglePersonRequest(long malId, short retryCount)
		{
			Person seiyuu = null;
			Thread.Sleep(3000 + retryCount * 10000);

			try
			{
				seiyuu = jikan.GetPerson(malId).Result;
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
				}
			}

			return seiyuu;
		}

		private static async Task<JikanDotNet.Character> SendSingleCharacterRequest(long malId, short retryCount)
		{
			JikanDotNet.Character character = null;
			Thread.Sleep(3000 + retryCount * 10000);

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

		private static JikanDotNet.Season SendSingleSeasonRequest(int year, Seasons seasonName, short retryCount)
		{
			JikanDotNet.Season season = null;
			Thread.Sleep(3000 + retryCount * 10000);

			try
			{
				season = jikan.GetSeason(year, seasonName).Result;
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
							SendSingleSeasonRequest(year, seasonName, retryCount);
						}
					}
				}
			}

			return season;
		}

		#endregion Requests

		#region ForeignKeyMatching

		private static long? MatchAnimeType(string typeName)
		{
			Data.Model.AnimeType foundType = animeTypeRepository.GetAsync(x => x.Name.ToLower().Equals(typeName.ToLower())).Result;

			return (foundType != null) ? foundType.Id : (long?)null;
		}

		private static long? MatchAnimeStatus(string statusName)
		{
			Data.Model.AnimeStatus foundStatus = animeStatusRepository.GetAsync(x => x.Name.ToLower().Equals(statusName.ToLower())).Result;

			return (foundStatus != null) ? foundStatus.Id : (long?)null;
		}

		private static long? MatchSeason(string seasonName)
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

						Data.Model.Season foundSeason = seasonRepository.GetAsync(x => x.Name.ToLower().Equals(season.ToLower()) && x.Year.Equals(year)).Result;

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

		private static long? MatchSeason(int year, Seasons season)
		{
			try
			{
				Data.Model.Season foundSeason = seasonRepository.GetAsync(x => x.Name.Equals(season.ToString()) && x.Year.Equals(year)).Result;

				return (foundSeason != null) ? foundSeason.Id : (long?)null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		#endregion

		#region InsertingRoleRelatedEntities

		private async static Task InsertRole(long seiyuuMalId, VoiceActingRole voiceActingRole, IReadOnlyList<Role> seiyuuRoles)
		{
			try
			{
				if (!seiyuuRoles.Any(x => x.AnimeId.Equals(voiceActingRole.Anime.MalId) && x.CharacterId.Equals(voiceActingRole.Character.MalId)))
				{
					roleRepository.Add(new Role()
					{
						LanguageId = 1, // Always japanese for now
						RoleTypeId = voiceActingRole.Role.Equals("Main") ? 1 : 2,
						AnimeId = voiceActingRole.Anime.MalId,
						CharacterId = voiceActingRole.Character.MalId,
						SeiyuuId = seiyuuMalId
					});
					dbContext.SaveChanges();

					logger.Log($"Inserted {voiceActingRole.Character.Name} in {voiceActingRole.Anime.Name}");
				}
				else
				{
					logger.Log($"Ommitting id {voiceActingRole.Character.Name} in {voiceActingRole.Anime.Name} - already inserted.");
				}
			}
			catch (Exception ex)
			{
				logger.Log($"Error during inserting with anime {voiceActingRole.Anime.Name}, character {voiceActingRole.Character.Name}: {ex.Message}");
				RecreateDbContext();
				if (await InsertCharacter(voiceActingRole) && await InsertAnime(voiceActingRole))
					await InsertRole(seiyuuMalId, voiceActingRole, seiyuuRoles);
				else
					logger.Log($"Could not insert anime with id {voiceActingRole.Anime.MalId}, or character with id {voiceActingRole.Character.MalId}, omitting role.");
			}
		}

		private async static Task<bool> InsertAnime(VoiceActingRole voiceActingRole)
		{
			var existingAnime = await animeRepository.GetAsync(voiceActingRole.Anime.MalId);

			if (existingAnime == null)
			{
				try
				{
					JikanDotNet.Anime animeFullData = SendSingleAnimeRequest(voiceActingRole.Anime.MalId, 0).Result;
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
								ImageUrl = animeFullData.ImageURL,
								Title = animeFullData.Title,
								Popularity = animeFullData.Members,
								About = animeFullData.Synopsis,
								JapaneseTitle = animeFullData.TitleJapanese,
								AiringDate = animeFullData.Aired.From.HasValue ? animeFullData.Aired.From.Value.ToString() : null,
								StatusId = MatchAnimeStatus(animeFullData.Status),
								TypeId = MatchAnimeType(animeFullData.Type),
								SeasonId = MatchSeason(animeFullData.Premiered)
							}
						);
						dbContext.SaveChanges();
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

		private async static Task<bool> InsertCharacter(VoiceActingRole voiceActingRole)
		{
			var existingCharacter = await characterRepository.GetAsync(voiceActingRole.Character.MalId);

			if (existingCharacter == null)
			{
				try
				{
					JikanDotNet.Character characterFullData = SendSingleCharacterRequest(voiceActingRole.Character.MalId, 0).Result;
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
								ImageUrl = characterFullData.ImageURL,
								Name = characterFullData.Name,
								Popularity = characterFullData.MemberFavorites,
								About = characterFullData.About,
								NameKanji = characterFullData.NameKanji,
								Nicknames = nicknames
							}
						);
						dbContext.SaveChanges();
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

		#endregion

		private static void BlacklistId(long id, string type, string reason = null)
		{
			BlacklistedId blacklistedId = new BlacklistedId()
			{
				MalId = id,
				EntityType = type,
				Reason = reason
			};

			blacklistedIdRepository.Add(blacklistedId);
			dbContext.SaveChanges();
		}

		private static void RecreateDbContext()
		{
			dbContext = new SeiyuuMoeContext("SeiyuuMoeDB.db");
			animeRepository = new AnimeRepository(dbContext);
			seasonRepository = new SeasonRepository(dbContext);
			seiyuuRepository = new SeiyuuRepository(dbContext);
			animeTypeRepository = new AnimeTypeRepository(dbContext);
			animeStatusRepository = new AnimeStatusRepository(dbContext);
			blacklistedIdRepository = new BlacklistedIdRepository(dbContext);
			characterRepository = new CharacterRepository(dbContext);
			roleRepository = new RoleRepository(dbContext);
		}
	}
}
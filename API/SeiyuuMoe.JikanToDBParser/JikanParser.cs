using JikanDotNet;
using JikanDotNet.Exceptions;
using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Data.Context;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SeiyuuMoe.JikanToDBParser
{
	public static class JikanParser
	{
		private static readonly SeiyuuMoeContext dbContext = new SeiyuuMoeContext("SeiyuuMoeDB.db");

		private readonly static IJikan jikan;
		private readonly static ISeasonRepository seasonRepository;
		private readonly static ISeiyuuRepository seiyuuRepository;
		private readonly static IAnimeTypeRepository animeTypeRepository;
		private readonly static IAnimeStatusRepository animeStatusRepository;

		static JikanParser()
		{
			jikan = new Jikan(true, false);

			dbContext.Database.EnsureCreated();
			dbContext.Database.Migrate();

			seasonRepository = new SeasonRepository(dbContext);
			seiyuuRepository = new SeiyuuRepository(dbContext);
			animeTypeRepository = new AnimeTypeRepository(dbContext);
			animeStatusRepository = new AnimeStatusRepository(dbContext);
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

						System.Console.WriteLine($"Currently parsing {year.Year} {season}");

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

								System.Console.WriteLine($"Inserted anime with Id {anime.MalId}: {anime.Title} with MalId = {anime.MalId}");
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
			Console.WriteLine("Cleared database");

			for (int malId = minId; malId < maxId; malId++)
			{
				Person seiyuu = SendSinglePersonRequest(malId, false);

				if (seiyuu != null)
				{
					Console.WriteLine($"Parsed id:{seiyuu.MalId}");
				}
				else
				{
					Console.WriteLine($"Omitted {malId} - not found");
					continue;
				}

				if (string.IsNullOrWhiteSpace(seiyuu.GivenName) && string.IsNullOrWhiteSpace(seiyuu.FamilyName))
				{
					Console.WriteLine($"Omitted {seiyuu.Name} - not Japanese");
					continue;
				}

				if (seiyuu.VoiceActingRoles.Count <= 0)
				{
					Console.WriteLine($"Omitted {seiyuu.Name} - not a seiyuu");
					continue;
				}

				if (dbContext.Seiyuu.FirstOrDefault(x => x.MalId == seiyuu.MalId) != null)
				{
					Console.WriteLine($"Omitted {seiyuu.Name} - already in database");
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

				Console.WriteLine($"Inserted {seiyuu.Name} with MalId: {seiyuu.MalId}");
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

		public static void ParseSeiyuuAdditional()
		{
			ICollection<Seiyuu> seiyuuCollection = dbContext.Seiyuu.Where(x => x.Birthday == null).ToList();
			string japaneseName = string.Empty;

			foreach (Seiyuu seiyuu in seiyuuCollection)
			{
				Person seiyuuFullData = SendSinglePersonRequest(seiyuu.MalId, 0);
				japaneseName = string.Empty;

				if (seiyuuFullData != null)
				{
					Console.WriteLine($"{DateTime.Now}: Parsed id:{seiyuu.Id}, MalId:{seiyuu.MalId}");

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
					Console.WriteLine($"Error on {seiyuu.MalId} - not found");
					continue;
				}
			}
		}

		public static void ParseAnimeAdditional()
		{
			ICollection<Data.Model.Anime> animeCollection = dbContext.Anime.Where(x => !x.StatusId.HasValue).ToList();
			string japaneseName = string.Empty;

			foreach (Data.Model.Anime anime in animeCollection)
			{
				JikanDotNet.Anime animeFullData = SendSingleAnimeRequest(anime.MalId, 0);
				japaneseName = string.Empty;

				if (animeFullData != null)
				{
					Console.WriteLine($"{DateTime.Now}: Parsed id:{anime.Id}, malId:{anime.MalId}");

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
					Console.WriteLine($"Error on {anime.MalId} - not found");
					continue;
				}
			}
		}

		#region Requests

		private static JikanDotNet.Anime SendSingleAnimeRequest(long malId, short retryCount)
		{
			JikanDotNet.Anime anime = null;
			Thread.Sleep(3000 + retryCount * 10000);

			try
			{
				anime = jikan.GetAnime(malId).Result;
			}
			catch (Exception ex)
			{
				if (retryCount < 10)
				{
					if (ex.InnerException is JikanRequestException && (ex.InnerException as JikanRequestException).ResponseCode == System.Net.HttpStatusCode.TooManyRequests)
					{
						retryCount++;
						return SendSingleAnimeRequest(malId, retryCount);
					}
				}
			}

			return anime;
		}

		private static Person SendSinglePersonRequest(long malId, short retryCount)
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
						SendSinglePersonRequest(malId, retryCount);
					}
				}
			}

			return seiyuu;
		}

		private static Person SendSinglePersonRequest(int malId, bool retry)
		{
			Person seiyuu = null;
			Thread.Sleep(3000);

			try
			{
				seiyuu = jikan.GetPerson(malId).Result;
			}
			catch (Exception ex)
			{
				if (ex.InnerException is JikanRequestException && !retry)
				{
					if ((ex.InnerException as JikanRequestException).ResponseCode == System.Net.HttpStatusCode.TooManyRequests)
					{
						// Additional extra time to recover from IP ban
						Thread.Sleep(30 * 1000);
						SendSinglePersonRequest(malId, true);
					}
				}
			}

			return seiyuu;
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

		#endregion
	}
}
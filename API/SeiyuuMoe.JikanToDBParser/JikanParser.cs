using JikanDotNet;
using JikanDotNet.Exceptions;
using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Data;
using SeiyuuMoe.Data.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SeiyuuMoe.JikanToDBParser
{
	public static class JikanParser
	{
		private readonly static DatabaseContext dbContext = new DatabaseContext();

		private readonly static IJikan jikan;

		static JikanParser()
		{
			jikan = new Jikan(true, false);

			dbContext.Database.EnsureCreated();
			dbContext.Database.Migrate();
		}

		public static void ParseAnime()
		{
			Season seasonToParse;
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
									new AnimeSnippet
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

				if (dbContext.Seiyuus.FirstOrDefault(x => x.MalId == seiyuu.MalId) != null)
				{
					Console.WriteLine($"Omitted {seiyuu.Name} - already in database");
					continue;
				}

				dbContext.Seiyuus.Add(
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

		public static void ParseSeiyuuAdditional()
		{
			ICollection<Seiyuu> seiyuuCollection = dbContext.Seiyuus.Where(x => !x.Popularity.HasValue).ToList();
			string japaneseName = string.Empty;

			foreach (Seiyuu seiyuu in seiyuuCollection)
			{
				Person seiyuuFullData = SendSinglePersonRequest(seiyuu.MalId, 0);
				japaneseName = string.Empty;

				if (seiyuuFullData != null)
				{
					Console.WriteLine($"Parsed id:{seiyuu.MalId}");

					seiyuu.Popularity = seiyuuFullData.MemberFavorites;
					seiyuu.NumberOfRoles = seiyuuFullData.VoiceActingRoles.Count;

					if (!string.IsNullOrWhiteSpace(seiyuuFullData.GivenName))
						japaneseName += seiyuuFullData.GivenName;

					if (!string.IsNullOrWhiteSpace(seiyuuFullData.FamilyName))
						japaneseName += seiyuuFullData.FamilyName;

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

		public static Person SendSinglePersonRequest(int malId, bool retry)
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

		public static Person SendSinglePersonRequest(long malId, short retryCount)
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

		public static void ParseAnimeAdditional()
		{
			ICollection<AnimeSnippet> animeCollection = dbContext.Anime.Where(x => !x.AiringFrom.HasValue).ToList();
			string japaneseName = string.Empty;

			foreach (AnimeSnippet anime in animeCollection)
			{
				Anime animeFullData = SendSingleAnimeRequest(anime.MalId, 0);
				japaneseName = string.Empty;

				if (animeFullData != null)
				{
					Console.WriteLine($"Parsed id:{anime.Id}, malId:{anime.MalId}");

					if (animeFullData.Aired.From.HasValue)
						anime.AiringFrom = animeFullData.Aired.From;

					if (animeFullData.TitleSynonyms.Count > 0)
						anime.TitleSynonyms = string.Join(';', animeFullData.TitleSynonyms);

					dbContext.SaveChanges();
				}
				else
				{
					Console.WriteLine($"Error on {anime.MalId} - not found");
					continue;
				}
			}
		}

		private static Anime SendSingleAnimeRequest(long malId, short retryCount)
		{
			Anime anime = null;
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
	}
}
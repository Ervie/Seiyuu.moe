using SeiyuuMoe.Animes;
using SeiyuuMoe.Domain.ComparisonEntities;
using System;

namespace SeiyuuMoe.Application.Animes.Extensions
{
	public static class AnimeExtensions
	{
		private const string _malAnimeBaseUrl = "https://myanimelist.net/anime/";

		public static AnimeSearchEntryDto ToAnimeSearchEntryDto(this Domain.Entities.Anime anime)
			=> new AnimeSearchEntryDto(anime.Title, anime.ImageUrl, anime.MalId);

		public static AnimeCardDto ToAnimeCardDto(this Domain.Entities.Anime anime)
			=> new AnimeCardDto(
				anime.Title,
				anime.ImageUrl,
				anime.MalId,
				anime.JapaneseTitle,
				anime.TitleSynonyms,
				anime.About,
				!string.IsNullOrWhiteSpace(anime.AiringDate)
					? DateTime.ParseExact(anime.AiringDate, "dd-MM-yyyy", null)
					: (DateTime?)null,
				anime.Status?.Name ?? string.Empty,
				anime.Type?.Name ?? string.Empty,
				anime.Season != null
					? anime.Season.Name + anime.Season.Year
					: string.Empty
			);

		public static AnimeTableEntry ToAnimeTableEntry(this Domain.Entities.Anime anime)
			=> new AnimeTableEntry(
				anime.MalId,
				anime.Title,
				anime.ImageUrl,
				_malAnimeBaseUrl + anime.MalId,
				!string.IsNullOrWhiteSpace(anime.AiringDate)
					? DateTime.ParseExact(anime.AiringDate, "dd-MM-yyyy", null)
					: (DateTime?)null
			);
	}
}
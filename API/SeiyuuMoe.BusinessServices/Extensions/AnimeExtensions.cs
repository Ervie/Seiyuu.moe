using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Data.Model;
using System;

namespace SeiyuuMoe.BusinessServices.Extensions
{
	public static class AnimeExtensions
	{
		private const string malAnimeBaseUrl = "https://myanimelist.net/anime/";

		public static AnimeSearchEntryDto ToAnimeSearchEntryDto(this Anime anime)
			=> new AnimeSearchEntryDto(anime.Title, anime.ImageUrl, anime.MalId);

		public static AnimeCardDto ToAnimeCardDto(this Anime anime)
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

		public static AnimeTableEntryDto ToAnimeTableEntryDto(this Anime anime)
			=> new AnimeTableEntryDto(
				anime.MalId,
				anime.Title,
				anime.ImageUrl,
				malAnimeBaseUrl + anime.MalId,
				!string.IsNullOrWhiteSpace(anime.AiringDate)
					? DateTime.ParseExact(anime.AiringDate, "dd-MM-yyyy", null)
					: (DateTime?)null
			);
	}
}
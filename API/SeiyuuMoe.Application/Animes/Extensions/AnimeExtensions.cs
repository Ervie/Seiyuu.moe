using SeiyuuMoe.Animes;
using SeiyuuMoe.Domain.ComparisonEntities;
using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Application.Animes.Extensions
{
	public static class AnimeExtensions
	{
		private const string MalAnimeBaseUrl = "https://myanimelist.net/anime/";

		public static AnimeSearchEntryDto ToAnimeSearchEntryDto(this Anime anime)
			=> new AnimeSearchEntryDto(anime.Title, anime.ImageUrl, anime.MalId);

		public static AnimeCardDto ToAnimeCardDto(this Anime anime) => 
			anime == null
				? null
				: new AnimeCardDto(
					anime.Title,
					anime.ImageUrl,
					anime.MalId,
					anime.KanjiTitle,
					anime.TitleSynonyms,
					anime.About,
					anime.AiringDate,
					anime.Status?.Description ?? string.Empty,
					anime.Type?.Description ?? string.Empty,
					anime.Season != null
						? anime.Season.Name + ' ' + anime.Season.Year
						: string.Empty
				);

		public static AnimeTableEntry ToAnimeTableEntry(this Anime anime)
			=> new AnimeTableEntry(
				anime.MalId,
				anime.Title,
				anime.ImageUrl,
				MalAnimeBaseUrl + anime.MalId,
				anime.AiringDate
			);
	}
}
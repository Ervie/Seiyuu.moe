using SeiyuuMoe.Application.Anime.SearchAnime;
using SeiyuuMoe.Application.Season.GetSeasonSummaries;
using SeiyuuMoe.Domain.Enums;

namespace SeiyuuMoe.BusinessServices.Extensions
{
	public static class GetSeasonSummariesQueryExtensions
	{
		public static SearchAnimeQuery ToSearchAnimeQuery(this GetSeasonSummariesQuery GetSeasonSummariesQuery)
			=> new SearchAnimeQuery
			{
				AnimeTypeId = GetSeasonSummariesQuery.TVSeriesOnly
					? (long)AnimeTypeDictionary.TV
					: (long)AnimeTypeDictionary.AllTypes,
				SeasonId = GetSeasonSummariesQuery.Id
			};
	}
}
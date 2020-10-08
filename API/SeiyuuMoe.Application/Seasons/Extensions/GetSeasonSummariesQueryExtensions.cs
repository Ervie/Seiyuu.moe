using SeiyuuMoe.Application.Animes.SearchAnime;
using SeiyuuMoe.Application.Seasons.GetSeasonSummaries;
using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Application.Seasons.Extensions
{
	public static class GetSeasonSummariesQueryExtensions
	{
		public static SearchAnimeQuery ToSearchAnimeQuery(this GetSeasonSummariesQuery GetSeasonSummariesQuery)
			=> new SearchAnimeQuery
			{
				AnimeTypeId = GetSeasonSummariesQuery.TVSeriesOnly
					? (long)AnimeTypeId.TV
					: (long)AnimeTypeId.AllTypes,
				SeasonId = GetSeasonSummariesQuery.Id
			};
	}
}
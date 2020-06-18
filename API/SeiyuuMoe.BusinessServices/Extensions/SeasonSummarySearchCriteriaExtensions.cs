using SeiyuuMoe.Contracts.Enums;
using SeiyuuMoe.Contracts.SearchCriteria;

namespace SeiyuuMoe.BusinessServices.Extensions
{
	public static class SeasonSummarySearchCriteriaExtensions
	{
		public static RoleSearchCriteria ToRoleSearchCriteria(this SeasonSummarySearchCriteria seasonSummarySearchCriteria)
			=> new RoleSearchCriteria
			{
				AnimeId = seasonSummarySearchCriteria.AnimeId,
				RoleTypeId = seasonSummarySearchCriteria.MainRolesOnly
					? (long)RoleTypeDictionary.Main
					: (long)RoleTypeDictionary.All
			};

		public static AnimeSearchCriteria ToAnimeSearchCriteria(this SeasonSummarySearchCriteria seasonSummarySearchCriteria)
			=> new AnimeSearchCriteria
			{
				AnimeTypeId = seasonSummarySearchCriteria.TVSeriesOnly
					? (long)AnimeTypeDictionary.TV
					: (long)AnimeTypeDictionary.AllTypes,
				SeasonId = seasonSummarySearchCriteria.Id
			};

		public static SeasonSearchCriteria ToSeasonSearchCriteria(this SeasonSummarySearchCriteria seasonSummarySearchCriteria)
			=> new SeasonSearchCriteria
			{
				Name = seasonSummarySearchCriteria.Season,
				Year = seasonSummarySearchCriteria.Year
			};
	}
}
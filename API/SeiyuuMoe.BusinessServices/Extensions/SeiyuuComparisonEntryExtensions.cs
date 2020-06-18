using SeiyuuMoe.Contracts.ComparisonEntities;
using SeiyuuMoe.Contracts.Dtos.Other;
using System.Linq;

namespace SeiyuuMoe.BusinessServices.Extensions
{
	public static class SeiyuuComparisonEntryExtensions
	{
		public static SeiyuuComparisonEntryDto ToSeiyuuComparisonEntryDto(this SeiyuuComparisonEntry seiyuuComparisonEntry)
			=> new SeiyuuComparisonEntryDto(
				seiyuuComparisonEntry.Anime.Select(x => x.ToAnimeTableEntryDto()).ToList(),
				seiyuuComparisonEntry.SeiyuuCharacters.Select(x => x.ToSeiyuuComparisonSubEntryDto()).ToList()
			);
	}
}
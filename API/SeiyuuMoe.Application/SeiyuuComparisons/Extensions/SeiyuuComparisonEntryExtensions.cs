using SeiyuuMoe.Application.Animes.Extensions;
using SeiyuuMoe.Domain.ComparisonEntities;
using System.Collections.Generic;
using System.Linq;

namespace SeiyuuMoe.Application.SeiyuuComparisons.Extensions
{
	public static class SeiyuuComparisonEntryExtensions
	{
		public static SeiyuuComparisonEntryDto ToSeiyuuComparisonEntryDto(this SeiyuuComparisonEntry seiyuuComparisonEntry)
			=> new SeiyuuComparisonEntryDto(
				seiyuuComparisonEntry.Anime?.Select(x => x.ToAnimeTableEntry()).ToList() ?? new List<AnimeTableEntry>(),
				seiyuuComparisonEntry.SeiyuuCharacters?.Select(x => x.ToSeiyuuComparisonSubEntryDto()).ToList() ?? new List<SeiyuuComparisonSubEntryDto>()
			);
	}
}
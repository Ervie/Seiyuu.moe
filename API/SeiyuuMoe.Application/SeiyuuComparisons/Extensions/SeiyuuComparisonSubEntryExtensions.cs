using SeiyuuMoe.Application.Characters.Extensions;
using SeiyuuMoe.Application.Seiyuus.Extensions;
using SeiyuuMoe.Domain.ComparisonEntities;
using System.Collections.Generic;
using System.Linq;

namespace SeiyuuMoe.Application.SeiyuuComparisons.Extensions
{
	public static class SeiyuuComparisonSubEntryExtensions
	{
		public static SeiyuuComparisonSubEntryDto ToSeiyuuComparisonSubEntryDto(this SeiyuuComparisonSubEntry seiyuuComparisonSubEntry)
			=> new SeiyuuComparisonSubEntryDto(
				seiyuuComparisonSubEntry.Seiyuu?.ToSeiyuuTableEntry(),
				seiyuuComparisonSubEntry.Characters?.Select(x => x?.ToCharacterTableEntry()).ToList() ?? new List<CharacterTableEntry>()
			);
	}
}
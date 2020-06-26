using SeiyuuMoe.Application.Seiyuu.Extensions;
using SeiyuuMoe.Application.Character.Extensions;
using SeiyuuMoe.Contracts.Dtos.Other;
using SeiyuuMoe.Domain.ComparisonEntities;
using System.Linq;
using System.Collections.Generic;

namespace SeiyuuMoe.Application.SeiyuuComparison.Extensions
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
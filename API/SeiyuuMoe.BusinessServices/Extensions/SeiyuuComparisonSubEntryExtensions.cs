using SeiyuuMoe.Contracts.ComparisonEntities;
using SeiyuuMoe.Contracts.Dtos.Other;
using System.Linq;

namespace SeiyuuMoe.BusinessServices.Extensions
{
	public static class SeiyuuComparisonSubEntryExtensions
	{
		public static SeiyuuComparisonSubEntryDto ToSeiyuuComparisonSubEntryDto(this SeiyuuComparisonSubEntry seiyuuComparisonSubEntry)
			=> new SeiyuuComparisonSubEntryDto(
				seiyuuComparisonSubEntry.Seiyuu.ToSeiyuuTableEntryDto(),
				seiyuuComparisonSubEntry.Characters.Select(x => x.ToCharacterTableEntryDto()).ToList()
			);
	}
}
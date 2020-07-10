using SeiyuuMoe.Domain.ComparisonEntities;
using System.Collections.Generic;

namespace SeiyuuMoe.Application.SeiyuuComparisons
{
	public class SeiyuuComparisonEntryDto
	{
		public ICollection<AnimeTableEntry> Anime { get; }

		public ICollection<SeiyuuComparisonSubEntryDto> SeiyuuCharacters { get; }

		public SeiyuuComparisonEntryDto(ICollection<AnimeTableEntry> animeTableEntries, ICollection<SeiyuuComparisonSubEntryDto> seiyuuComparisonSubEntries)
		{
			Anime = animeTableEntries;
			SeiyuuCharacters = seiyuuComparisonSubEntries;
		}
	}

	public class SeiyuuComparisonSubEntryDto
	{
		public SeiyuuTableEntry Seiyuu { get; }

		public ICollection<CharacterTableEntry> Characters { get; }

		public SeiyuuComparisonSubEntryDto(SeiyuuTableEntry seiyuuTableEntry, ICollection<CharacterTableEntry> characterTableEntries)
		{
			Seiyuu = seiyuuTableEntry;
			Characters = characterTableEntries;
		}
	}
}
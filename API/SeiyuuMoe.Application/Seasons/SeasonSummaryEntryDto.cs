using SeiyuuMoe.Domain.ComparisonEntities;
using System;
using System.Collections.Generic;

namespace SeiyuuMoe.Application.Seasons
{
	public class SeasonSummaryEntryDto
	{
		public SeiyuuTableEntry Seiyuu { get; }

		public ICollection<Tuple<AnimeTableEntry, CharacterTableEntry>> AnimeCharacterPairs { get; }

		public SeasonSummaryEntryDto(
			SeiyuuTableEntry seiyuuTableEntry,
			ICollection<Tuple<AnimeTableEntry, CharacterTableEntry>> animeCharacterPairs
		)
		{
			Seiyuu = seiyuuTableEntry;
			AnimeCharacterPairs = animeCharacterPairs;
		}
	}
}
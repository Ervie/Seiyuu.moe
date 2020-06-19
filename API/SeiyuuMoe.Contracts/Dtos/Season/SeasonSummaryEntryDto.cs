using System;
using System.Collections.Generic;

namespace SeiyuuMoe.Contracts.Dtos.Season
{
	public class SeasonSummaryEntryDto
	{
		public SeiyuuTableEntryDto Seiyuu { get; }

		public ICollection<Tuple<AnimeTableEntryDto, CharacterTableEntryDto>> AnimeCharacterPairs { get; }

		public SeasonSummaryEntryDto(
			SeiyuuTableEntryDto seiyuuTableEntryDto,
			ICollection<Tuple<AnimeTableEntryDto, CharacterTableEntryDto>> animeCharacterPairs
		)
		{
			Seiyuu = seiyuuTableEntryDto;
			AnimeCharacterPairs = animeCharacterPairs;
		}
	}
}
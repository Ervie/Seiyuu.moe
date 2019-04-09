using System;
using System.Collections.Generic;

namespace SeiyuuMoe.Contracts.Dtos.Season
{
	public class SeasonSummaryEntryDto
	{
		public SeiyuuTableEntryDto Seiyuu { get; set; }

		public ICollection<Tuple<AnimeTableEntryDto, CharacterTableEntryDto>> AnimeCharacterPairs { get; set; }
	}
}
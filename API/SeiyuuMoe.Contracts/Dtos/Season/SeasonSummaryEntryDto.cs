using System;
using System.Collections.Generic;

namespace SeiyuuMoe.Contracts.Dtos.Season
{
	public class SeasonSummaryEntryDto
	{
		public SeiyuuTableEntryDto Seiyuu { get; set; }

		public ICollection<(AnimeTableEntryDto anime, CharacterTableEntryDto character)> AnimeCharacterPairs { get; set; }
	}
}
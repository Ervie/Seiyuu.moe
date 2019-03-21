using System;
using System.Collections.Generic;
using System.Text;

namespace SeiyuuMoe.Contracts.Dtos.Other
{
	public class SeiyuuComparisonEntryDto
	{
		public AnimeTableEntryDto Anime { get; set; }

		public ICollection<SeiyuuComparisonSubEntryDto> SeiyuuCharacters { get; set; }
	}

	public class SeiyuuComparisonSubEntryDto
	{
		public SeiyuuTableEntryDto Seiyuu { get; set; }

		public ICollection<CharacterTableEntryDto> Characters { get; set; }
	}
}

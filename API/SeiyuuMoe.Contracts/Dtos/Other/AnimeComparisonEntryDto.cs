using System;
using System.Collections.Generic;
using System.Text;

namespace SeiyuuMoe.Contracts.Dtos
{
	public class AnimeComparisonEntryDto
	{
		public SeiyuuTableEntryDto Seiyuu { get; set; }

		public ICollection<CharacterAnimePairDto> CharacterAnimePairs { get; set; }
	}

	public class CharacterAnimePairDto
	{
		public CharacterTableEntryDto Character { get; set; }

		public AnimeTableEntryDto Anime { get; set; }
	}
}

using System.Collections.Generic;

namespace SeiyuuMoe.Contracts.Dtos
{
	public class AnimeComparisonEntryDto
	{
		public SeiyuuTableEntryDto Seiyuu { get; set; }

		public ICollection<AnimeComparisonSubEntryDto> CharacterAnimePairs { get; set; }
	}

	public class AnimeComparisonSubEntryDto
	{
		public AnimeTableEntryDto Anime { get; set; }

		public ICollection<CharacterTableEntryDto> Characters { get; set; }
	}
}
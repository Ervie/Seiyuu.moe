using System.Collections.Generic;

namespace SeiyuuMoe.Contracts.Dtos.Other
{
	public class SeiyuuComparisonEntryDto
	{
		public ICollection<AnimeTableEntryDto> Anime { get; }

		public ICollection<SeiyuuComparisonSubEntryDto> SeiyuuCharacters { get; }

		public SeiyuuComparisonEntryDto(ICollection<AnimeTableEntryDto> animeTableEntryDtos, ICollection<SeiyuuComparisonSubEntryDto> seiyuuComparisonSubEntryDtos)
		{
			Anime = animeTableEntryDtos;
			SeiyuuCharacters = seiyuuComparisonSubEntryDtos;
		}
	}

	public class SeiyuuComparisonSubEntryDto
	{
		public SeiyuuTableEntryDto Seiyuu { get; }

		public ICollection<CharacterTableEntryDto> Characters { get; }

		public SeiyuuComparisonSubEntryDto(SeiyuuTableEntryDto seiyuuTableEntryDto, ICollection<CharacterTableEntryDto> characterTableEntryDtos)
		{
			Seiyuu = seiyuuTableEntryDto;
			Characters = characterTableEntryDtos;
		}
	}
}
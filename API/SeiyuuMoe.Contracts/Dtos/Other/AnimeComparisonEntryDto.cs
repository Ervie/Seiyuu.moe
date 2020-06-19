using System.Collections.Generic;

namespace SeiyuuMoe.Contracts.Dtos
{
	public class AnimeComparisonEntryDto
	{
		public SeiyuuTableEntryDto Seiyuu { get; }

		public ICollection<AnimeComparisonSubEntryDto> AnimeCharacters { get; }

		public AnimeComparisonEntryDto(SeiyuuTableEntryDto seiyuuTableEntryDto, ICollection<AnimeComparisonSubEntryDto> animeComparisonSubEntryDtos)
		{
			Seiyuu = seiyuuTableEntryDto;
			AnimeCharacters = animeComparisonSubEntryDtos;
		}
	}

	public class AnimeComparisonSubEntryDto
	{
		public AnimeTableEntryDto Anime { get; }

		public ICollection<CharacterTableEntryDto> Characters { get; }

		public AnimeComparisonSubEntryDto(AnimeTableEntryDto animeTableEntryDto, ICollection<CharacterTableEntryDto> characterTableEntryDtos)
		{
			Anime = animeTableEntryDto;
			Characters = characterTableEntryDtos;
		}
	}
}
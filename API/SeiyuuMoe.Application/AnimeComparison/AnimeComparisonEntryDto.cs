using SeiyuuMoe.Domain.ComparisonEntities;
using System.Collections.Generic;

namespace SeiyuuMoe.AnimeComparison
{
	public class AnimeComparisonEntryDto
	{
		public SeiyuuTableEntry Seiyuu { get; }

		public ICollection<AnimeComparisonSubEntryDto> AnimeCharacters { get; }

		public AnimeComparisonEntryDto(SeiyuuTableEntry seiyuuTableEntryDto, ICollection<AnimeComparisonSubEntryDto> animeComparisonSubEntryDtos)
		{
			Seiyuu = seiyuuTableEntryDto;
			AnimeCharacters = animeComparisonSubEntryDtos;
		}
	}

	public class AnimeComparisonSubEntryDto
	{
		public AnimeTableEntry Anime { get; }

		public ICollection<CharacterTableEntry> Characters { get; }

		public AnimeComparisonSubEntryDto(AnimeTableEntry animeTableEntryDto, ICollection<CharacterTableEntry> characterTableEntryDtos)
		{
			Anime = animeTableEntryDto;
			Characters = characterTableEntryDtos;
		}
	}
}
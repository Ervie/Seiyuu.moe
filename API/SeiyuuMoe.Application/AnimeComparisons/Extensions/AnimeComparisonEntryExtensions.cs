using SeiyuuMoe.AnimeComparisons;
using SeiyuuMoe.Application.Seiyuus.Extensions;
using SeiyuuMoe.Domain.ComparisonEntities;
using System.Collections.Generic;
using System.Linq;

namespace SeiyuuMoe.Application.AnimeComparisons.Extensions
{
	public static class AnimeComparisonEntryExtensions
	{
		public static AnimeComparisonEntryDto ToAnimeComparisonEntryDto(this AnimeComparisonEntry animeComparisonEntryDto)
			=> new AnimeComparisonEntryDto(
				animeComparisonEntryDto.Seiyuu?.ToSeiyuuTableEntry(),
				animeComparisonEntryDto.AnimeCharacters?.Select(x => x.ToAnimeComparisonSubEntryDto()).ToList() ?? new List<AnimeComparisonSubEntryDto>()
			);
	}
}
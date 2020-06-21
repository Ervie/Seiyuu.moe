using SeiyuuMoe.AnimeComparison;
using SeiyuuMoe.Application.Seiyuu.Extensions;
using SeiyuuMoe.Domain.ComparisonEntities;
using System.Linq;

namespace SeiyuuMoe.Application.AnimeComparison.Extensions
{
	public static class AnimeComparisonEntryExtensions
	{
		public static AnimeComparisonEntryDto ToAnimeComparisonEntryDto(this AnimeComparisonEntry animeComparisonEntryDto)
			=> new AnimeComparisonEntryDto(
				animeComparisonEntryDto.Seiyuu.ToSeiyuuTableEntry(),
				animeComparisonEntryDto.AnimeCharacters.Select(x => x.ToAnimeComparisonSubEntryDto()).ToList()
			);
	}
}
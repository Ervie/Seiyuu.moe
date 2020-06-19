using SeiyuuMoe.Contracts.ComparisonEntities;
using SeiyuuMoe.Contracts.Dtos;
using System.Linq;

namespace SeiyuuMoe.BusinessServices.Extensions
{
	public static class AnimeComparisonEntryExtensions
	{
		public static AnimeComparisonEntryDto ToAnimeComparisonEntryDto(this AnimeComparisonEntry animeComparisonEntryDto)
			=> new AnimeComparisonEntryDto(
				animeComparisonEntryDto.Seiyuu.ToSeiyuuTableEntryDto(),
				animeComparisonEntryDto.AnimeCharacters.Select(x => x.ToAnimeComparisonSubEntryDto()).ToList()
			);
	}
}
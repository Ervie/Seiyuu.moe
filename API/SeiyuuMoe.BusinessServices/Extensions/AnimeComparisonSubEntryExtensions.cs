using SeiyuuMoe.Contracts.ComparisonEntities;
using SeiyuuMoe.Contracts.Dtos;
using System.Linq;

namespace SeiyuuMoe.BusinessServices.Extensions
{
	public static class AnimeComparisonSubEntryExtensions
	{
		public static AnimeComparisonSubEntryDto ToAnimeComparisonSubEntryDto(this AnimeComparisonSubEntry animeComparisonSubEntry)
			=> new AnimeComparisonSubEntryDto(
				animeComparisonSubEntry.Anime.ToAnimeTableEntryDto(),
				animeComparisonSubEntry.Characters.Select(x => x.ToCharacterTableEntryDto()).ToList()
			);
	}
}
using SeiyuuMoe.AnimeComparison;
using SeiyuuMoe.Application.Anime.Extensions;
using SeiyuuMoe.Application.Character.Extensions;
using SeiyuuMoe.Domain.ComparisonEntities;
using System.Linq;

namespace SeiyuuMoe.Application.AnimeComparison.Extensions
{
	public static class AnimeComparisonSubEntryExtensions
	{
		public static AnimeComparisonSubEntryDto ToAnimeComparisonSubEntryDto(this AnimeComparisonSubEntry animeComparisonSubEntry)
			=> new AnimeComparisonSubEntryDto(
				animeComparisonSubEntry.Anime.ToAnimeTableEntry(),
				animeComparisonSubEntry.Characters.Select(x => x.ToCharacterTableEntry()).ToList()
			);
	}
}
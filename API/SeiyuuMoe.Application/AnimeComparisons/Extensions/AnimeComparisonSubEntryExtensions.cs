using SeiyuuMoe.AnimeComparisons;
using SeiyuuMoe.Application.Animes.Extensions;
using SeiyuuMoe.Application.Characters.Extensions;
using SeiyuuMoe.Domain.ComparisonEntities;
using System.Collections.Generic;
using System.Linq;

namespace SeiyuuMoe.Application.AnimeComparisons.Extensions
{
	public static class AnimeComparisonSubEntryExtensions
	{
		public static AnimeComparisonSubEntryDto ToAnimeComparisonSubEntryDto(this AnimeComparisonSubEntry animeComparisonSubEntry)
			=> new AnimeComparisonSubEntryDto(
				animeComparisonSubEntry.Anime?.ToAnimeTableEntry(),
				animeComparisonSubEntry.Characters?.Select(x => x.ToCharacterTableEntry()).ToList() ?? new List<CharacterTableEntry>()
			);
	}
}
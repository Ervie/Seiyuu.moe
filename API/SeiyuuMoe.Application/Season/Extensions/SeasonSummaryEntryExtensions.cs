using SeiyuuMoe.Application.Anime.Extensions;
using SeiyuuMoe.Application.Character.Extensions;
using SeiyuuMoe.Application.Season;
using SeiyuuMoe.Application.Seiyuu.Extensions;
using SeiyuuMoe.Domain.ComparisonEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SeiyuuMoe.BusinessServices.Extensions
{
	public static class SeasonSummaryEntryExtensions
	{
		public static SeasonSummaryEntryDto ToSeasonSummaryEntryDto(this SeasonSummaryEntry seasonSummaryEntry)
			=> new SeasonSummaryEntryDto(
				seasonSummaryEntry.Seiyuu?.ToSeiyuuTableEntry(),
				seasonSummaryEntry.AnimeCharacterPairs?.Select(
					x => new Tuple<AnimeTableEntry, CharacterTableEntry>(
						x.anime?.ToAnimeTableEntry(),
						x.character?.ToCharacterTableEntry()
					)
				)
				.ToList() ?? new List<Tuple<AnimeTableEntry, CharacterTableEntry>>()
			);
	}
}
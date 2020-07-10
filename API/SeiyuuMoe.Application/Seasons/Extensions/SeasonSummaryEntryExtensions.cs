using SeiyuuMoe.Application.Animes.Extensions;
using SeiyuuMoe.Application.Characters.Extensions;
using SeiyuuMoe.Application.Seiyuus.Extensions;
using SeiyuuMoe.Domain.ComparisonEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SeiyuuMoe.Application.Seasons.Extensions
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
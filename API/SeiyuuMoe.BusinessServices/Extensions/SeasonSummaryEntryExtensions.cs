using SeiyuuMoe.Contracts.ComparisonEntities;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.Dtos.Season;
using System;
using System.Linq;

namespace SeiyuuMoe.BusinessServices.Extensions
{
	public static class SeasonSummaryEntryExtensions
	{
		public static SeasonSummaryEntryDto ToSeasonSummaryEntryDto(this SeasonSummaryEntry seasonSummaryEntry)
			=> new SeasonSummaryEntryDto(
				seasonSummaryEntry.Seiyuu.ToSeiyuuTableEntryDto(),
				seasonSummaryEntry.AnimeCharacterPairs.Select(
					x => new Tuple<AnimeTableEntryDto, CharacterTableEntryDto>(
						x.anime.ToAnimeTableEntryDto(),
						x.character.ToCharacterTableEntryDto()
					)
				)
				.ToList()
			);
	}
}
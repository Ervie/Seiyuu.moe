using SeiyuuMoe.Domain.Entities;
using System.Collections.Generic;

namespace SeiyuuMoe.Domain.ComparisonEntities
{
	public class SeasonSummaryEntry
	{
		public Seiyuu Seiyuu { get; set; }

		public ICollection<(Anime anime, AnimeCharacter character)> AnimeCharacterPairs { get; set; }

		public SeasonSummaryEntry()
		{
			AnimeCharacterPairs = new List<(Anime anime, AnimeCharacter character)>();
		}

		public SeasonSummaryEntry(Seiyuu seiyuu, Anime anime, AnimeCharacter character)
		{
			Seiyuu = seiyuu;
			AnimeCharacterPairs = new List<(Anime anime, AnimeCharacter character)>()
			{
				(anime, character)
			};
		}

		public decimal GetTotalSignificanceValue()
		{
			decimal total = 0m;
			foreach (var (anime, character) in AnimeCharacterPairs)
			{
				decimal animePopularityFactor = anime.Popularity.Value / 1000m;
				decimal characterPopularityFactor = character.Popularity.Value != 0 ? character.Popularity.Value : 1;
				total += animePopularityFactor * characterPopularityFactor;
			}
			return total;
		}
	}
}
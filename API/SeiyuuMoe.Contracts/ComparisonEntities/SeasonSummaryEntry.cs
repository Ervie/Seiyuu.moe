using SeiyuuMoe.Data.Model;
using System;
using System.Collections.Generic;

namespace SeiyuuMoe.Contracts.ComparisonEntities
{
	public class SeasonSummaryEntry
	{
		public Seiyuu Seiyuu { get; set; }

		public ICollection<(Anime anime, Character character)> AnimeCharacterPairs { get; set; }

		public SeasonSummaryEntry()
		{
			AnimeCharacterPairs = new List<(Anime anime, Character character)>();
		}

		public SeasonSummaryEntry(Seiyuu seiyuu, Anime anime, Character character)
		{
			Seiyuu = seiyuu;
			AnimeCharacterPairs = new List<(Anime anime, Character character)>()
			{
				(anime, character)
			};
		}

		public decimal TotalSignificanceValue
		{
			get
			{
				decimal total = 0m;
				foreach (var animeCharacterPair in AnimeCharacterPairs)
				{
					decimal animePopularityFactor = animeCharacterPair.anime.Popularity.Value / 1000m;
					decimal characterPopularityFactor = animeCharacterPair.character.Popularity.Value != 0 ? animeCharacterPair.character.Popularity.Value : 1;
					total += animePopularityFactor * characterPopularityFactor;
				}
				return total;
			}
		}
	}
}
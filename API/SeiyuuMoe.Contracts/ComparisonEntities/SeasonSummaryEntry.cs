using SeiyuuMoe.Data.Model;
using System;
using System.Collections.Generic;

namespace SeiyuuMoe.Contracts.ComparisonEntities
{
	public class SeasonSummaryEntry
	{
		public Seiyuu Seiyuu { get; set; }

		public ICollection<Tuple<Anime, Character>> AnimeCharacterPairs { get; set; }

		public SeasonSummaryEntry()
		{
			AnimeCharacterPairs = new List<Tuple<Anime, Character>>();
		}

		public SeasonSummaryEntry(Seiyuu seiyuu, Anime anime, Character character)
		{
			Seiyuu = seiyuu;
			AnimeCharacterPairs = new List<Tuple<Anime, Character>>()
			{
				new Tuple<Anime, Character>(anime, character)
			};
		}
	}
}
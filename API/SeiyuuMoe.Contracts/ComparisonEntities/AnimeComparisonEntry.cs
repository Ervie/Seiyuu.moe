using SeiyuuMoe.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeiyuuMoe.Contracts.ComparisonEntities
{
	public class AnimeComparisonEntry
	{
		public Seiyuu Seiyuu { get; set; }

		public ICollection<AnimeComparisonSubEntry> AnimeCharacters { get; set; }

		public AnimeComparisonEntry()
		{
			AnimeCharacters = new List<AnimeComparisonSubEntry>();
		}
	}

	public class AnimeComparisonSubEntry
	{
		public Anime Anime { get; set; }

		public ICollection<Character> Characters { get; set; }

		public AnimeComparisonSubEntry(Character character, Anime anime)
		{
			Anime = anime;
			Characters = new List<Character>
			{
				character
			};
		}
	}
}

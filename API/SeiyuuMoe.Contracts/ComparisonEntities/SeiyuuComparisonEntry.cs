using SeiyuuMoe.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeiyuuMoe.Contracts.ComparisonEntities
{
	public class SeiyuuComparisonEntry
	{
		public Anime Anime { get; set; }

		public ICollection<SeiyuuComparisonSubEntry> SeiyuuCharacters { get; set; }

		public SeiyuuComparisonEntry()
		{
			SeiyuuCharacters = new List<SeiyuuComparisonSubEntry>();
		}
	}

	public class SeiyuuComparisonSubEntry
	{
		public Seiyuu Seiyuu { get; set; }

		public ICollection<Character> Characters { get; set; }

		public SeiyuuComparisonSubEntry(Character character, Seiyuu seiyuu)
		{
			Seiyuu = seiyuu;
			Characters = new List<Character>
			{
				character
			};
		}
	}
}

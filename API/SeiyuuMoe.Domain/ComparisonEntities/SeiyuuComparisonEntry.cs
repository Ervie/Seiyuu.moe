using SeiyuuMoe.Domain.Entities;
using System.Collections.Generic;

namespace SeiyuuMoe.Domain.ComparisonEntities
{
	public class SeiyuuComparisonEntry
	{
		public ICollection<Anime> Anime { get; set; }

		public ICollection<SeiyuuComparisonSubEntry> SeiyuuCharacters { get; set; }

		public SeiyuuComparisonEntry()
		{
			Anime = new List<Anime>();
			SeiyuuCharacters = new List<SeiyuuComparisonSubEntry>();
		}

		public SeiyuuComparisonEntry(Anime anime, Character character, Seiyuu seiyuu)
		{
			Anime = new List<Anime>
			{
				anime
			};
			SeiyuuCharacters = new List<SeiyuuComparisonSubEntry>
			{
				new SeiyuuComparisonSubEntry(character, seiyuu)
			};
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
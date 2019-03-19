using SeiyuuMoe.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeiyuuMoe.Contracts.ComparisonEntities
{
	public class AnimeComparisonEntry
	{
		public Seiyuu Seiyuu { get; set; }

		public ICollection<CharacterAnimePair> CharacterAnimePairs { get; set; }

		public AnimeComparisonEntry()
		{
			CharacterAnimePairs = new List<CharacterAnimePair>();
		}
	}

	public class CharacterAnimePair
	{
		public Character Character { get; set; }

		public Anime Anime { get; set; }

		public CharacterAnimePair(Character character, Anime anime)
		{
			Character = character;
			Anime = anime;
		}
	}
}

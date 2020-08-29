using SeiyuuMoe.Domain.ComparisonEntities;
using SeiyuuMoe.Domain.Entities;
using System.Collections.Generic;

namespace SeiyuuMoe.Tests.Common.Builders.ComparisonEntities
{
	public class SeiyuuComparisonEntryBuilder
	{
		private ICollection<Anime> _anime;
		private ICollection<SeiyuuComparisonSubEntry> _seiyuuComparisonSubEntries;

		public SeiyuuComparisonEntry Build() => new SeiyuuComparisonEntry
		{
			Anime = _anime,
			SeiyuuCharacters = _seiyuuComparisonSubEntries
		};

		public SeiyuuComparisonEntryBuilder WithAnime(ICollection<Anime> anime)
		{
			_anime = anime;
			return this;
		}

		public SeiyuuComparisonEntryBuilder WithSubentries(ICollection<SeiyuuComparisonSubEntry> seiyuuComparisonSubEntries)
		{
			_seiyuuComparisonSubEntries = seiyuuComparisonSubEntries;
			return this;
		}
	}
}
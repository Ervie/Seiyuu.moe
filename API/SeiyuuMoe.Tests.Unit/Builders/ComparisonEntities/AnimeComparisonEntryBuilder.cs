using SeiyuuMoe.Domain.ComparisonEntities;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Tests.Unit.Builders.Model;
using System;

namespace SeiyuuMoe.Tests.Unit.Builders.ComparisonEntities
{
	public class AnimeComparisonEntryBuilder
	{
		private Seiyuu _seiyuu;

		private SeiyuuBuilder _seiyuuBuilder;

		public AnimeComparisonEntry Build() => new AnimeComparisonEntry
		{
			Seiyuu = _seiyuuBuilder.Build() ?? _seiyuu
		};

		public AnimeComparisonEntryBuilder WithSeiyuu(Seiyuu seiyuu)
		{
			_seiyuu = seiyuu;
			return this;
		}

		public AnimeComparisonEntryBuilder WithSeiyuu(Action<SeiyuuBuilder> builderAction)
		{
			_seiyuuBuilder = new SeiyuuBuilder();
			builderAction(_seiyuuBuilder);
			return this;
		}
	}
}
using SeiyuuMoe.Domain.ComparisonEntities;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Tests.Unit.Builders.Model;
using System;

namespace SeiyuuMoe.Tests.Unit.Builders.ComparisonEntities
{
	internal class SeiyuuComparisonSubEntryBuilder
	{
		private Seiyuu _seiyuu;
		private AnimeCharacter _character;

		private SeiyuuBuilder _seiyuuBuilder;
		private CharacterBuilder _characterBuilder;

		public SeiyuuComparisonSubEntry Build()
			=> new SeiyuuComparisonSubEntry
			(
				_characterBuilder?.Build() ?? _character,
				_seiyuuBuilder?.Build() ?? _seiyuu
			);

		public SeiyuuComparisonSubEntryBuilder WithSeiyuu(Seiyuu seiyuu)
		{
			_seiyuu = seiyuu;
			return this;
		}

		public SeiyuuComparisonSubEntryBuilder WithSeiyuu(Action<SeiyuuBuilder> builderAction)
		{
			_seiyuuBuilder = new SeiyuuBuilder();
			builderAction(_seiyuuBuilder);
			return this;
		}

		public SeiyuuComparisonSubEntryBuilder WithCharacter(AnimeCharacter character)
		{
			_character = character;
			return this;
		}

		public SeiyuuComparisonSubEntryBuilder WithCharacter(Action<CharacterBuilder> builderAction)
		{
			_characterBuilder = new CharacterBuilder();
			builderAction(_characterBuilder);
			return this;
		}
	}
}
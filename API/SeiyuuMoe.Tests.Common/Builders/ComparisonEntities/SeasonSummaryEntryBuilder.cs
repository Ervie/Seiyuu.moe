using SeiyuuMoe.Domain.ComparisonEntities;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Tests.Common.Builders.Model;
using System;

namespace SeiyuuMoe.Tests.Common.Builders.ComparisonEntities
{
	public class SeasonSummaryEntryBuilder
	{
		private Seiyuu _seiyuu;
		private Anime _anime;
		private AnimeCharacter _character;

		private SeiyuuBuilder _seiyuuBuilder;
		private AnimeBuilder _animeBuilder;
		private CharacterBuilder _characterBuilder;

		public SeasonSummaryEntry Build()
			=> new SeasonSummaryEntry(
				_seiyuuBuilder?.Build() ?? _seiyuu,
				_animeBuilder?.Build() ?? _anime,
				_characterBuilder?.Build() ?? _character
			);

		public SeasonSummaryEntryBuilder WithSeiyuu(Seiyuu seiyuu)
		{
			_seiyuu = seiyuu;
			return this;
		}

		public SeasonSummaryEntryBuilder WithSeiyuu(Action<SeiyuuBuilder> builderAction)
		{
			_seiyuuBuilder = new SeiyuuBuilder();
			builderAction(_seiyuuBuilder);
			return this;
		}

		public SeasonSummaryEntryBuilder WithAnime(Anime anime)
		{
			_anime = anime;
			return this;
		}

		public SeasonSummaryEntryBuilder WithAnime(Action<AnimeBuilder> builderAction)
		{
			_animeBuilder = new AnimeBuilder();
			builderAction(_animeBuilder);
			return this;
		}

		public SeasonSummaryEntryBuilder WithCharacter(AnimeCharacter character)
		{
			_character = character;
			return this;
		}

		public SeasonSummaryEntryBuilder WithCharacter(Action<CharacterBuilder> builderAction)
		{
			_characterBuilder = new CharacterBuilder();
			builderAction(_characterBuilder);
			return this;
		}
	}
}
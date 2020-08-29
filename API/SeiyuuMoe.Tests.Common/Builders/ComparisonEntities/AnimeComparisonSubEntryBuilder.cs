using SeiyuuMoe.Domain.ComparisonEntities;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Tests.Common.Builders.Model;
using System;
using System.Collections.Generic;

namespace SeiyuuMoe.Tests.Common.Builders.ComparisonEntities
{
	public class AnimeComparisonSubEntryBuilder
	{
		private Anime _anime;
		private ICollection<AnimeCharacter> _characters;

		private AnimeBuilder _animeBuilder;

		public AnimeComparisonSubEntry Build()
			=> new AnimeComparisonSubEntry
			{
				Anime = _animeBuilder?.Build() ?? _anime,
				Characters = _characters
			};

		public AnimeComparisonSubEntryBuilder WithAnime(Anime anime)
		{
			_anime = anime;
			return this;
		}

		public AnimeComparisonSubEntryBuilder WithAnime(Action<AnimeBuilder> builderAction)
		{
			_animeBuilder = new AnimeBuilder();
			builderAction(_animeBuilder);
			return this;
		}

		public AnimeComparisonSubEntryBuilder WithCharacters(ICollection<AnimeCharacter> characters)
		{
			_characters = characters;
			return this;
		}
	}
}
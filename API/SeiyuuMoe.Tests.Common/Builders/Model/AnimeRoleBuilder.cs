using SeiyuuMoe.Domain.Entities;
using System;

namespace SeiyuuMoe.Tests.Common.Builders.Model
{
	public class AnimeRoleBuilder
	{
		private Seiyuu _seiyuu;
		private Anime _anime;
		private AnimeCharacter _character;
		private Language _language;
		private AnimeRoleType _roleType;

		private SeiyuuBuilder _seiyuuBuilder;
		private AnimeBuilder _animeBuilder;
		private CharacterBuilder _characterBuilder;
		private LanguageBuilder _languageBuilder;
		private AnimeRoleTypeBuilder _roleTypeBuilder;

		public AnimeRole Build()
			=> new AnimeRole
			{
				Character = _characterBuilder?.Build() ?? _character,
				Anime = _animeBuilder?.Build() ?? _anime,
				Seiyuu = _seiyuuBuilder?.Build() ?? _seiyuu,
				Language = _languageBuilder?.Build() ?? _language,
				RoleType = _roleTypeBuilder?.Build() ?? _roleType,
			};

		public AnimeRoleBuilder WithSeiyuu(Seiyuu seiyuu)
		{
			_seiyuu = seiyuu;
			return this;
		}

		public AnimeRoleBuilder WithSeiyuu(Action<SeiyuuBuilder> builderAction)
		{
			_seiyuuBuilder = new SeiyuuBuilder();
			builderAction(_seiyuuBuilder);
			return this;
		}

		public AnimeRoleBuilder WithAnime(Anime anime)
		{
			_anime = anime;
			return this;
		}

		public AnimeRoleBuilder WithAnime(Action<AnimeBuilder> builderAction)
		{
			_animeBuilder = new AnimeBuilder();
			builderAction(_animeBuilder);
			return this;
		}

		public AnimeRoleBuilder WithCharacter(AnimeCharacter character)
		{
			_character = character;
			return this;
		}

		public AnimeRoleBuilder WithCharacter(Action<CharacterBuilder> builderAction)
		{
			_characterBuilder = new CharacterBuilder();
			builderAction(_characterBuilder);
			return this;
		}

		public AnimeRoleBuilder WithLanguage(Language language)
		{
			_language = language;
			return this;
		}

		public AnimeRoleBuilder WithLanguage(Action<LanguageBuilder> builderAction)
		{
			_languageBuilder = new LanguageBuilder();
			builderAction(_languageBuilder);
			return this;
		}

		public AnimeRoleBuilder WithRoleType(AnimeRoleType roleType)
		{
			_roleType = roleType;
			return this;
		}

		public AnimeRoleBuilder WithRoleType(Action<AnimeRoleTypeBuilder> builderAction)
		{
			_roleTypeBuilder = new AnimeRoleTypeBuilder();
			builderAction(_roleTypeBuilder);
			return this;
		}
	}
}
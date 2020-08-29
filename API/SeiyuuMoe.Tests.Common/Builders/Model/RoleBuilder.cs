using SeiyuuMoe.Domain.Entities;
using System;

namespace SeiyuuMoe.Tests.Common.Builders.Model
{
	public class RoleBuilder
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
		private RoleTypeBuilder _roleTypeBuilder;

		public AnimeRole Build()
			=> new AnimeRole
			{
				Character = _characterBuilder?.Build() ?? _character,
				Anime = _animeBuilder?.Build() ?? _anime,
				Seiyuu = _seiyuuBuilder?.Build() ?? _seiyuu,
				Language = _languageBuilder?.Build() ?? _language,
				RoleType = _roleTypeBuilder?.Build() ?? _roleType,
			};

		public RoleBuilder WithSeiyuu(Seiyuu seiyuu)
		{
			_seiyuu = seiyuu;
			return this;
		}

		public RoleBuilder WithSeiyuu(Action<SeiyuuBuilder> builderAction)
		{
			_seiyuuBuilder = new SeiyuuBuilder();
			builderAction(_seiyuuBuilder);
			return this;
		}

		public RoleBuilder WithAnime(Anime anime)
		{
			_anime = anime;
			return this;
		}

		public RoleBuilder WithAnime(Action<AnimeBuilder> builderAction)
		{
			_animeBuilder = new AnimeBuilder();
			builderAction(_animeBuilder);
			return this;
		}

		public RoleBuilder WithCharacter(AnimeCharacter character)
		{
			_character = character;
			return this;
		}

		public RoleBuilder WithCharacter(Action<CharacterBuilder> builderAction)
		{
			_characterBuilder = new CharacterBuilder();
			builderAction(_characterBuilder);
			return this;
		}

		public RoleBuilder WithLanguage(Language language)
		{
			_language = language;
			return this;
		}

		public RoleBuilder WithLanguage(Action<LanguageBuilder> builderAction)
		{
			_languageBuilder = new LanguageBuilder();
			builderAction(_languageBuilder);
			return this;
		}

		public RoleBuilder WithRoleType(AnimeRoleType roleType)
		{
			_roleType = roleType;
			return this;
		}

		public RoleBuilder WithRoleType(Action<RoleTypeBuilder> builderAction)
		{
			_roleTypeBuilder = new RoleTypeBuilder();
			builderAction(_roleTypeBuilder);
			return this;
		}
	}
}
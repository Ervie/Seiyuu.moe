using SeiyuuMoe.Domain.Entities;
using System;

namespace SeiyuuMoe.Tests.Common.Builders.Model
{
	public class CharacterBuilder
	{
		private string _name = string.Empty;
		private string _imageUrl = string.Empty;
		private string _nameKanji = string.Empty;
		private string _about = string.Empty;
		private string _nicknames = string.Empty;
		private long _malId;
		private int _popularity;
		private Guid _id;

		public AnimeCharacter Build() => new AnimeCharacter
		{
			Id = _id,
			Name = _name,
			MalId = _malId,
			ImageUrl = _imageUrl,
			KanjiName = _nameKanji,
			Nicknames = _nicknames,
			Popularity = _popularity,
			About = _about
		};

		public CharacterBuilder WithId(Guid id)
		{
			_id = id;
			return this;
		}

		public CharacterBuilder WithName(string name)
		{
			_name = name;
			return this;
		}

		public CharacterBuilder WithImageUrl(string imageUrl)
		{
			_imageUrl = imageUrl;
			return this;
		}

		public CharacterBuilder WithMalId(long malId)
		{
			_malId = malId;
			return this;
		}

		public CharacterBuilder WithPopularity(int popularity)
		{
			_popularity = popularity;
			return this;
		}

		public CharacterBuilder WithKanjiName(string kanjiName)
		{
			_nameKanji = kanjiName;
			return this;
		}

		public CharacterBuilder WithAbout(string about)
		{
			_about = about;
			return this;
		}

		public CharacterBuilder WithNicknames(string nicknames)
		{
			_nicknames = nicknames;
			return this;
		}
	}
}
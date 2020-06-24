using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Tests.Unit.Builders.Model
{
	internal class CharacterBuilder
	{
		private string _name = string.Empty;
		private string _imageUrl = string.Empty;
		private string _birthday = string.Empty;
		private string _nameKanji = string.Empty;
		private string _about = string.Empty;
		private string _nicknames = string.Empty;
		private long _malId;
		private int _popularity;

		public Character Build() => new Character
		{
			Name = _name,
			MalId = _malId,
			ImageUrl = _imageUrl,
			NameKanji = _nameKanji,
			Nicknames = _nicknames,
			Popularity = _popularity,
			About = _about
		};

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
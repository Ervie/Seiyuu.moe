using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Tests.Unit.Builders.Model
{
	public class SeiyuuBuilder
	{
		private string _name = string.Empty;
		private string _imageUrl = string.Empty;
		private string _birthday = string.Empty;
		private string _japaneseName = string.Empty;
		private string _about = string.Empty;
		private long _malId;
		private int _popularity;

		public Seiyuu Build() => new Seiyuu
		{
			Name = _name,
			ImageUrl = _imageUrl,
			MalId = _malId,
			Birthday = _birthday,
			Popularity = _popularity,
			JapaneseName = _japaneseName,
			About = _about
		};

		public SeiyuuBuilder WithName(string name)
		{
			_name = name;
			return this;
		}

		public SeiyuuBuilder WithImageUrl(string imageUrl)
		{
			_imageUrl = imageUrl;
			return this;
		}

		public SeiyuuBuilder WithMalId(long malId)
		{
			_malId = malId;
			return this;
		}

		public SeiyuuBuilder WithBirthday(string birthday)
		{
			_birthday = birthday;
			return this;
		}

		public SeiyuuBuilder WithPopularity(int popularity)
		{
			_popularity = popularity;
			return this;
		}

		public SeiyuuBuilder WithJapaneseName(string japaneseName)
		{
			_japaneseName = japaneseName;
			return this;
		}

		public SeiyuuBuilder WithAbout(string about)
		{
			_about = about;
			return this;
		}
	}
}
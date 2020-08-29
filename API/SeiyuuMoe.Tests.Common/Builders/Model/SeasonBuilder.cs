using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Tests.Common.Builders.Model
{
	public class SeasonBuilder
	{
		private string _name;
		private int _year;
		private long _id;

		public AnimeSeason Build() => new AnimeSeason
		{
			Name = _name,
			Year = _year,
			Id = _id
		};

		public SeasonBuilder WithName(string name)

		{
			_name = name;
			return this;
		}

		public SeasonBuilder WithYear(int year)

		{
			_year = year;
			return this;
		}

		public SeasonBuilder WithId(long id)
		{
			_id = id;
			return this;
		}
	}
}
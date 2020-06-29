using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Tests.Unit.Builders.Model
{
	public class SeasonBuilder
	{
		private string _name;
		private int _year;

		public Season Build() => new Season
		{
			Name = _name,
			Year = _year
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
	}
}
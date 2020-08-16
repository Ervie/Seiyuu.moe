using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Tests.Unit.Builders.Model
{
	public class AnimeStatusBuilder
	{
		private string _name = string.Empty;

		public AnimeStatus Build() => new AnimeStatus { Description = _name };

		public AnimeStatusBuilder WithName(string name)
		{
			_name = name;
			return this;
		}
	}
}
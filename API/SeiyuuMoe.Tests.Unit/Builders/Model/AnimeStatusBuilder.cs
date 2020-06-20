using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Tests.Unit.Builders.Model
{
	internal class AnimeStatusBuilder
	{
		private string _name = string.Empty;

		public AnimeStatus Build() => new AnimeStatus { Name = _name };

		public AnimeStatusBuilder WithName(string name)
		{
			_name = name;
			return this;
		}
	}
}
using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Tests.Unit.Builders.Model
{
	public class AnimeTypeBuilder
	{
		private string _name = string.Empty;

		public AnimeType Build() => new AnimeType { Description = _name };

		public AnimeTypeBuilder WithName(string name)
		{
			_name = name;
			return this;
		}
	}
}
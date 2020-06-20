using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Tests.Unit.Builders.Model
{
	internal class AnimeTypeBuilder
	{
		private string _name = string.Empty;

		public AnimeType Build() => new AnimeType { Name = _name };

		public AnimeTypeBuilder WithName(string name)
		{
			_name = name;
			return this;
		}
	}
}
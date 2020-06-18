using SeiyuuMoe.Data.Model;

namespace SeiyuuMoe.Tests.Unit.Builders.Model
{
	class AnimeTypeBuilder
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

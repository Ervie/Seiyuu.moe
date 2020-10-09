using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Tests.Common.Builders.Model
{
	public class AnimeTypeBuilder
	{
		private AnimeTypeId _id;

		public AnimeType Build() => new AnimeType { Id = _id };

		public AnimeTypeBuilder WithId(AnimeTypeId id)
		{
			_id = id;
			return this;
		}
	}
}
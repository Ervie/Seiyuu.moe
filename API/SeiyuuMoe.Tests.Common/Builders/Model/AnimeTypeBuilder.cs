using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Tests.Common.Builders.Model
{
	public class AnimeTypeBuilder
	{
		private AnimeTypeId _id;
		private string _description = "TV";

		public AnimeType Build() => new AnimeType
		{
			Id = _id,
			Description = _description
		};

		public AnimeTypeBuilder WithId(AnimeTypeId id)
		{
			_id = id;
			return this;
		}

		public AnimeTypeBuilder WithDescription(string description)
		{
			_description = description;
			return this;
		}
	}
}
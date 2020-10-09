using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Tests.Common.Builders.Model
{
	public class AnimeRoleTypeBuilder
	{
		private string _description;
		private AnimeRoleTypeId _id;

		public AnimeRoleType Build() => new AnimeRoleType
		{
			Description = _description,
			Id = _id
		};

		public AnimeRoleTypeBuilder WithDescription(string description)
		{
			_description = description;
			return this;
		}

		public AnimeRoleTypeBuilder WithId(AnimeRoleTypeId id)
		{
			_id = id;
			return this;
		}
	}
}
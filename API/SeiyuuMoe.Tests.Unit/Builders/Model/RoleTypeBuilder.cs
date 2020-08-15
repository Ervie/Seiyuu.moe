using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Tests.Unit.Builders.Model
{
	public class RoleTypeBuilder
	{
		private string _description;
		private long _id;

		public AnimeRoleType Build() => new AnimeRoleType
		{
			Description = _description,
			Id = _id
		};

		public RoleTypeBuilder WithDescription(string description)
		{
			_description = description;
			return this;
		}

		public RoleTypeBuilder WithId(long id)
		{
			_id = id;
			return this;
		}
	}
}
using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Tests.Unit.Builders.Model
{
	public class RoleTypeBuilder
	{
		private string _name;
		private long _id;

		public RoleType Build() => new RoleType
		{
			Name = _name,
			Id = _id
		};

		public RoleTypeBuilder WithName(string name)
		{
			_name = name;
			return this;
		}

		public RoleTypeBuilder WithId(long id)
		{
			_id = id;
			return this;
		}
	}
}
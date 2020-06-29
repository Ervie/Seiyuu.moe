using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Tests.Unit.Builders.Model
{
	public class RoleTypeBuilder
	{
		private string _name;

		public RoleType Build() => new RoleType
		{
			Name = _name
		};

		public RoleTypeBuilder WithName(string name)
		{
			_name = name;
			return this;
		}
	}
}
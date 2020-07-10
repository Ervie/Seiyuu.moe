using System.Collections.Generic;

namespace SeiyuuMoe.Domain.Entities
{
	public partial class RoleType
	{
		public RoleType()
		{
			Role = new HashSet<Role>();
		}

		public long Id { get; set; }
		public string Name { get; set; }

		public virtual ICollection<Role> Role { get; set; }
	}
}
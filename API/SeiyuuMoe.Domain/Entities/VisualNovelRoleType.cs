using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SeiyuuMoe.Domain.Entities
{
	public class VisualNovelRoleType
	{
		public VisualNovelRoleType()
		{
			Role = new HashSet<VisualNovelRole>();
		}

		[Key]
		public VisualNovelRoleTypeId Id { get; set; }

		public string Description { get; set; }

		public virtual ICollection<VisualNovelRole> Role { get; set; }
	}

	public enum VisualNovelRoleTypeId : long
	{
		All = 0,
		Main = 1,
		Primary = 2,
		Side = 3,
		Appears = 4
	}
}
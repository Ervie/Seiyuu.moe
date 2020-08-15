using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SeiyuuMoe.Domain.Entities
{
	public partial class AnimeRoleType
	{
		public AnimeRoleType()
		{
			Role = new HashSet<AnimeRole>();
		}

		[Key]
		public long Id { get; set; }
		public string Description { get; set; }

		public virtual ICollection<AnimeRole> Role { get; set; }
	}
}
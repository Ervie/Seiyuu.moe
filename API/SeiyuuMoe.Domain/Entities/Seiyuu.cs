using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SeiyuuMoe.Domain.Entities
{
	public partial class Seiyuu
	{
		public Seiyuu()
		{
			Role = new HashSet<AnimeRole>();
		}

		public string Name { get; set; }

		[Key]
		public Guid Id { get; set; }

		public long MalId { get; set; }

		public string ImageUrl { get; set; }
		public long? Popularity { get; set; }
		public string KanjiName { get; set; }
		public string About { get; set; }
		public DateTime? Birthday { get; set; }

		public virtual ICollection<AnimeRole> Role { get; set; }
	}
}
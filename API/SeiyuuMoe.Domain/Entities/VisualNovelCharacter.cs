using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SeiyuuMoe.Domain.Entities
{
	public class VisualNovelCharacter
	{
		public VisualNovelCharacter()
		{
			Role = new HashSet<VisualNovelRole>();
		}

		public string Name { get; set; }

		[Key]
		public Guid Id { get; set; }

		public long VndbId { get; set; }

		public string ImageUrl { get; set; }
		public string KanjiName { get; set; }
		public string About { get; set; }
		public string Nicknames { get; set; }
		public DateTime ModificationDate { get; set; }

		public virtual ICollection<VisualNovelRole> Role { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SeiyuuMoe.Domain.Entities
{
	public partial class Seiyuu
	{
		public Seiyuu()
		{
			AnimeRoles = new HashSet<AnimeRole>();
			VisualNovelRoles = new HashSet<VisualNovelRole>();
		}

		public string Name { get; set; }

		[Key]
		public Guid Id { get; set; }

		public long MalId { get; set; }
		public long? VndbId { get; set; }

		public string ImageUrl { get; set; }
		public long? Popularity { get; set; }
		public string KanjiName { get; set; }
		public string About { get; set; }
		public DateTime? Birthday { get; set; }
		public DateTime ModificationDate { get; set; }

		public virtual ICollection<AnimeRole> AnimeRoles { get; set; }
		public virtual ICollection<VisualNovelRole> VisualNovelRoles { get; set; }
	}
}
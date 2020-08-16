using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SeiyuuMoe.Domain.Entities
{
	public partial class AnimeStatus
	{
		public AnimeStatus()
		{
			Anime = new HashSet<Anime>();
		}

		[Key]
		public long Id { get; set; }
		public string Description { get; set; }

		public virtual ICollection<Anime> Anime { get; set; }
	}
}
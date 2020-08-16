using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SeiyuuMoe.Domain.Entities
{
	public partial class AnimeSeason
	{
		public AnimeSeason()
		{
			Anime = new HashSet<Anime>();
		}

		[Key]
		public long Id { get; set; }

		public long Year { get; set; }
		public string Name { get; set; }

		public virtual ICollection<Anime> Anime { get; set; }
	}
}
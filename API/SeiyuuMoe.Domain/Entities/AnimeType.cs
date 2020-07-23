using System.Collections.Generic;

namespace SeiyuuMoe.Domain.Entities
{
	public partial class AnimeType
	{
		public AnimeType()
		{
			Anime = new HashSet<Anime>();
		}

		public long Id { get; set; }
		public string Name { get; set; }

		public virtual ICollection<Anime> Anime { get; set; }
	}
}
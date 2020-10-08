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
		public AnimeStatusId Id { get; set; }
		public string Description { get; set; }

		public virtual ICollection<Anime> Anime { get; set; }
	}

	public enum AnimeStatusId: int
	{
		All = 0,
		Finished_Airing = 1,
		Currently_Airing = 2,
		Not_yet_aired = 3
	}
}
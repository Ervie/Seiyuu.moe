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

	public enum AnimeStatusId: long
	{
		All = 0,
		FinishedAiring = 1,
		CurrentlyAiring = 2,
		Notyetaired = 3
	}
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SeiyuuMoe.Domain.Entities
{
	public partial class AnimeType
	{
		public AnimeType()
		{
			Anime = new HashSet<Anime>();
		}

		[Key]
		public AnimeTypeId Id { get; set; }
		public string Description { get; set; }

		public virtual ICollection<Anime> Anime { get; set; }
	}

	public enum AnimeTypeId: int
	{
		AllTypes = 0,
		TV = 1,
		Movie = 2,
		OVA = 3,
		Special = 4,
		ONA = 5,
		Music = 6
	}
}
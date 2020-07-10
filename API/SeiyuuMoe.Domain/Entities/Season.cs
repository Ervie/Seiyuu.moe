﻿using System.Collections.Generic;

namespace SeiyuuMoe.Domain.Entities
{
	public partial class Season
	{
		public Season()
		{
			Anime = new HashSet<Anime>();
		}

		public long Id { get; set; }
		public long Year { get; set; }
		public string Name { get; set; }

		public virtual ICollection<Anime> Anime { get; set; }
	}
}
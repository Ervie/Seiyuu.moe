﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SeiyuuMoe.Domain.Entities
{
	public class VisualNovel
	{
		public VisualNovel()
		{
			Role = new HashSet<VisualNovelRole>();
		}

		[Key]
		public Guid Id { get; set; }
		public long VndbId { get; set; }
		public string Title { get; set; }
		public string TitleOriginal { get; set; }
		public string Alias { get; set; }
		public string ImageUrl { get; set; }
		public string About { get; set; }
		public int Popularity { get; set; }
		public DateTime ModificationDate { get; set; }
		public virtual ICollection<VisualNovelRole> Role { get; set; }
	}
}
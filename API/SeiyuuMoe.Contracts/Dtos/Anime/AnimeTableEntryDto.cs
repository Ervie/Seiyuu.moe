using System;

namespace SeiyuuMoe.Contracts.Dtos
{
	public class AnimeTableEntryDto
	{
		public string Title { get; set; }

		public string ImageUrl { get; set; }

		public string Url { get; set; }

		public long MalId { get; set; }

		public DateTime? AiringFrom { get; set; }
	}
}
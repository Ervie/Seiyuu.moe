using System;

namespace SeiyuuMoe.Contracts.Dtos
{
	public class AnimeDto
	{
		public string Title { get; set; }

		public string ImageUrl { get; set; }

		public long MalId { get; set; }

		public DateTime? AiringFrom { get; set; }
	}
}
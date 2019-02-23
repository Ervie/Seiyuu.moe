using System;

namespace SeiyuuMoe.Contracts.Dtos
{
	public class AnimeDto
	{
		public string Title { get; set; }

		public string ImageUrl { get; set; }

		public long MalId { get; set; }

		public int Popularity { get; set; }

		public string TitleSynonyms { get; set; }

		public DateTime? AiringFrom { get; set; }
	}
}
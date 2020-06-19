using System;

namespace SeiyuuMoe.Contracts.Dtos
{
	public class AnimeTableEntryDto
	{
		public long MalId { get; }

		public string Title { get; }

		public string ImageUrl { get; }

		public string Url { get; }

		public DateTime? AiringFrom { get; }

		public AnimeTableEntryDto(long malId, string title, string imageUrl, string url, DateTime? airingDate)
		{
			MalId = malId;
			Title = title;
			ImageUrl = imageUrl;
			Url = url;
			AiringFrom = airingDate;
		}
	}
}
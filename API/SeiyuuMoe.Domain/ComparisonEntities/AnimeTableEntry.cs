using System;

namespace SeiyuuMoe.Domain.ComparisonEntities
{
	public class AnimeTableEntry
	{
		public long MalId { get; }

		public string Title { get; }

		public string ImageUrl { get; }

		public string Url { get; }

		public DateTime? AiringFrom { get; }

		public AnimeTableEntry(long malId, string title, string imageUrl, string url, DateTime? airingDate)
		{
			MalId = malId;
			Title = title;
			ImageUrl = imageUrl;
			Url = url;
			AiringFrom = airingDate;
		}
	}
}
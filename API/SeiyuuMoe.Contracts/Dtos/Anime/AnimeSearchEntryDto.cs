namespace SeiyuuMoe.Contracts.Dtos
{
	public class AnimeSearchEntryDto
	{
		public string Title { get; }

		public string ImageUrl { get; }

		public long MalId { get; }

		public AnimeSearchEntryDto(string title, string imageUrl, long malId)
		{
			Title = title;
			ImageUrl = imageUrl;
			MalId = malId;
		}
	}
}
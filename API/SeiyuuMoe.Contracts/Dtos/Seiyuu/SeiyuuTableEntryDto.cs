namespace SeiyuuMoe.Contracts.Dtos
{
	public class SeiyuuTableEntryDto
	{
		public long MalId { get; }
		public string Name { get; }

		public string ImageUrl { get; }

		public string Url { get; }

		public SeiyuuTableEntryDto(long malId, string name, string imageUrl, string url)
		{
			MalId = malId;
			Name = name;
			ImageUrl = imageUrl;
			Url = url;
		}
	}
}
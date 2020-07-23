namespace SeiyuuMoe.Domain.ComparisonEntities
{
	public class SeiyuuTableEntry
	{
		public long MalId { get; }

		public string Name { get; }

		public string ImageUrl { get; }

		public string Url { get; }

		public SeiyuuTableEntry(long malId, string name, string imageUrl, string url)
		{
			MalId = malId;
			Name = name;
			ImageUrl = imageUrl;
			Url = url;
		}
	}
}
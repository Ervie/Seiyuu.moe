namespace SeiyuuMoe.Domain.ComparisonEntities
{
	public class CharacterTableEntry
	{
		public string Name { get; }

		public string ImageUrl { get; }

		public string Url { get; }

		public long MalId { get; }

		public CharacterTableEntry(long malId, string name, string imageUrl, string url)
		{
			MalId = malId;
			Name = name;
			ImageUrl = imageUrl;
			Url = url;
		}
	}
}
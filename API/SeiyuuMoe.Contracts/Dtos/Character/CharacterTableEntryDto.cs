namespace SeiyuuMoe.Contracts.Dtos
{
	public class CharacterTableEntryDto
	{
		public string Name { get; }

		public string ImageUrl { get; }

		public string Url { get; }

		public long MalId { get; }

		public CharacterTableEntryDto(long malId, string name, string imageUrl, string url)
		{
			MalId = malId;
			Name = name;
			ImageUrl = imageUrl;
			Url = url;
		}
	}
}
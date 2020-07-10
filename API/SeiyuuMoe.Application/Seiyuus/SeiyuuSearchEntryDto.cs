namespace SeiyuuMoe.Application.Seiyuus
{
	public class SeiyuuSearchEntryDto
	{
		public string Name { get; }

		public string ImageUrl { get; }

		public long MalId { get; }

		public SeiyuuSearchEntryDto(long malId, string name, string imageUrl)
		{
			MalId = malId;
			Name = name;
			ImageUrl = imageUrl;
		}
	}
}
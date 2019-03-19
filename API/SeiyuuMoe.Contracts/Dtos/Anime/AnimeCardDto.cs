namespace SeiyuuMoe.Contracts.Dtos
{
	public class AnimeCardDto
	{
		public string Title { get; set; }

		public string ImageUrl { get; set; }

		public long MalId { get; set; }

		public string JapaneseTitle { get; set; }

		public string TitleSynonyms { get; set; }

		public string About { get; set; }

		public string AiringDate { get; set; }

		public string Status { get; set; }

		public string Type { get; set; }

		public string Season { get; set; }
	}
}
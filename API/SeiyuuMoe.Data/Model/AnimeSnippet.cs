using Newtonsoft.Json;
using System;

namespace SeiyuuMoe.Data.Model
{
	public class AnimeSnippet
	{
		public long Id { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("image_url")]
		public string ImageUrl { get; set; }

		[JsonProperty("mal_id")]
		public long MalId { get; set; }

		[JsonProperty("popularity")]
		public int Popularity { get; set; }

		[JsonProperty("title_synonyms")]
		public string TitleSynonyms { get; set; }

		[JsonProperty("aired_from")]
		public DateTime? AiringFrom{ get; set; }
	}
}
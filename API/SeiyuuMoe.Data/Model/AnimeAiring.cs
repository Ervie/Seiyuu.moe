using Newtonsoft.Json;
using System;

namespace SeiyuuMoe.Data.Model
{
	public class AnimeAiring
	{
		[JsonProperty("mal_id")]
		public long MalId { get; set; }

		[JsonProperty("airing_from")]
		public DateTime? AiringFrom { get; set; }
	}
}
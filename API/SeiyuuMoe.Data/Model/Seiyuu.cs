using Newtonsoft.Json;

namespace SeiyuuMoe.Data.Model
{
	public class Seiyuu
	{
		public long Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("image_url")]
		public string ImageUrl { get; set; }

		[JsonProperty("mal_id")]
		public long MalId { get; set; }

		[JsonProperty("popularity")]
		public int? Popularity { get; set; }

		[JsonProperty("number_of_roles")]
		public int? NumberOfRoles { get; set; }

		[JsonProperty("japanese_name")]
		public string JapaneseName { get; set; }
	}
}
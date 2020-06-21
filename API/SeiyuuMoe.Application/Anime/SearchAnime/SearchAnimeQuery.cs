using System.Collections.Generic;

namespace SeiyuuMoe.Application.Anime.SearchAnime
{
	public class SearchAnimeQuery
	{
		public string Title { get; set; }

		public ICollection<long> MalId { get; set; }

		public long? SeasonId { get; set; }

		public long? AnimeTypeId { get; set; }
	}
}
using System.Collections.Generic;

namespace SeiyuuMoe.Contracts.SearchCriteria
{
	public class AnimeSearchCriteria
	{
		public string Title { get; set; }

		public ICollection<long> MalId { get; set; }

		public long? SeasonId { get; set; }

		public long? AnimeTypeId { get; set; }
	}
}
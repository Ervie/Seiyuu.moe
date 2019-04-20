using System.Collections.Generic;

namespace SeiyuuMoe.Contracts.SearchCriteria
{
	public class SeiyuuComparisonSearchCriteria
	{
		public ICollection<long> SeiyuuMalId { get; set; }

		public bool MainRolesOnly { get; set; }

		public bool GroupByFranchise { get; set; }
	}
}
using System.Collections.Generic;

namespace SeiyuuMoe.Application.SeiyuuComparisons.CompareSeiyuu
{
	public class CompareSeiyuuQuery
	{
		public ICollection<long> SeiyuuMalIds { get; set; }

		public bool MainRolesOnly { get; set; }

		public bool GroupByFranchise { get; set; }
	}
}
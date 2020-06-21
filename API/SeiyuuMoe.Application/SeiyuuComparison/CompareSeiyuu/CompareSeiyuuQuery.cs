using System.Collections.Generic;

namespace SeiyuuMoe.Application.SeiyuuComparison.CompareSeiyuu
{
	public class CompareSeiyuuQuery
	{
		public ICollection<long> SeiyuuMalIds { get; set; }

		public bool MainRolesOnly { get; set; }

		public bool GroupByFranchise { get; set; }
	}
}
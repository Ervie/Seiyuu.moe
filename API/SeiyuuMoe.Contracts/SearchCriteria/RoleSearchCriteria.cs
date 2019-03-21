using System.Collections.Generic;

namespace SeiyuuMoe.Contracts.SearchCriteria
{
	public class RoleSearchCriteria
	{
		public ICollection<long> AnimeMalId { get; set; }

		public ICollection<long> SeiyuuMalId { get; set; }

		public bool? MainRolesOnly { get; set; }
	}
}
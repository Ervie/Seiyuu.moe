using System;
using System.Collections.Generic;
using System.Text;

namespace SeiyuuMoe.Contracts.SearchCriteria
{
	public class RoleSearchCriteria
	{
		public ICollection<long> AnimeMalId { get; set; }

		public ICollection<long> SeiyuuMalId { get; set; }
	}
}

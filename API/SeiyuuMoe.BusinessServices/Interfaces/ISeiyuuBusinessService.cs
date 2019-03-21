using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.Dtos.Other;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Repositories.Models;
using SeiyuuMoe.WebEssentials;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.BusinessServices
{
	public interface ISeiyuuBusinessService
	{
		Task<PagedResult<SeiyuuSearchEntryDto>> GetAsync(Query<SeiyuuSearchCriteria> query);

		Task<ICollection<SeiyuuComparisonEntryDto>> GetSeiyuuComparison(RoleSearchCriteria searchCriteria);
	}
}

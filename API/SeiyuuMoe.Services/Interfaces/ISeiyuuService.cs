using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.Dtos.Other;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Repositories.Models;
using SeiyuuMoe.WebEssentials;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.Services
{
	public interface ISeiyuuService
	{
		Task<QueryResponse<SeiyuuCardDto>> GetSingleAsync(long id);

		Task<QueryResponse<PagedResult<SeiyuuSearchEntryDto>>> GetAsync(Query<SeiyuuSearchCriteria> query);

		Task<QueryResponse<ICollection<SeiyuuComparisonEntryDto>>> GetSeiyuuComparison(Query<RoleSearchCriteria> searchCriteria);
	}
}

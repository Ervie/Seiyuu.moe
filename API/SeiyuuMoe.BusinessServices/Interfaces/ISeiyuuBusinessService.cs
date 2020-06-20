using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.Dtos.Other;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Domain.WebEssentials;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.BusinessServices
{
	public interface ISeiyuuBusinessService
	{
		Task<SeiyuuCardDto> GetSingleAsync(long id);

		Task<PagedResult<SeiyuuSearchEntryDto>> GetAsync(Query<SeiyuuSearchCriteria> query);

		Task<ICollection<SeiyuuComparisonEntryDto>> GetSeiyuuComparison(SeiyuuComparisonSearchCriteria searchCriteria);
	}
}
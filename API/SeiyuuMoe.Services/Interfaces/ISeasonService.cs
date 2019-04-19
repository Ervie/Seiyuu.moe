using SeiyuuMoe.Contracts.Dtos.Season;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Repositories.Models;
using SeiyuuMoe.WebEssentials;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.Services.Interfaces
{
	public interface ISeasonService
	{
		Task<QueryResponse<PagedResult<SeasonSummaryEntryDto>>> GetSeasonSummary(Query<SeasonSearchCriteria> query);
	}
}
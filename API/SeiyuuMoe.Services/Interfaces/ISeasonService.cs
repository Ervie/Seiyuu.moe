using SeiyuuMoe.Contracts.Dtos.Season;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Domain.WebEssentials;
using System.Threading.Tasks;

namespace SeiyuuMoe.Services.Interfaces
{
	public interface ISeasonService
	{
		Task<QueryResponse<PagedResult<SeasonSummaryEntryDto>>> GetSeasonSummary(Query<SeasonSummarySearchCriteria> query);
	}
}
using SeiyuuMoe.Contracts.Dtos.Season;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.WebEssentials;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.Services.Interfaces
{
	public interface ISeasonService
	{
		Task<QueryResponse<ICollection<SeasonSummaryEntryDto>>> GetSeasonSummary(Query<SeasonSearchCriteria> query);
	}
}
using SeiyuuMoe.Contracts.Dtos.Season;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Domain.WebEssentials;
using System.Threading.Tasks;

namespace SeiyuuMoe.BusinessServices
{
	public interface ISeasonBusinessService
	{
		Task<PagedResult<SeasonSummaryEntryDto>> GetSeasonRolesSummary(Query<SeasonSummarySearchCriteria> seasonSearchCriteria);
	}
}
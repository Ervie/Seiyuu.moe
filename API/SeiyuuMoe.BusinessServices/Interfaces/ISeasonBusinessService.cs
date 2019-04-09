using SeiyuuMoe.Contracts.Dtos.Season;
using SeiyuuMoe.Contracts.SearchCriteria;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.BusinessServices
{
	public interface ISeasonBusinessService
	{
		Task<ICollection<SeasonSummaryEntryDto>> GetSeasonRolesSummary(SeasonSearchCriteria seasonSearchCriteria);
	}
}
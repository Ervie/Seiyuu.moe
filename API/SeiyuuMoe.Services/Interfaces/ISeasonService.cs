using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.WebEssentials;

namespace SeiyuuMoe.Services.Interfaces
{
	public interface ISeasonService
	{
		Task<QueryResponse<object>> GetSeasonSummary(Query<SeasonSearchCriteria> query);
	}
}

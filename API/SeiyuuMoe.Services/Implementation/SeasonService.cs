using SeiyuuMoe.BusinessServices;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Services.Interfaces;
using SeiyuuMoe.WebEssentials;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SeiyuuMoe.Services.Implementation
{
	class SeasonService : ISeasonService
	{
		ISeasonBusinessService seasonBusinessService;

		public SeasonService(ISeasonBusinessService seasonBusinessService)
		{
			this.seasonBusinessService = seasonBusinessService;
		}

		public Task<QueryResponse<object>> GetSeasonSummary(Query<SeasonSearchCriteria> query)
		{
			throw new NotImplementedException();
		}
	}
}

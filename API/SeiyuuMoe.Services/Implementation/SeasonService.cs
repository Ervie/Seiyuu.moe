using EnsureThat;
using SeiyuuMoe.BusinessServices;
using SeiyuuMoe.Contracts.Dtos.Season;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Repositories.Models;
using SeiyuuMoe.Services.Interfaces;
using SeiyuuMoe.WebEssentials;
using System.Threading.Tasks;

namespace SeiyuuMoe.Services.Implementation
{
	internal class SeasonService : ISeasonService
	{
		private readonly ISeasonBusinessService _seasonBusinessService;

		public SeasonService(ISeasonBusinessService seasonBusinessService)
		{
			_seasonBusinessService = seasonBusinessService;
		}

		public async Task<QueryResponse<PagedResult<SeasonSummaryEntryDto>>> GetSeasonSummary(Query<SeasonSummarySearchCriteria> query)
		{
			Ensure.That(query, nameof(query)).IsNotNull();
			Ensure.That(query.SearchCriteria, nameof(query.SearchCriteria)).IsNotNull();
			Ensure.That(query.SearchCriteria.Year).IsGte(1916);
			Ensure.That(query.SearchCriteria.Season).IsNotEmptyOrWhiteSpace();

			return new QueryResponse<PagedResult<SeasonSummaryEntryDto>>(await _seasonBusinessService.GetSeasonRolesSummary(query));
		}
	}
}
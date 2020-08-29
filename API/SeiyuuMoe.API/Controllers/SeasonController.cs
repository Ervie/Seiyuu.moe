using Microsoft.AspNetCore.Mvc;
using SeiyuuMoe.API.Controllers.Base;
using SeiyuuMoe.Application.Seasons;
using SeiyuuMoe.Application.Seasons.GetSeasonSummaries;
using SeiyuuMoe.Domain.WebEssentials;
using SeiyuuMoe.Infrastructure.Logger;
using System.Threading.Tasks;

namespace SeiyuuMoe.API.Controllers
{
	[Route("api/season")]
	public class SeasonController : BaseController
	{
		private readonly GetSeasonSummariesQueryHandler _getSeasonSummariesQueryHandler;

		public SeasonController(GetSeasonSummariesQueryHandler getSeasonSummariesQueryHandler, ILoggingService loggingService) : base(loggingService)
		{
			_getSeasonSummariesQueryHandler = getSeasonSummariesQueryHandler;
		}

		[HttpGet]
		[Route("Summary")]
		public Task<PagedResult<SeasonSummaryEntryDto>> GetSeasonSummary([FromQuery] GetSeasonSummariesQuery query)
			=> HandleAsync(async () => await _getSeasonSummariesQueryHandler.HandleAsync(query));
	}
}
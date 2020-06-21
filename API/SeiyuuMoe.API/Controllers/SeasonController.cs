using Microsoft.AspNetCore.Mvc;
using SeiyuuMoe.API.Controllers.Base;
using SeiyuuMoe.Application.Season.GetSeasonSummaries;
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
		public Task<IActionResult> GetSeasonSummary([FromQuery] GetSeasonSummariesQuery query)
		{
			return Handle(async () => HandleServiceResult(await _getSeasonSummariesQueryHandler.HandleAsync(query)));
		}
	}
}
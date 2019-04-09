using Microsoft.AspNetCore.Mvc;
using SeiyuuMoe.API.Controllers.Base;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Logger;
using SeiyuuMoe.Services;
using SeiyuuMoe.Services.Interfaces;
using SeiyuuMoe.WebEssentials;
using System.Threading.Tasks;

namespace SeiyuuMoe.API.Controllers
{
	[Route("api/season")]
	public class SeasonController : BaseController
	{
		private readonly ISeasonService seasonService;

		public SeasonController(ISeasonService seasonService, ILoggingService loggingService) : base(loggingService)
		{
			this.seasonService = seasonService;
		}

		[HttpGet]
		[Route("Summary")]
		public Task<IActionResult> GetSeasonSummary([FromQuery] Query<SeasonSearchCriteria> query)
		{
			return Handle(async () => HandleServiceResult(await seasonService.GetSeasonSummary(query)));
		}
	}
}
using Microsoft.AspNetCore.Mvc;
using SeiyuuMoe.API.Controllers.Base;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Domain.WebEssentials;
using SeiyuuMoe.Infrastructure.Logger;
using SeiyuuMoe.Services.Interfaces;
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
		public Task<IActionResult> GetSeasonSummary([FromQuery] Query<SeasonSummarySearchCriteria> query)
		{
			return Handle(async () => HandleServiceResult(await seasonService.GetSeasonSummary(query)));
		}
	}
}
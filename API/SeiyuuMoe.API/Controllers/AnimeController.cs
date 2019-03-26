using Microsoft.AspNetCore.Mvc;
using SeiyuuMoe.API.Controllers.Base;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Logger;
using SeiyuuMoe.Services;
using SeiyuuMoe.WebEssentials;
using System.Threading.Tasks;

namespace SeiyuuMoe.API.Controllers
{
	[Route("api/anime")]
	public class AnimeController : BaseController
	{
		private readonly IAnimeService animeService;

		public AnimeController(IAnimeService animeService, ILoggingService loggingService): base(loggingService)
		{
			this.animeService = animeService;
		}

		[HttpGet]
		[Route("{id}")]
		public Task<IActionResult> GetCardInfo(long id)
		{
			return Handle(async () => HandleServiceResult(await animeService.GetSingleAsync(id)));
		}

		[HttpGet]
		public Task<IActionResult> GetSearchEntries([FromQuery] Query<AnimeSearchCriteria> query)
		{
			return Handle(async () => HandleServiceResult(await animeService.GetAsync(query)));
		}

		[HttpGet]
		[Route("Compare")]
		public Task<IActionResult> GetComparison([FromQuery] Query<AnimeComparisonSearchCriteria> query)
		{
			return Handle(async () => HandleServiceResult(await animeService.GetAnimeComparison(query)));
		}
	}
}

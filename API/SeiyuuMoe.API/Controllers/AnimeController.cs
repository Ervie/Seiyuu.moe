using Microsoft.AspNetCore.Mvc;
using SeiyuuMoe.API.Controllers.Base;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Services;
using SeiyuuMoe.WebEssentials;
using System.Threading.Tasks;

namespace SeiyuuMoe.API.Controllers
{
	[Route("api/anime")]
	public class AnimeController : BaseController
	{
		private readonly IAnimeService animeService;

		public AnimeController(IAnimeService animeService)
		{
			this.animeService = animeService;
		}

		[HttpGet("{query}")]
		public Task<IActionResult> Get([FromQuery] Query<AnimeSearchCriteria> query)
		{
			return Handle(async () => HandleServiceResult(await animeService.GetAsync(query)));
		}

		[HttpGet]
		[Route("AiringDates")]
		public Task<IActionResult> GetDates([FromQuery] Query<AnimeSearchCriteria> query)
		{
			return Handle(async () => HandleServiceResult(await animeService.GetDatesAsync(query)));
		}
	}
}

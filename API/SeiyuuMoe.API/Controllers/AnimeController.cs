using Microsoft.AspNetCore.Mvc;
using SeiyuuMoe.API.Controllers.Base;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Repositories.Models;
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
		public Task<IActionResult> Get(Query<AnimeSearchCriteria> query)
		{
			return Handle(async () => HandleServiceResult<PagedResult<AnimeDto>>(await animeService.GetAsync(query)));
		}

		//	[HttpGet]
		//	[Route("AiringDates")]
		//	public ICollection<AnimeAiring> Get([FromQuery] ICollection<long> malId) =>
		//		 _dbContext.AnimeSet
		//		.Where(x => malId.Contains(x.MalId))
		//		.Select(entry => new AnimeAiring{ MalId = entry.MalId, AiringFrom = entry.AiringFrom })
		//		.OrderByDescending(x => x.AiringFrom).ToList();
		//}
	}
}
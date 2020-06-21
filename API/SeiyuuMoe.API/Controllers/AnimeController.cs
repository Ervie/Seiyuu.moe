using Microsoft.AspNetCore.Mvc;
using SeiyuuMoe.API.Controllers.Base;
using SeiyuuMoe.Application.Anime.GetAnimeCardInfo;
using SeiyuuMoe.Application.Anime.SearchAnime;
using SeiyuuMoe.Application.AnimeComparison.CompareAnime;
using SeiyuuMoe.Infrastructure.Logger;
using System.Threading.Tasks;

namespace SeiyuuMoe.API.Controllers
{
	[Route("api/anime")]
	public class AnimeController : BaseController
	{
		private readonly GetAnimeCardInfoQueryHandler _getAnimeCardInfoQueryHandler;
		private readonly CompareAnimeQueryHandler _compareAnimeQueryHandler;
		private readonly SearchAnimeQueryHandler _searchAnimeQueryHandler;

		public AnimeController(
			CompareAnimeQueryHandler compareAnimeQueryHandler,
			GetAnimeCardInfoQueryHandler getAnimeCardInfoQueryHandler,
			SearchAnimeQueryHandler searchAnimeQueryHandler,
			ILoggingService loggingService
		) : base(loggingService)
		{
			_compareAnimeQueryHandler = compareAnimeQueryHandler;
			_getAnimeCardInfoQueryHandler = getAnimeCardInfoQueryHandler;
			_searchAnimeQueryHandler = searchAnimeQueryHandler;
		}

		[HttpGet]
		[Route("{id}")]
		public Task<IActionResult> GetCardInfo(long id)
		{
			var query = new GetAnimeCardInfoQuery(id);
			return Handle(async () => HandleServiceResult(await _getAnimeCardInfoQueryHandler.HandleAsync(query)));
		}

		[HttpGet]
		public Task<IActionResult> GetSearchEntries([FromQuery] SearchAnimeQuery query)
		{
			return Handle(async () => HandleServiceResult(await _searchAnimeQueryHandler.HandleAsync(query)));
		}

		[HttpGet]
		[Route("Compare")]
		public Task<IActionResult> GetComparison([FromQuery] CompareAnimeQuery query)
		{
			return Handle(async () => HandleServiceResult(await _compareAnimeQueryHandler.HandleAsync(query)));
		}
	}
}
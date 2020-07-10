using Microsoft.AspNetCore.Mvc;
using SeiyuuMoe.API.Controllers.Base;
using SeiyuuMoe.Application.SeiyuuComparisons.CompareSeiyuu;
using SeiyuuMoe.Application.Seiyuus.GetSeiyuuCardInfo;
using SeiyuuMoe.Application.Seiyuus.SearchSeiyuu;
using SeiyuuMoe.Infrastructure.Logger;
using System.Threading.Tasks;

namespace SeiyuuMoe.API.Controllers
{
	[Route("api/seiyuu")]
	public class SeiyuuController : BaseController
	{
		private readonly GetSeiyuuCardInfoQueryHandler _getSeiyuuCardInfoQueryHandler;
		private readonly SearchSeiyuuQueryHandler _searchSeiyuuQueryHandler;
		private readonly CompareSeiyuuQueryHandler _compareSeiyuuQueryHandler;

		public SeiyuuController(
			ILoggingService loggingService,
			GetSeiyuuCardInfoQueryHandler getSeiyuuCardInfoQueryHandler,
			SearchSeiyuuQueryHandler searchSeiyuuQueryHandler,
			CompareSeiyuuQueryHandler compareSeiyuuQueryHandler
		) : base(loggingService)
		{
			_getSeiyuuCardInfoQueryHandler = getSeiyuuCardInfoQueryHandler;
			_searchSeiyuuQueryHandler = searchSeiyuuQueryHandler;
			_compareSeiyuuQueryHandler = compareSeiyuuQueryHandler;
		}

		[HttpGet]
		[Route("{id}")]
		public Task<IActionResult> GetCardInfo(long id)
		{
			var query = new GetSeiyuuCardInfoQuery(id);
			return Handle(async () => HandleServiceResult(await _getSeiyuuCardInfoQueryHandler.HandleAsync(query)));
		}

		[HttpGet]
		public Task<IActionResult> Get([FromQuery] SearchSeiyuuQuery query)
		{
			return Handle(async () => HandleServiceResult(await _searchSeiyuuQueryHandler.HandleAsync(query)));
		}

		[HttpGet]
		[Route("Compare")]
		public Task<IActionResult> GetComparison([FromQuery] CompareSeiyuuQuery query)
		{
			return Handle(async () => HandleServiceResult(await _compareSeiyuuQueryHandler.HandleAsync(query)));
		}
	}
}
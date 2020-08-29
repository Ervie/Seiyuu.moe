using Microsoft.AspNetCore.Mvc;
using SeiyuuMoe.API.Controllers.Base;
using SeiyuuMoe.Application.SeiyuuComparisons;
using SeiyuuMoe.Application.SeiyuuComparisons.CompareSeiyuu;
using SeiyuuMoe.Application.Seiyuus;
using SeiyuuMoe.Application.Seiyuus.GetSeiyuuCardInfo;
using SeiyuuMoe.Application.Seiyuus.SearchSeiyuu;
using SeiyuuMoe.Domain.WebEssentials;
using SeiyuuMoe.Infrastructure.Logger;
using System.Collections.Generic;
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
		public Task<SeiyuuCardDto> GetCardInfo(long id)
		{
			var query = new GetSeiyuuCardInfoQuery(id);
			return HandleAsync(async () => await _getSeiyuuCardInfoQueryHandler.HandleAsync(query));
		}

		[HttpGet]
		public Task<PagedResult<SeiyuuSearchEntryDto>> Get([FromQuery] SearchSeiyuuQuery query) 
			=> HandleAsync(async () => await _searchSeiyuuQueryHandler.HandleAsync(query));

		[HttpGet]
		[Route("Compare")]
		public Task<ICollection<SeiyuuComparisonEntryDto>> GetComparison([FromQuery] CompareSeiyuuQuery query)
			=> HandleAsync(async () => await _compareSeiyuuQueryHandler.HandleAsync(query));
		
	}
}
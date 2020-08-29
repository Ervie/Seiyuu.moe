using Microsoft.AspNetCore.Mvc;
using SeiyuuMoe.AnimeComparisons;
using SeiyuuMoe.Animes;
using SeiyuuMoe.API.Controllers.Base;
using SeiyuuMoe.Application.AnimeComparisons.CompareAnime;
using SeiyuuMoe.Application.Animes;
using SeiyuuMoe.Application.Animes.GetAnimeCardInfo;
using SeiyuuMoe.Application.Animes.SearchAnime;
using SeiyuuMoe.Domain.WebEssentials;
using SeiyuuMoe.Infrastructure.Logger;
using System.Collections.Generic;
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
		public async Task<QueryResponse<AnimeCardDto>> GetCardInfo(long id)
		{
			var query = new GetAnimeCardInfoQuery(id);
			return await HandleAsync(async () => await _getAnimeCardInfoQueryHandler.HandleAsync(query));
		}

		[HttpGet]
		public Task<QueryResponse<PagedResult<AnimeSearchEntryDto>>> GetSearchEntries([FromQuery] SearchAnimeQuery query)
			=> HandleAsync(async () => await _searchAnimeQueryHandler.HandleAsync(query));

		[HttpGet]
		[Route("Compare")]
		public Task<QueryResponse<ICollection<AnimeComparisonEntryDto>>> GetComparison([FromQuery] CompareAnimeQuery query)
			=> HandleAsync(async () => await _compareAnimeQueryHandler.HandleAsync(query));
		
	}
}
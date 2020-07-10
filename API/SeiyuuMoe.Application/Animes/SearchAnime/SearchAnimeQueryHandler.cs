using SeiyuuMoe.Animes;
using SeiyuuMoe.Application.Animes.Extensions;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.WebEssentials;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.Application.Animes.SearchAnime
{
	public class SearchAnimeQueryHandler
	{
		private readonly IAnimeRepository _animeRepository;
		private readonly IAnimeSearchCriteriaService _animeSearchCriteriaService;

		public SearchAnimeQueryHandler(
			IAnimeRepository animeRepository,
			IAnimeSearchCriteriaService animeSearchCriteriaService
		)
		{
			_animeRepository = animeRepository;
			_animeSearchCriteriaService = animeSearchCriteriaService;
		}

		public async Task<QueryResponse<PagedResult<AnimeSearchEntryDto>>> HandleAsync(SearchAnimeQuery query)
		{
			var expression = _animeSearchCriteriaService.BuildExpression(query);

			var entities = await _animeRepository.GetOrderedPageAsync(expression);

			var result = entities.Map<Anime, AnimeSearchEntryDto>(entities.Results.Select(x => x.ToAnimeSearchEntryDto()));

			return new QueryResponse<PagedResult<AnimeSearchEntryDto>>(result);
		}
	}
}
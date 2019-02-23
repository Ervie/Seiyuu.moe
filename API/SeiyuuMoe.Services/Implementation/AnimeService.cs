using AutoMapper;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Repositories.Models;
using SeiyuuMoe.Repositories.Repositories;
using SeiyuuMoe.Services.SearchCriteria;
using SeiyuuMoe.WebEssentials;
using System.Threading.Tasks;

namespace SeiyuuMoe.Services
{
	internal class AnimeService : IAnimeService
	{
		private readonly IMapper mapper;
		private readonly IAnimeRepository animeRepository;
		private readonly IAnimeSearchCriteriaService animeSearchCriteriaService;

		public AnimeService(IMapper mapper, IAnimeRepository animeRepository, IAnimeSearchCriteriaService animeSearchCriteriaService)
		{
			this.mapper = mapper;
			this.animeRepository = animeRepository;
			this.animeSearchCriteriaService = animeSearchCriteriaService;
		}

		public async Task<PagedResult<AnimeDto>> GetAsync(Query<AnimeSearchCriteria> query)
		{
			var expression = await animeSearchCriteriaService.BuildExpression(query.SearchCriteria);

			var entities = await animeRepository.GetOrderedPageAsync(expression, query.SortExpression, query.Page, query.PageSize);

			return mapper.Map<PagedResult<AnimeDto>>(entities);
		}

		public async Task<PagedResult<AnimeAiringDto>> GetDatesAsync(Query<AnimeSearchCriteria> query)
		{
			var expression = await animeSearchCriteriaService.BuildExpression(query.SearchCriteria);

			var entities = await animeRepository.GetOrderedPageAsync(expression, query.SortExpression, query.Page, query.PageSize);

			return mapper.Map<PagedResult<AnimeAiringDto>>(entities);
		}
	}
}
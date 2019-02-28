﻿using AutoMapper;
using SeiyuuMoe.BusinessServices.SearchCriteria;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Repositories.Models;
using SeiyuuMoe.Repositories.Repositories;
using SeiyuuMoe.WebEssentials;
using System.Threading.Tasks;

namespace SeiyuuMoe.BusinessServices
{
	internal class AnimeBusinessService : IAnimeBusinessService
	{
		private readonly IMapper mapper;
		private readonly IAnimeRepository animeRepository;
		private readonly IAnimeSearchCriteriaService animeSearchCriteriaService;

		public AnimeBusinessService(IMapper mapper, IAnimeRepository animeRepository, IAnimeSearchCriteriaService animeSearchCriteriaService)
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
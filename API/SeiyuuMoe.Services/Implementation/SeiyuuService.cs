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
	class SeiyuuService : ISeiyuuService
	{
		private readonly IMapper mapper;
		private readonly ISeiyuuRepository seiyuuRepository;
		private readonly ISeiyuuSearchCriteriaService seiyuuSearchCriteriaService;

		public SeiyuuService(IMapper mapper, ISeiyuuRepository seiyuuRepository, ISeiyuuSearchCriteriaService seiyuuSearchCriteriaService)
		{
			this.mapper = mapper;
			this.seiyuuRepository = seiyuuRepository;
			this.seiyuuSearchCriteriaService = seiyuuSearchCriteriaService;
		}

		public async Task<PagedResult<SeiyuuDto>> GetAsync(Query<SeiyuuSearchCriteria> query)
		{
			var expression = await seiyuuSearchCriteriaService.BuildExpression(query.SearchCriteria);

			var entities = await seiyuuRepository.GetOrderedPageAsync(expression, query.SortExpression, query.Page, query.PageSize);

			return mapper.Map<PagedResult<SeiyuuDto>>(entities);
		}
	}
}

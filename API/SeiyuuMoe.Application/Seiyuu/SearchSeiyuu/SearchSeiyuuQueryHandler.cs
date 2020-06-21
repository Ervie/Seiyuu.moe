using SeiyuuMoe.Application.Seiyuu.Extensions;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.WebEssentials;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.Application.Seiyuu.SearchSeiyuu
{
	public class SearchSeiyuuQueryHandler
	{
		private readonly ISeiyuuRepository _seiyuuRepository;
		private readonly ISeiyuuSearchCriteriaService _seiyuuSearchCriteriaService;

		public SearchSeiyuuQueryHandler(
			ISeiyuuRepository seiyuuRepository,
			ISeiyuuSearchCriteriaService seiyuuSearchCriteriaService
		)
		{
			_seiyuuRepository = seiyuuRepository;
			_seiyuuSearchCriteriaService = seiyuuSearchCriteriaService;
		}

		public async Task<QueryResponse<PagedResult<SeiyuuSearchEntryDto>>> HandleAsync(SearchSeiyuuQuery query)
		{
			var expression = _seiyuuSearchCriteriaService.BuildExpression(query);

			var entities = await _seiyuuRepository.GetOrderedPageAsync(expression);

			var result = entities.Map<Domain.Entities.Seiyuu, SeiyuuSearchEntryDto>(entities.Results.Select(x => x.ToSeiyuuSearchEntryDto()));

			return new QueryResponse<PagedResult<SeiyuuSearchEntryDto>>(result);
		}
	}
}
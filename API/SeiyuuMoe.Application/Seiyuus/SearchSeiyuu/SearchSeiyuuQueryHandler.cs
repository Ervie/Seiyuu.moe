using SeiyuuMoe.Application.Seiyuus.Extensions;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.WebEssentials;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.Application.Seiyuus.SearchSeiyuu
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

		public async Task<PagedResult<SeiyuuSearchEntryDto>> HandleAsync(SearchSeiyuuQuery query)
		{
			var expression = _seiyuuSearchCriteriaService.BuildExpression(query);

			var entities = await _seiyuuRepository.GetOrderedPageByPopularityAsync(expression);

			return entities.Map<Seiyuu, SeiyuuSearchEntryDto>(entities.Results.Select(x => x.ToSeiyuuSearchEntryDto()));
		}
	}
}
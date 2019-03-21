using EnsureThat;
using SeiyuuMoe.BusinessServices;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.Dtos.Other;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Repositories.Models;
using SeiyuuMoe.WebEssentials;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.Services
{
	public class SeiyuuService : ISeiyuuService
	{
		private readonly ISeiyuuBusinessService seiyuuBusinessService;

		public SeiyuuService(ISeiyuuBusinessService seiyuuBusinessService)
		{
			this.seiyuuBusinessService = seiyuuBusinessService;
		}

		public async Task<QueryResponse<SeiyuuCardDto>> GetSingleAsync(long id)
		{
			Ensure.That(id, nameof(id)).IsGte(1);

			var payload = await seiyuuBusinessService.GetSingleAsync(id);

			return new QueryResponse<SeiyuuCardDto>(payload);
		}

		public async Task<QueryResponse<PagedResult<SeiyuuSearchEntryDto>>> GetAsync(Query<SeiyuuSearchCriteria> query)
		{
			Ensure.That(query, nameof(query)).IsNotNull();

			var payload = await seiyuuBusinessService.GetAsync(query);

			return new QueryResponse<PagedResult<SeiyuuSearchEntryDto>>(payload);
		}

		public async Task<QueryResponse<ICollection<SeiyuuComparisonEntryDto>>> GetSeiyuuComparison(Query<RoleSearchCriteria> query)
		{
			Ensure.That(query, nameof(query)).IsNotNull();
			Ensure.That(query.SearchCriteria, nameof(query.SearchCriteria)).IsNotNull();
			Ensure.That(query.SearchCriteria.SeiyuuMalId, nameof(query.SearchCriteria.SeiyuuMalId)).IsNotNull();
			Ensure.That(query.SearchCriteria.SeiyuuMalId.Count).IsGte(2);

			return new QueryResponse<ICollection<SeiyuuComparisonEntryDto>>(await seiyuuBusinessService.GetSeiyuuComparison(query.SearchCriteria));
		}
	}
}
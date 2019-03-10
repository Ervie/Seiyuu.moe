using EnsureThat;
using SeiyuuMoe.BusinessServices;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Repositories.Models;
using SeiyuuMoe.WebEssentials;
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

		public async Task<QueryResponse<PagedResult<SeiyuuSearchEntryDto>>> GetAsync(Query<SeiyuuSearchCriteria> query)
		{
			Ensure.That(query, nameof(query)).IsNotNull();

			var payload = await seiyuuBusinessService.GetAsync(query);

			return new QueryResponse<PagedResult<SeiyuuSearchEntryDto>>(payload);
		}
	}
}
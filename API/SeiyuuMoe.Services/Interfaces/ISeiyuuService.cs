using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Repositories.Models;
using SeiyuuMoe.WebEssentials;
using System.Threading.Tasks;

namespace SeiyuuMoe.Services
{
	public interface ISeiyuuService
	{
		Task<QueryResponse<PagedResult<SeiyuuSearchEntryDto>>> GetAsync(Query<SeiyuuSearchCriteria> query);
	}
}

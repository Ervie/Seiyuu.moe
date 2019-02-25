using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Repositories.Models;
using SeiyuuMoe.WebEssentials;
using System.Threading.Tasks;

namespace SeiyuuMoe.Services
{
	public interface IAnimeService
	{
		Task<QueryResponse<PagedResult<AnimeDto>>> GetAsync(Query<AnimeSearchCriteria> query);

		Task<QueryResponse<PagedResult<AnimeAiringDto>>> GetDatesAsync(Query<AnimeSearchCriteria> query);
	}
}

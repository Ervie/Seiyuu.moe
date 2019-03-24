using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Repositories.Models;
using SeiyuuMoe.WebEssentials;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.Services
{
	public interface IAnimeService
	{
		Task<QueryResponse<AnimeCardDto>> GetSingleAsync(long id);

		Task<QueryResponse<PagedResult<AnimeSearchEntryDto>>> GetAsync(Query<AnimeSearchCriteria> query);

		Task<QueryResponse<PagedResult<AnimeAiringDto>>> GetDatesAsync(Query<AnimeSearchCriteria> query);

		Task<QueryResponse<ICollection<AnimeComparisonEntryDto>>> GetAnimeComparison(Query<AnimeComparisonSearchCriteria> searchCriteria);
	}
}
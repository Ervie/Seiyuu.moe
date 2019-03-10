using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Repositories.Models;
using SeiyuuMoe.WebEssentials;
using System.Threading.Tasks;

namespace SeiyuuMoe.BusinessServices
{
	public interface IAnimeBusinessService
	{
		Task<AnimeCardDto> GetSingleAsync(long id);

		Task<PagedResult<AnimeSearchEntryDto>> GetAsync(Query<AnimeSearchCriteria> query);

		Task<PagedResult<AnimeAiringDto>> GetDatesAsync(Query<AnimeSearchCriteria> query);
	}
}
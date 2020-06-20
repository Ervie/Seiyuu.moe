using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Domain.WebEssentials;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.BusinessServices
{
	public interface IAnimeBusinessService
	{
		Task<AnimeCardDto> GetSingleAsync(long id);

		Task<PagedResult<AnimeSearchEntryDto>> GetAsync(Query<AnimeSearchCriteria> query);

		Task<ICollection<AnimeComparisonEntryDto>> GetAnimeComparison(AnimeComparisonSearchCriteria searchCriteria);
	}
}
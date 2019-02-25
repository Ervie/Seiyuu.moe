using SeiyuuMoe.BusinessServices;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Repositories.Models;
using SeiyuuMoe.WebEssentials;
using System.Threading.Tasks;

namespace SeiyuuMoe.Services
{
	internal class AnimeService : IAnimeService
	{
		private readonly IAnimeBusinessService animeBusinessService;

		public AnimeService(IAnimeBusinessService animeBusinessService)
		{
			this.animeBusinessService = animeBusinessService;
		}

		public async Task<QueryResponse<PagedResult<AnimeDto>>> GetAsync(Query<AnimeSearchCriteria> query)
		{
			var payload = await animeBusinessService.GetAsync(query);

			return new QueryResponse<PagedResult<AnimeDto>>(payload);
		}

		public async Task<QueryResponse<PagedResult<AnimeAiringDto>>> GetDatesAsync(Query<AnimeSearchCriteria> query)
		{
			var payload = await animeBusinessService.GetDatesAsync(query);

			return new QueryResponse<PagedResult<AnimeAiringDto>>(payload);
		}
	}
}
using EnsureThat;
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

		public async Task<QueryResponse<PagedResult<AnimeSearchEntryDto>>> GetAsync(Query<AnimeSearchCriteria> query)
		{
			Ensure.That(query, nameof(query)).IsNotNull();

			var payload = await animeBusinessService.GetAsync(query);

			return new QueryResponse<PagedResult<AnimeSearchEntryDto>>(payload);
		}

		public async Task<QueryResponse<PagedResult<AnimeAiringDto>>> GetDatesAsync(Query<AnimeSearchCriteria> query)
		{
			Ensure.That(query, nameof(query)).IsNotNull();

			var payload = await animeBusinessService.GetDatesAsync(query);

			return new QueryResponse<PagedResult<AnimeAiringDto>>(payload);
		}

		public async Task<QueryResponse<AnimeCardDto>> GetSingleAsync(long id)
		{
			Ensure.That(id, nameof(id)).IsGte(1);

			var payload = await animeBusinessService.GetSingleAsync(id);

			return new QueryResponse<AnimeCardDto>(payload);
		}
	}
}
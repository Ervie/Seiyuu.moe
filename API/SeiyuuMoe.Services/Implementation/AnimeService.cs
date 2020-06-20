using EnsureThat;
using SeiyuuMoe.BusinessServices;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Domain.WebEssentials;
using System.Collections.Generic;
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

		public async Task<QueryResponse<AnimeCardDto>> GetSingleAsync(long id)
		{
			Ensure.That(id, nameof(id)).IsGte(1);

			var payload = await animeBusinessService.GetSingleAsync(id);

			return new QueryResponse<AnimeCardDto>(payload);
		}

		public async Task<QueryResponse<ICollection<AnimeComparisonEntryDto>>> GetAnimeComparison(Query<AnimeComparisonSearchCriteria> query)
		{
			Ensure.That(query, nameof(query)).IsNotNull();
			Ensure.That(query.SearchCriteria, nameof(query.SearchCriteria)).IsNotNull();
			Ensure.That(query.SearchCriteria.AnimeMalId, nameof(query.SearchCriteria.AnimeMalId)).IsNotNull();
			Ensure.That(query.SearchCriteria.AnimeMalId.Count).IsGte(2);

			return new QueryResponse<ICollection<AnimeComparisonEntryDto>>(await animeBusinessService.GetAnimeComparison(query.SearchCriteria));
		}
	}
}
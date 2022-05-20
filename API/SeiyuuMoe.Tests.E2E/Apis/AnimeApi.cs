using RestEase;
using SeiyuuMoe.AnimeComparisons;
using SeiyuuMoe.Animes;
using SeiyuuMoe.Application.AnimeComparisons.CompareAnime;
using SeiyuuMoe.Application.Animes;
using SeiyuuMoe.Application.Animes.SearchAnime;
using SeiyuuMoe.Domain.WebEssentials;
using System.Collections.Generic;
using System.Threading.Tasks;
using SeiyuuMoe.Tests.E2E.Apis.Interfaces;

namespace SeiyuuMoe.Tests.E2E.Apis
{
	public class AnimeApi
	{
		private readonly IAnimeApi _animeApi;

		public AnimeApi(IRequester requester)
		{
			_animeApi = RestClient.For<IAnimeApi>(requester);
		}

		public Task<AnimeCardDto> GetCardInfoAsync(int animeId) => _animeApi.GetCardInfoAsync(animeId);

		public Task<PagedResult<AnimeSearchEntryDto>> GetSearchEntriesAsync(SearchAnimeQuery query) => _animeApi.GetSearchEntriesAsync(query.Title);

		public Task<ICollection<AnimeComparisonEntryDto>> GetComparison(CompareAnimeQuery query) => _animeApi.GetComparison(query.AnimeMalIds);
	}
}
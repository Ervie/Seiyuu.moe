using RestEase;
using SeiyuuMoe.Animes;
using SeiyuuMoe.Application.Animes;
using SeiyuuMoe.Application.Animes.SearchAnime;
using SeiyuuMoe.Domain.WebEssentials;
using System.Threading.Tasks;

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
	}
}
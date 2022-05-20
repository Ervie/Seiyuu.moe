using RestEase;
using SeiyuuMoe.Domain.WebEssentials;
using System.Collections.Generic;
using System.Threading.Tasks;
using SeiyuuMoe.Application.SeiyuuComparisons;
using SeiyuuMoe.Application.SeiyuuComparisons.CompareSeiyuu;
using SeiyuuMoe.Application.Seiyuus;
using SeiyuuMoe.Application.Seiyuus.SearchSeiyuu;
using SeiyuuMoe.Tests.E2E.Apis.Interfaces;

namespace SeiyuuMoe.Tests.E2E.Apis
{
	public class SeiyuuApi
	{
		private readonly ISeiyuuApi _seiyuuApi;

		public SeiyuuApi(IRequester requester)
		{
			_seiyuuApi = RestClient.For<ISeiyuuApi>(requester);
		}

		public Task<SeiyuuCardDto> GetCardInfoAsync(int animeId) => _seiyuuApi.GetCardInfoAsync(animeId);

		public Task<PagedResult<SeiyuuSearchEntryDto>> GetSearchEntriesAsync(SearchSeiyuuQuery query) => _seiyuuApi.GetSearchEntriesAsync(query.Name);

		public Task<ICollection<SeiyuuComparisonEntryDto>> GetComparison(CompareSeiyuuQuery query) => _seiyuuApi.GetComparison(query.SeiyuuMalIds);
	}
}
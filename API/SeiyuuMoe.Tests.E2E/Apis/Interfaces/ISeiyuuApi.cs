using RestEase;
using SeiyuuMoe.Domain.WebEssentials;
using System.Collections.Generic;
using System.Threading.Tasks;
using SeiyuuMoe.Application.SeiyuuComparisons;
using SeiyuuMoe.Application.Seiyuus;

namespace SeiyuuMoe.Tests.E2E.Apis.Interfaces
{
	[BasePath("seiyuu")]
	public interface ISeiyuuApi
	{
		[Get("{id}")]
		public Task<SeiyuuCardDto> GetCardInfoAsync([Path] long id);

		[Get]
		public Task<PagedResult<SeiyuuSearchEntryDto>> GetSearchEntriesAsync([Query("Name")] string title);

		[Get("Compare")]
		public Task<ICollection<SeiyuuComparisonEntryDto>> GetComparison([Query("SeiyuuMalIds")] ICollection<long> seiyuuMalIds);
	}
}
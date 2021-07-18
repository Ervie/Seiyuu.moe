using RestEase;
using SeiyuuMoe.AnimeComparisons;
using SeiyuuMoe.Animes;
using SeiyuuMoe.Application.Animes;
using SeiyuuMoe.Domain.WebEssentials;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.Tests.E2E.Apis
{
	[BasePath("anime")]
	public interface IAnimeApi
	{
		[Get("{id}")]
		public Task<AnimeCardDto> GetCardInfoAsync([Path] long id);

		[Get]
		public Task<PagedResult<AnimeSearchEntryDto>> GetSearchEntriesAsync([Query("Title")] string title);

		[Get("Compare")]
		public Task<ICollection<AnimeComparisonEntryDto>> GetComparison([Query("AnimeMalIds")] ICollection<long> animeMalIds);
	}
}
using System.Threading.Tasks;
using RestEase;
using SeiyuuMoe.Application.Seasons;
using SeiyuuMoe.Domain.WebEssentials;

namespace SeiyuuMoe.Tests.E2E.Apis.Interfaces
{
    [BasePath("season")]
    public interface ISeasonApi
    {
        [Get("Summary")]
        public Task<PagedResult<SeasonSummaryEntryDto>> GetSeasonSummaryAsync([Query("Year")] long year, [Query("Season")] string season, [Query("PageSize")] int pageSize);
    }
}
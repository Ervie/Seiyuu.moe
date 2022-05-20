using System.Threading.Tasks;
using RestEase;
using SeiyuuMoe.Application.Seasons;
using SeiyuuMoe.Application.Seasons.GetSeasonSummaries;
using SeiyuuMoe.Domain.WebEssentials;
using SeiyuuMoe.Tests.E2E.Apis.Interfaces;

namespace SeiyuuMoe.Tests.E2E.Apis;

public class SeasonApi
{
    private readonly ISeasonApi _seasonApi;

    public SeasonApi(IRequester requester)
    {
        _seasonApi = RestClient.For<ISeasonApi>(requester);
    }

    public Task<PagedResult<SeasonSummaryEntryDto>> GetSeasonSummaryAsync(GetSeasonSummariesQuery query) => _seasonApi.GetSeasonSummaryAsync(query.Year, query.Season, query.PageSize);
}
using System.Threading.Tasks;
using FluentAssertions;
using SeiyuuMoe.Application.Seasons.GetSeasonSummaries;
using SeiyuuMoe.Tests.E2E.Apis;
using Xunit;

namespace SeiyuuMoe.Tests.E2E.Tests;

public class SeasonControllerTests: BaseSetupClass
{
    [Fact]
    public async Task GetSeasonSummary_Winter2000_ShouldReturnSummary()
    {
        // Given
        var api = new SeasonApi(requester);
        var query = new GetSeasonSummariesQuery {Year = 2000, Season = "Winter", PageSize = 10};

        // When
        var apiResult = await api.GetSeasonSummaryAsync(query);

        // Then
        apiResult.TotalCount.Should().Be(231);
    }
}
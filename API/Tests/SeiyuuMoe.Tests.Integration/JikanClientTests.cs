using FluentAssertions;
using FluentAssertions.Execution;
using JikanDotNet;
using Xunit;

namespace SeiyuuMoe.Tests.Integration;

public class JikanClientTests
{
    private readonly IJikan _jikanClient;

    public JikanClientTests()
    {
        _jikanClient = new Jikan();
    }
    
    [Fact]
    public async Task GetAnimeAsync_GivenBebopId_ShouldParseRequiredData()
    {
        // Given
        const int bebopMalId = 1;

        // When
        var result = await _jikanClient.GetAnimeAsync(bebopMalId);

        // Then
        using var _ = new AssertionScope();
        result.Data.Title.Should().Be("Cowboy Bebop");
        result.Data.Synopsis.Should().StartWith("Crime is timeless.");
        result.Data.TitleEnglish.Should().Be("Cowboy Bebop");
        result.Data.TitleJapanese.Should().Be("カウボーイビバップ");
        result.Data.TitleSynonyms.Should().BeEmpty();
        result.Data.Members.Should().BeGreaterThan(1500000);
        result.Data.Images.JPG.ImageUrl.Should().NotBeNullOrWhiteSpace();
        result.Data.Aired.From.Should().NotBeNull();
        result.Data.Type.Should().Be("TV");
        result.Data.Status.Should().Be("Finished Airing");
        result.Data.Season.Should().Be(Season.Spring);
        result.Data.Year.Should().Be(1998);
    }
    
    [Fact]
    public async Task GetCharacterAsync_GivenSpikeId_ShouldParseRequiredData()
    {
        // Given
        const int spikeMalId = 1;

        // When
        var result = await _jikanClient.GetCharacterAsync(spikeMalId);

        // Then
        using var _ = new AssertionScope();
        result.Data.Name.Should().Be("Spike Spiegel");
        result.Data.About.Should().StartWith("Birthdate: June 26, 2044");
        result.Data.NameKanji.Should().Be("スパイク・スピーゲル");
        result.Data.Images.JPG.ImageUrl.Should().NotBeNullOrWhiteSpace();
        result.Data.Nicknames.Should().BeEmpty();
        result.Data.Favorites.Should().BeGreaterThan(40000);
    }
    
    [Fact]
    public async Task GetSeasonArchiveAsync_ShouldParseRequiredData()
    {
        // When
        var result = await _jikanClient.GetSeasonArchiveAsync();

        // Then
        result.Data.Should().NotBeNullOrEmpty();
    }
    
    [Fact]
    public async Task GetPersonAsync_GivenSeki_ShouldParseRequiredData()
    {
        // Given
        const int sekiMalId = 1;

        // When
        var result = await _jikanClient.GetPersonAsync(sekiMalId);

        // Then
        using var _ = new AssertionScope();
        result.Data.Name.Should().Be("Tomokazu Seki");
        result.Data.About.Should().StartWith("Hometown: Tokyo, Japan");
        result.Data.GivenName.Should().Be("智一");
        result.Data.FamilyName.Should().Be("関");
        result.Data.Images.JPG.ImageUrl.Should().NotBeNullOrWhiteSpace();
        result.Data.AlternativeNames.Should().HaveCount(3).And.Contain("門戸 開");
        result.Data.MemberFavorites.Should().BeGreaterThan(5000);
    }
    
    [Fact]
    public async Task GetPersonVoiceActingRolesAsync_GivenSeki_ShouldParseRequiredData()
    {
        // Given
        const int sekiMalId = 1;

        // When
        var result = await _jikanClient.GetPersonVoiceActingRolesAsync(sekiMalId);

        // Then
        result.Data.Should().HaveCountGreaterThan(400);
    }
}
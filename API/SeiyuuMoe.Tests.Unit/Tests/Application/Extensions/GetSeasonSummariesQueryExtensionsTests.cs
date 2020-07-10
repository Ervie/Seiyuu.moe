using FluentAssertions;
using FluentAssertions.Execution;
using SeiyuuMoe.Application.Seasons.Extensions;
using SeiyuuMoe.Domain.Enums;
using SeiyuuMoe.Tests.Unit.Builders.Queries;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tests.Application.Extensions
{
	public class GetSeasonSummariesQueryExtensionsTests
	{
		[Fact]
		public void ToSearchAnimeQuery_GivenEmptyQuery_ShouldReturnQueryWithDefaultValues()
		{
			// Given
			var query = new GetSeasonSummariesQueryBuilder().Build();

			// When
			var searchAnimeQuery = query.ToSearchAnimeQuery();

			// Then
			using (new AssertionScope())
			{
				searchAnimeQuery.Should().NotBeNull();
				searchAnimeQuery.MalId.Should().BeNull();
				searchAnimeQuery.AnimeTypeId.Should().Be(default);
				searchAnimeQuery.SeasonId.Should().Be(default);
			}
		}

		[Fact]
		public void ToSearchAnimeQuery_GivenSeasonId_ShouldReturnQueryWithMappedSeasonId()
		{
			// Given
			const long seasonId = 1;
			var query = new GetSeasonSummariesQueryBuilder()
				.WithId(seasonId)
				.Build();

			// When
			var searchAnimeQuery = query.ToSearchAnimeQuery();

			// Then
			using (new AssertionScope())
			{
				searchAnimeQuery.Should().NotBeNull();
				searchAnimeQuery.SeasonId.Should().Be(seasonId);
			}
		}

		[Theory]
		[InlineData(false, AnimeTypeDictionary.AllTypes)]
		[InlineData(true, AnimeTypeDictionary.TV)]
		public void ToSearchAnimeQuery_GivenTvSeriesOnly_ShouldReturnQueryWithMappedAnimeType(bool tvSeriesOnly, AnimeTypeDictionary expectedAnimeType)
		{
			// Given
			var query = new GetSeasonSummariesQueryBuilder()
				.WithTvSeriesOnly(tvSeriesOnly)
				.Build();

			// When
			var searchAnimeQuery = query.ToSearchAnimeQuery();

			// Then
			using (new AssertionScope())
			{
				searchAnimeQuery.Should().NotBeNull();
				searchAnimeQuery.AnimeTypeId.Should().Be((int)expectedAnimeType);
			}
		}
	}
}
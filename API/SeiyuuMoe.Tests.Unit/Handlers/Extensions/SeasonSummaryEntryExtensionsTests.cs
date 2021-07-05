using FluentAssertions;
using FluentAssertions.Execution;
using SeiyuuMoe.Application.Seasons.Extensions;
using SeiyuuMoe.Tests.Common.Builders.ComparisonEntities;
using System;
using System.Linq;
using Xunit;

namespace SeiyuuMoe.Tests.Application.Extensions
{
	public class SeasonSummaryEntryExtensionsTests
	{
		[Fact]
		public void ToSeasonSummaryEntryDto_GivenEmptySeasonSummaryEntry_ShouldReturnEmptySeasonSummaryEntry()
		{
			// Given
			var seasonSummaryEntry = new SeasonSummaryEntryBuilder().Build();

			// When
			var dto = seasonSummaryEntry.ToSeasonSummaryEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();

				dto.Seiyuu.Should().BeNull();
				dto.AnimeCharacterPairs.Should().ContainSingle();
				dto.AnimeCharacterPairs.First().Item1.Should().BeNull();
				dto.AnimeCharacterPairs.First().Item2.Should().BeNull();
			}
		}

		[Fact]
		public void ToSeasonSummaryEntryDto_GivenSeasonSummaryEntryWithEntites_ShouldReturnSeasonSummaryEntryWithEntities()
		{
			// Given
			var seasonSummaryEntry = new SeasonSummaryEntryBuilder()
				.WithSeiyuu(x => x.Build())
				.WithAnime(x => x.Build())
				.WithCharacter(x => x.Build())
				.Build();

			// When
			var dto = seasonSummaryEntry.ToSeasonSummaryEntryDto();

			// Then

			using (new AssertionScope())
			{
				dto.Should().NotBeNull();

				dto.Seiyuu.Should().NotBeNull();
				dto.AnimeCharacterPairs.Should().ContainSingle();
				dto.AnimeCharacterPairs.First().Item1.Should().NotBeNull();
				dto.AnimeCharacterPairs.First().Item2.Should().NotBeNull();
			}
		}

		[Fact]
		public void ToSeasonSummaryEntryDto_GivenSeasonSummaryEntryWithFullData_ShouldReturnSeasonSummaryEntryWithFullData()
		{
			// Given
			const string expectedTitle = "ExpectedTitle";
			const string expectedAnimeImageUrl = "ExpectedAnimeImageUrl";
			const long expectedAnimeMalId = 2;
			const string expectedAnimeUrl = "https://myanimelist.net/anime/2";

			const string expectedSeiyuuName = "ExpectedSeiyuuName";
			const string expectedSeiyuuImageUrl = "ExpectedSeiyuuImageUrl";
			const long expectedSeiyuuMalId = 1;
			const string expectedSeiyuuUrl = "https://myanimelist.net/people/1";

			const string expectedCharacterName = "ExpectedCharacterName";
			const string expectedCharacterImageUrl = "ExpectedCharacterImageUrl";
			const long expectedCharacterMalId = 1;
			const string expectedCharacterUrl = "https://myanimelist.net/character/1";

			var seasonSummaryEntry = new SeasonSummaryEntryBuilder()
				.WithSeiyuu(x => x.WithName(expectedSeiyuuName)
					.WithImageUrl(expectedSeiyuuImageUrl)
					.WithMalId(expectedSeiyuuMalId)
					.Build())
				.WithAnime(x => x
					.WithMalId(expectedAnimeMalId)
					.WithTitle(expectedTitle)
					.WithImageUrl(expectedAnimeImageUrl)
					.WithAiringDate(new DateTime(1990, 1, 1)))
				.WithCharacter(x => x.WithName(expectedCharacterName)
					.WithMalId(expectedCharacterMalId)
					.WithImageUrl(expectedCharacterImageUrl)
					.Build())
				.Build();

			// When
			var dto = seasonSummaryEntry.ToSeasonSummaryEntryDto();

			// Then

			var subEntry = dto.AnimeCharacterPairs.FirstOrDefault();
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();

				dto.Seiyuu.Should().NotBeNull();
				dto.AnimeCharacterPairs.Should().ContainSingle();

				dto.Seiyuu.Should().NotBeNull();
				dto.Seiyuu.MalId.Should().Be(expectedSeiyuuMalId);
				dto.Seiyuu.ImageUrl.Should().Be(expectedSeiyuuImageUrl);
				dto.Seiyuu.Url.Should().Be(expectedSeiyuuUrl);
				dto.Seiyuu.Name.Should().Be(expectedSeiyuuName);

				subEntry.Item1.MalId.Should().Be(expectedAnimeMalId);
				subEntry.Item1.Title.Should().Be(expectedTitle);
				subEntry.Item1.Url.Should().Be(expectedAnimeUrl);
				subEntry.Item1.ImageUrl.Should().Be(expectedAnimeImageUrl);
				subEntry.Item1.AiringFrom.Should().NotBeNull();

				subEntry.Item2.MalId.Should().Be(expectedCharacterMalId);
				subEntry.Item2.Name.Should().Be(expectedCharacterName);
				subEntry.Item2.Url.Should().Be(expectedCharacterUrl);
				subEntry.Item2.ImageUrl.Should().Be(expectedCharacterImageUrl);
			}
		}
	}
}
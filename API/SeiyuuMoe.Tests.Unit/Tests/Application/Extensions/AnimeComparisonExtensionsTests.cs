using FluentAssertions;
using FluentAssertions.Execution;
using SeiyuuMoe.Application.AnimeComparisons.Extensions;
using SeiyuuMoe.Domain.ComparisonEntities;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Tests.Unit.Builders.ComparisonEntities;
using SeiyuuMoe.Tests.Unit.Builders.Model;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tests.Application.Extensions
{
	public class AnimeComparisonExtensionsTests
	{
		[Fact]
		public void ToAnimeComparisonEntryDto_GivenAnimeComparisonEntryWithNoSeiyuu_ShouldReturnEmptyAnimeComparisonEntryDto()
		{
			// Given
			var animeComparison = new AnimeComparisonEntryBuilder().Build();

			// When
			var dto = animeComparison.ToAnimeComparisonEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.AnimeCharacters.Should().BeEmpty();
				dto.Seiyuu.Should().BeNull();
			}
		}

		[Fact]
		public void ToAnimeComparisonEntryDto_GivenEmptyAnimeComparisonEntryWithSeiyuu_ShouldReturnAnimeComparisonEntryDtoWithSeiyuu()
		{
			// Given
			var animeComparison = new AnimeComparisonEntryBuilder()
				.WithSeiyuu(x => x.Build())
				.Build();

			// When
			var dto = animeComparison.ToAnimeComparisonEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.AnimeCharacters.Should().BeEmpty();
				dto.Seiyuu.Should().NotBeNull();
			}
		}

		[Fact]
		public void ToAnimeComparisonEntryDto_GivenEmptyAnimeComparisonEntryWithSubentry_ShouldReturnEmptyAnimeComparisonEntryDtoWithSubentry()
		{
			// Given
			var animeCharacterPairs = new List<AnimeComparisonSubEntry>
			{
				new AnimeComparisonSubEntryBuilder().Build()
			};

			var animeComparison = new AnimeComparisonEntryBuilder()
				.WithSubentries(animeCharacterPairs)
				.Build();

			// When
			var dto = animeComparison.ToAnimeComparisonEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.AnimeCharacters.Should().ContainSingle();
				dto.Seiyuu.Should().BeNull();
			}
		}

		[Fact]
		public void ToAnimeComparisonEntryDto_GivenAnimeComparisonEntryWithSingleSubEntry_ShouldReturnAnimeComparisonEntryDtoWithSubEntry()
		{
			// Given
			var animeCharacterPairs = new List<AnimeComparisonSubEntry>
			{
				new AnimeComparisonSubEntryBuilder().Build()
			};

			var animeComparison = new AnimeComparisonEntryBuilder()
				.WithSeiyuu(x => x.Build())
				.WithSubentries(animeCharacterPairs)
				.Build();

			// When
			var dto = animeComparison.ToAnimeComparisonEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.AnimeCharacters.Should().ContainSingle();
				dto.Seiyuu.Should().NotBeNull();
			}
		}

		[Fact]
		public void ToAnimeComparisonEntryDto_GivenAnimeComparisonEntryWithMultipleSubEntries_ShouldReturnAnimeComparisonEntryDtoWithMultipleSubEntries()
		{
			// Given
			var animeCharacterPairs = new List<AnimeComparisonSubEntry>
			{
				new AnimeComparisonSubEntryBuilder().Build(),
				new AnimeComparisonSubEntryBuilder().Build(),
				new AnimeComparisonSubEntryBuilder().Build()
			};

			var animeComparison = new AnimeComparisonEntryBuilder()
				.WithSeiyuu(x => x.Build())
				.WithSubentries(animeCharacterPairs)
				.Build();

			// When
			var dto = animeComparison.ToAnimeComparisonEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.AnimeCharacters.Should().HaveCount(3);
				dto.Seiyuu.Should().NotBeNull();
			}
		}

		[Fact]
		public void ToAnimeComparisonEntryDto_GivenFullDataAnimeComparisonEntry_ShouldReturnAnimeComparisonEntryDtoWithAllData()
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

			var character = new CharacterBuilder()
				.WithName(expectedCharacterName)
				.WithMalId(expectedCharacterMalId)
				.WithImageUrl(expectedCharacterImageUrl)
				.Build();

			var animeCharacterPairs = new List<AnimeComparisonSubEntry>
			{
				new AnimeComparisonSubEntryBuilder()
					.WithAnime(x => x
						.WithMalId(expectedAnimeMalId)
						.WithTitle(expectedTitle)
						.WithImageUrl(expectedAnimeImageUrl)
						.WithAiringDate("10-10-1990")
					)
					.WithCharacters(new List<Character> { character })
					.Build()
			};

			var animeComparison = new AnimeComparisonEntryBuilder()
				.WithSeiyuu(
					x => x
					.WithName(expectedSeiyuuName)
					.WithImageUrl(expectedSeiyuuImageUrl)
					.WithMalId(expectedSeiyuuMalId)
					.Build()
				)
				.WithSubentries(animeCharacterPairs)
				.Build();

			// When
			var dto = animeComparison.ToAnimeComparisonEntryDto();

			var subEntry = dto.AnimeCharacters.FirstOrDefault();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();

				dto.Seiyuu.Should().NotBeNull();
				dto.Seiyuu.MalId.Should().Be(expectedSeiyuuMalId);
				dto.Seiyuu.ImageUrl.Should().Be(expectedSeiyuuImageUrl);
				dto.Seiyuu.Url.Should().Be(expectedSeiyuuUrl);
				dto.Seiyuu.Name.Should().Be(expectedSeiyuuName);

				dto.AnimeCharacters.Should().ContainSingle();

				subEntry.Anime.MalId.Should().Be(expectedAnimeMalId);
				subEntry.Anime.Title.Should().Be(expectedTitle);
				subEntry.Anime.Url.Should().Be(expectedAnimeUrl);
				subEntry.Anime.ImageUrl.Should().Be(expectedAnimeImageUrl);
				subEntry.Anime.AiringFrom.Should().NotBeNull();

				subEntry.Characters.First().MalId.Should().Be(expectedCharacterMalId);
				subEntry.Characters.First().Name.Should().Be(expectedCharacterName);
				subEntry.Characters.First().Url.Should().Be(expectedCharacterUrl);
				subEntry.Characters.First().ImageUrl.Should().Be(expectedCharacterImageUrl);
			}
		}
	}
}
using FluentAssertions;
using FluentAssertions.Execution;
using SeiyuuMoe.Application.SeiyuuComparisons.Extensions;
using SeiyuuMoe.Domain.ComparisonEntities;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Tests.Unit.Builders.ComparisonEntities;
using SeiyuuMoe.Tests.Unit.Builders.Model;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tests.Application.Extensions
{
	public class SeiyuuComparisonExtensionsTests
	{
		[Fact]
		public void ToSeiyuuComparisonEntryDto_GivenSeiyuuComparisonEntryWithNoAnime_ShouldReturnSeiyuuAnimeComparisonEntryDto()
		{
			// Given
			var seiyuuComparison = new SeiyuuComparisonEntryBuilder().Build();

			// When
			var dto = seiyuuComparison.ToSeiyuuComparisonEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.Anime.Should().BeEmpty();
				dto.SeiyuuCharacters.Should().BeEmpty();
			}
		}

		[Fact]
		public void ToSeiyuuComparisonEntryDto_GivenSeiyuuComparisonEntryWithMultipleAnime_ShouldReturnSeiyuuComparisonEntryDtoWithMultipleAnime()
		{
			// Given
			var anime = new List<Anime>
			{
				new AnimeBuilder().Build(),
				new AnimeBuilder().Build(),
				new AnimeBuilder().Build()
			};

			var seiyuuComparison = new SeiyuuComparisonEntryBuilder()
				.WithAnime(anime)
				.Build();

			// When
			var dto = seiyuuComparison.ToSeiyuuComparisonEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.Anime.Should().HaveCount(3);
				dto.SeiyuuCharacters.Should().NotBeNull().And.BeEmpty();
			}
		}

		[Fact]
		public void ToSeiyuuComparisonEntryDto_GivenSeiyuuComparisonEntryWithAnime_ShouldReturnSeiyuuComparisonEntryDtoWithAnime()
		{
			// Given
			var anime = new List<Anime>
			{
				new AnimeBuilder().Build()
			};

			var seiyuuComparison = new SeiyuuComparisonEntryBuilder()
				.WithAnime(anime)
				.Build();

			// When
			var dto = seiyuuComparison.ToSeiyuuComparisonEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.Anime.Should().ContainSingle();
				dto.SeiyuuCharacters.Should().NotBeNull().And.BeEmpty();
			}
		}

		[Fact]
		public void ToSeiyuuComparisonEntryDto_GivenSeiyuuComparisonEntryWithSubEntry_ShouldReturnSeiyuuComparisonEntryDtoWithCharacterSeiyuuPair()
		{
			// Given
			var seiyuuCharacterPairs = new List<SeiyuuComparisonSubEntry>
			{
				new SeiyuuComparisonSubEntryBuilder().Build()
			};

			var seiyuuComparison = new SeiyuuComparisonEntryBuilder()
				.WithSubentries(seiyuuCharacterPairs)
				.Build();

			// When
			var dto = seiyuuComparison.ToSeiyuuComparisonEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.Anime.Should().NotBeNull().And.BeEmpty();
				dto.SeiyuuCharacters.Should().NotBeNull().And.ContainSingle();
			}
		}

		[Fact]
		public void ToSeiyuuComparisonEntryDto_GivenSeiyuuComparisonEntryWithMultipleSubEntries_ShouldReturnSeiyuuComparisonEntryDtoWithMultipleCharacterSeiyuuPair()
		{
			// Given
			var seiyuuCharacterPairs = new List<SeiyuuComparisonSubEntry>
			{
				new SeiyuuComparisonSubEntryBuilder().Build(),
				new SeiyuuComparisonSubEntryBuilder().Build(),
				new SeiyuuComparisonSubEntryBuilder().Build()
			};

			var seiyuuComparison = new SeiyuuComparisonEntryBuilder()
				.WithSubentries(seiyuuCharacterPairs)
				.Build();

			// When
			var dto = seiyuuComparison.ToSeiyuuComparisonEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.Anime.Should().NotBeNull().And.BeEmpty();
				dto.SeiyuuCharacters.Should().NotBeNull().And.HaveCount(3);
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

			var seiyuuCharacterPairs = new List<SeiyuuComparisonSubEntry>
			{
				new SeiyuuComparisonSubEntryBuilder()
					.WithSeiyuu(x => x
						.WithMalId(expectedSeiyuuMalId)
						.WithName(expectedSeiyuuName)
						.WithImageUrl(expectedSeiyuuImageUrl)
						.WithBirthday("10-10-1970")
					)
					.WithCharacter(x => x
						.WithName(expectedCharacterName)
						.WithMalId(expectedCharacterMalId)
						.WithImageUrl(expectedCharacterImageUrl)
						.Build())
					.Build()
			};

			var anime = new List<Anime>
			{
				new AnimeBuilder()
					.WithTitle(expectedTitle)
					.WithImageUrl(expectedAnimeImageUrl)
					.WithMalId(expectedAnimeMalId)
					.WithAiringDate("10-10-1990")
					.Build()
			};

			var seiyuuComparison = new SeiyuuComparisonEntryBuilder()
				.WithAnime(anime)
				.WithSubentries(seiyuuCharacterPairs)
				.Build();

			// When
			var dto = seiyuuComparison.ToSeiyuuComparisonEntryDto();

			var subEntry = dto.SeiyuuCharacters.FirstOrDefault();
			var animeEntry = dto.Anime.FirstOrDefault();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();

				animeEntry.MalId.Should().Be(expectedAnimeMalId);
				animeEntry.Title.Should().Be(expectedTitle);
				animeEntry.Url.Should().Be(expectedAnimeUrl);
				animeEntry.ImageUrl.Should().Be(expectedAnimeImageUrl);
				animeEntry.AiringFrom.Should().NotBeNull();

				dto.SeiyuuCharacters.Should().ContainSingle();

				subEntry.Seiyuu.Should().NotBeNull();
				subEntry.Seiyuu.MalId.Should().Be(expectedSeiyuuMalId);
				subEntry.Seiyuu.ImageUrl.Should().Be(expectedSeiyuuImageUrl);
				subEntry.Seiyuu.Url.Should().Be(expectedSeiyuuUrl);
				subEntry.Seiyuu.Name.Should().Be(expectedSeiyuuName);

				subEntry.Characters.First().MalId.Should().Be(expectedCharacterMalId);
				subEntry.Characters.First().Name.Should().Be(expectedCharacterName);
				subEntry.Characters.First().Url.Should().Be(expectedCharacterUrl);
				subEntry.Characters.First().ImageUrl.Should().Be(expectedCharacterImageUrl);
			}
		}
	}
}
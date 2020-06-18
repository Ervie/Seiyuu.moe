﻿using FluentAssertions;
using FluentAssertions.Execution;
using SeiyuuMoe.BusinessServices.Extensions;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Tests.Unit.Builders.Model;
using System;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tests
{
	public class AnimeExtensionsTests
	{
		[Fact]
		public void ToAnimeSearchEntryDto_GivenEmptyAnime_ShouldReturnEmptyAnimeSearchEntryDto()
		{
			// Given
			var anime = new AnimeBuilder().Build();

			// When
			var dto = anime.ToAnimeSearchEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.ImageUrl.Should().BeEmpty();
				dto.Title.Should().BeEmpty();
				dto.MalId.Should().Be(default);
			}
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("Naruto")]
		[InlineData("This is very pecular")]
		[InlineData("Zażółć gęślą jaźń")]
		[InlineData("\n\n \t        \t\n")]
		public void ToAnimeSearchEntryDto_GivenVariousTitleAnime_ShouldReturnEmptyAnimeSearchEntryDtoWithTitle(string title)
		{
			// Given
			var anime = new AnimeBuilder()
				.WithTitle(title)
				.Build();

			// When
			var dto = anime.ToAnimeSearchEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.Title.Should().Be(title);
			}
		}

		[Fact]
		public void ToAnimeSearchEntryDto_GivenAnimeWithValues_ShouldReturnEmptyAnimeSearchEntryDtoWithValues()
		{
			// Given
			const string expectedTitle = "ExpectedTitle";
			const string expectedImageUrl = "ExpectedImageUrl";
			const long expectedMalId = 1;

			var anime = new AnimeBuilder()
				.WithTitle(expectedTitle)
				.WithImageUrl(expectedImageUrl)
				.WithMalId(expectedMalId)
				.Build();

			// When
			var dto = anime.ToAnimeSearchEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.Title.Should().Be(expectedTitle);
				dto.ImageUrl.Should().Be(expectedImageUrl);
				dto.MalId.Should().Be(expectedMalId);
			}
		}

		[Fact]
		public void ToAnimeCardDto_GivenEmptyAnime_ShouldReturnEmptyAnimeCardDto()
		{
			// Given
			var anime = new AnimeBuilder().Build();

			// When
			var dto = anime.ToAnimeCardDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.ImageUrl.Should().BeEmpty();
				dto.Title.Should().BeEmpty();
				dto.MalId.Should().Be(default);
				dto.JapaneseTitle.Should().BeEmpty();
				dto.TitleSynonyms.Should().BeEmpty();
				dto.About.Should().BeEmpty();
				dto.Status.Should().BeEmpty();
				dto.Type.Should().BeEmpty();
				dto.Season.Should().BeEmpty();
			}
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("Naruto")]
		[InlineData("This is very pecular")]
		[InlineData("Zażółć gęślą jaźń")]
		[InlineData("\n\n \t        \t\n")]
		public void ToAnimeCardDto_GivenVariousTitleAnime_ShouldReturnEmptyAnimeCardDtoWithTitle(string title)
		{
			// Given
			var anime = new AnimeBuilder()
				.WithTitle(title)
				.Build();

			// When
			var dto = anime.ToAnimeCardDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.Title.Should().Be(title);
			}
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("\n\n \t        \t\n")]
		public void ToAnimeCardDto_GivenAnimeWithNullOrWhitespaceAiringDate_ShouldReturnAnimeCardDtoWithNullAiringDate(string date)
		{
			// Given

			var anime = new AnimeBuilder()
				.WithAiringDate(date)
				.Build();

			// When
			var dto = anime.ToAnimeCardDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.AiringDate.Should().BeNull();
			}
		}

		[Theory]
		[InlineData("01-01-0001")]
		[InlineData("01-01-1999")]
		[InlineData("01-01-2000")]
		[InlineData("10-10-2020")]
		[InlineData("31-12-2020")]
		[InlineData("29-02-2020")]
		public void ToAnimeCardDto_GivenAnimeWithValidAiringDate_ShouldReturnAnimeCardDtoWithAiringDate(string date)
		{
			// Given

			var anime = new AnimeBuilder()
				.WithAiringDate(date)
				.Build();

			// When
			var dto = anime.ToAnimeCardDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.AiringDate.Should().NotBeNull();
			}
		}

		[Theory]
		[InlineData("Naruto")]
		[InlineData("This is very pecular")]
		[InlineData("Zażółć gęślą jaźń")]
		[InlineData("01/01/1999")]
		[InlineData("1th sept 1999")]
		public void ToAnimeCardDto_GivenAnimeWithInvalidAiringDate_ShouldThrowException(string date)
		{
			// Given

			var anime = new AnimeBuilder()
				.WithAiringDate(date)
				.Build();

			// When
			Func<AnimeCardDto> func = () => anime.ToAnimeCardDto();

			// Then
			func.Should().Throw<Exception>();
		}

		[Fact]
		public void ToAnimeCardDto_GivenAnimeWithValues_ShouldReturnEmptyAnimeSearchEntryDtoWithValues()
		{
			// Given
			const string expectedTitle = "ExpectedTitle";
			const string expectedImageUrl = "ExpectedImageUrl";
			const string expectedJapaneseTitle = "期待される日本語タイトル";
			const string expectedTitleSynonyms = "expectedTitleSynonyms";
			const string expectedAbout = "ExpectedAbout";
			const string expectedType = "ExpectedAbout";
			const string expectedStatus = "ExpectedStatus";
			const long expectedMalId = 1;

			var anime = new AnimeBuilder()
				.WithTitle(expectedTitle)
				.WithImageUrl(expectedImageUrl)
				.WithMalId(expectedMalId)
				.WithJapaneseTitle(expectedJapaneseTitle)
				.WithTitleSynonyms(expectedTitleSynonyms)
				.WithAbout(expectedAbout)
				.WithAnimeType(at => at.WithName(expectedType))
				.WithAnimeStatus(at => at.WithName(expectedStatus))
				.Build();

			// When
			var dto = anime.ToAnimeCardDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.Title.Should().Be(expectedTitle);
				dto.ImageUrl.Should().Be(expectedImageUrl);
				dto.MalId.Should().Be(expectedMalId);
				dto.JapaneseTitle.Should().Be(expectedJapaneseTitle);
				dto.TitleSynonyms.Should().Be(expectedTitleSynonyms);
				dto.About.Should().Be(expectedAbout);
				dto.Type.Should().Be(expectedType);
				dto.Status.Should().Be(expectedStatus);
			}
		}

		[Fact]
		public void ToAnimeTableEntryDto_GivenEmptyAnime_ShouldReturnEmptyAnimeTableEntryDto()
		{
			// Given
			var anime = new AnimeBuilder().Build();

			// When
			var dto = anime.ToAnimeTableEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.ImageUrl.Should().BeEmpty();
				dto.Title.Should().BeEmpty();
				dto.Url.Should().NotBeEmpty();
				dto.MalId.Should().Be(default);
				dto.AiringFrom.Should().Be(null);
			}
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("Naruto")]
		[InlineData("This is very pecular")]
		[InlineData("Zażółć gęślą jaźń")]
		[InlineData("\n\n \t        \t\n")]
		public void ToAnimeTableEntryDto_GivenVariousTitleAnime_ShouldReturnEmptyAnimeTableEntryDtoWithTitle(string title)
		{
			// Given
			var anime = new AnimeBuilder()
				.WithTitle(title)
				.Build();

			// When
			var dto = anime.ToAnimeSearchEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.Title.Should().Be(title);
			}
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("\n\n \t        \t\n")]
		public void ToAnimeTableEntryDto_GivenAnimeWithNullOrWhitespaceAiringFrom_ShouldReturnAnimeTableEntryDtoWithNullAiringFrom(string date)
		{
			// Given
			var anime = new AnimeBuilder()
				.WithAiringDate(date)
				.Build();

			// When
			var dto = anime.ToAnimeTableEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.AiringFrom.Should().BeNull();
			}
		}

		[Theory]
		[InlineData("01-01-0001")]
		[InlineData("01-01-1999")]
		[InlineData("01-01-2000")]
		[InlineData("10-10-2020")]
		[InlineData("31-12-2020")]
		[InlineData("29-02-2020")]
		public void ToAnimeTableEntryDto_GivenAnimeWithValidAiringDate_ShouldReturnAnimeCardDtoWithAiringFrom(string date)
		{
			// Given
			var anime = new AnimeBuilder()
				.WithAiringDate(date)
				.Build();

			// When
			var dto = anime.ToAnimeTableEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.AiringFrom.Should().NotBeNull();
			}
		}

		[Theory]
		[InlineData("Naruto")]
		[InlineData("This is very pecular")]
		[InlineData("Zażółć gęślą jaźń")]
		[InlineData("01/01/1999")]
		[InlineData("1th sept 1999")]
		public void ToAnimeTableEntryDto_GivenAnimeWithInvalidAiringDate_ShouldThrowException(string date)
		{
			// Given
			var anime = new AnimeBuilder()
				.WithAiringDate(date)
				.Build();

			// When
			Func<AnimeTableEntryDto> func = () => anime.ToAnimeTableEntryDto();

			// Then
			func.Should().Throw<Exception>();
		}

		[Fact]
		public void ToAnimeTableEntryDto_GivenAnimeWithValues_ShouldReturnEmptyAnimeSearchEntryDtoWithValues()
		{
			// Given
			const string expectedTitle = "ExpectedTitle";
			const string expectedImageUrl = "ExpectedImageUrl";
			const long expectedMalId = 1;
			const string expectedUrl = "https://myanimelist.net/anime/1";

			var anime = new AnimeBuilder()
				.WithTitle(expectedTitle)
				.WithImageUrl(expectedImageUrl)
				.WithMalId(expectedMalId)
				.Build();

			// When
			var dto = anime.ToAnimeTableEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.Title.Should().Be(expectedTitle);
				dto.ImageUrl.Should().Be(expectedImageUrl);
				dto.MalId.Should().Be(expectedMalId);
				dto.Url.Should().Be(expectedUrl);
			}
		}
	}
}
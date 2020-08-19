using FluentAssertions;
using FluentAssertions.Execution;
using SeiyuuMoe.Application.Seiyuus;
using SeiyuuMoe.Application.Seiyuus.Extensions;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Tests.Unit.Builders.Model;
using System;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tests.Application.Extensions
{
	public class SeiyuuExtensionsTests
	{
		[Fact]
		public void ToSeiyuuSearchEntryDto_GivenSeiyuuAnime_ShouldReturnEmptySeiyuuSearchEntryDto()
		{
			// Given
			var seiyuu = new SeiyuuBuilder().Build();

			// When
			var dto = seiyuu.ToSeiyuuSearchEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.ImageUrl.Should().BeEmpty();
				dto.Name.Should().BeEmpty();
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
		public void ToSeiyuuSearchEntryDto_GivenVariousNameSeiyuu_ShouldReturnEmptySeiyuuSearchEntryDtoWithName(string name)
		{
			// Given
			var seiyuu = new SeiyuuBuilder()
				.WithName(name)
				.Build();

			// When
			var dto = seiyuu.ToSeiyuuSearchEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.Name.Should().Be(name);
			}
		}

		[Fact]
		public void ToSeiyuuSearchEntryDto_GivenSeiyuuWithValues_ShouldReturnEmptySeiyuuSearchEntryDtoWithValues()
		{
			// Given
			const string expectedName = "ExpectedTitle";
			const string expectedImageUrl = "ExpectedImageUrl";
			const long expectedMalId = 1;

			var seiyuu = new SeiyuuBuilder()
				.WithName(expectedName)
				.WithImageUrl(expectedImageUrl)
				.WithMalId(expectedMalId)
				.Build();

			// When
			var dto = seiyuu.ToSeiyuuSearchEntryDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.Name.Should().Be(expectedName);
				dto.ImageUrl.Should().Be(expectedImageUrl);
				dto.MalId.Should().Be(expectedMalId);
			}
		}

		[Fact]
		public void ToSeiyuuCardDto_GivenNull_ShouldReturnNull()
		{
			// Given && When
			var dto = ((Seiyuu) null).ToSeiyuuCardDto();

			// Then
			dto.Should().BeNull();
		}

		[Fact]
		public void ToSeiyuuCardDto_GivenEmptySeiyuu_ShouldReturnEmptySeiyuuCardDto()
		{
			// Given
			var seiyuu = new SeiyuuBuilder().Build();

			// When
			var dto = seiyuu.ToSeiyuuCardDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.ImageUrl.Should().BeEmpty();
				dto.Name.Should().BeEmpty();
				dto.MalId.Should().Be(default);
				dto.JapaneseName.Should().BeEmpty();
				dto.About.Should().BeEmpty();
			}
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("Naruto")]
		[InlineData("This is very pecular")]
		[InlineData("Zażółć gęślą jaźń")]
		[InlineData("\n\n \t        \t\n")]
		public void ToSeiyuuCardDto_GivenVariousTitleSeiyuu_ShouldReturnEmptySeiyuuCardDtoWithTitle(string name)
		{
			// Given
			var seiyuu = new SeiyuuBuilder()
				.WithName(name)
				.Build();

			// When
			var dto = seiyuu.ToSeiyuuCardDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.Name.Should().Be(name);
			}
		}

		[Fact]
		public void ToSeiyuuCardDto_GivenAnimeWithValues_ShouldReturnEmptyAnimeSearchEntryDtoWithValues()
		{
			// Given
			const string expectedName = "ExpectedTitle";
			const string expectedImageUrl = "ExpectedImageUrl";
			const string expectedJapaneseName = "期待される日本語タイトル";
			const string expectedAbout = "ExpectedAbout";
			const long expectedMalId = 1;

			var anime = new SeiyuuBuilder()
				.WithName(expectedName)
				.WithImageUrl(expectedImageUrl)
				.WithMalId(expectedMalId)
				.WithJapaneseName(expectedJapaneseName)
				.WithAbout(expectedAbout)
				.Build();

			// When
			var dto = anime.ToSeiyuuCardDto();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.Name.Should().Be(expectedName);
				dto.ImageUrl.Should().Be(expectedImageUrl);
				dto.MalId.Should().Be(expectedMalId);
				dto.JapaneseName.Should().Be(expectedJapaneseName);
				dto.About.Should().Be(expectedAbout);
			}
		}

		[Fact]
		public void ToSeiyuuTableEntry_GivenEmptySeiyuu_ShouldReturnEmptySeiyuuTableEntryDto()
		{
			// Given
			var seiyuu = new SeiyuuBuilder().Build();

			// When
			var dto = seiyuu.ToSeiyuuTableEntry();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.ImageUrl.Should().BeEmpty();
				dto.Name.Should().BeEmpty();
				dto.Url.Should().NotBeEmpty();
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
		public void ToSeiyuuTableEntry_GivenVariousSeiyuuName_ShouldReturnEmptySeiyuuTableEntryDtoWithName(string name)
		{
			// Given
			var seiyuu = new SeiyuuBuilder()
				.WithName(name)
				.Build();

			// When
			var dto = seiyuu.ToSeiyuuTableEntry();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.Name.Should().Be(name);
			}
		}

		[Fact]
		public void ToSeiyuuTableEntry_GivenSeiyuuWithValues_ShouldReturnSeiyuuTableEntryDtoWithValues()
		{
			// Given
			const string expectedTitle = "ExpectedTitle";
			const string expectedImageUrl = "ExpectedImageUrl";
			const long expectedMalId = 1;
			const string expectedUrl = "https://myanimelist.net/people/1";

			var anime = new SeiyuuBuilder()
				.WithName(expectedTitle)
				.WithImageUrl(expectedImageUrl)
				.WithMalId(expectedMalId)
				.Build();

			// When
			var dto = anime.ToSeiyuuTableEntry();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.Name.Should().Be(expectedTitle);
				dto.ImageUrl.Should().Be(expectedImageUrl);
				dto.MalId.Should().Be(expectedMalId);
				dto.Url.Should().Be(expectedUrl);
			}
		}
	}
}
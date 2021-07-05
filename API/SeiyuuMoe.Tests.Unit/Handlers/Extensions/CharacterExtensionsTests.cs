using FluentAssertions;
using FluentAssertions.Execution;
using SeiyuuMoe.Application.Characters.Extensions;
using SeiyuuMoe.Tests.Common.Builders.Model;
using Xunit;

namespace SeiyuuMoe.Tests.Application.Extensions
{
	public class CharacterExtensionsTests
	{
		[Fact]
		public void ToCharacterTableEntry_GivenEmptyCharacter_ShouldReturnEmptyCharacterTableEntryDto()
		{
			// Given
			var character = new CharacterBuilder().Build();

			// When
			var dto = character.ToCharacterTableEntry();

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
		public void ToCharacterTableEntry_GivenVariousCharacterName_ShouldReturnEmptyCharacterTableEntryDtoWithName(string name)
		{
			// Given
			var seiyuu = new CharacterBuilder()
				.WithName(name)
				.Build();

			// When
			var dto = seiyuu.ToCharacterTableEntry();

			// Then
			using (new AssertionScope())
			{
				dto.Should().NotBeNull();
				dto.Name.Should().Be(name);
			}
		}

		[Fact]
		public void ToCharacterTableEntry_GivenCharacterWithValues_ShouldReturnCharacterTableEntryDtoWithValues()
		{
			// Given
			const string expectedTitle = "ExpectedTitle";
			const string expectedImageUrl = "ExpectedImageUrl";
			const long expectedMalId = 1;
			const string expectedUrl = "https://myanimelist.net/character/1";

			var anime = new CharacterBuilder()
				.WithName(expectedTitle)
				.WithImageUrl(expectedImageUrl)
				.WithMalId(expectedMalId)
				.Build();

			// When
			var dto = anime.ToCharacterTableEntry();

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
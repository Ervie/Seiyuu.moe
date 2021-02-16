using FluentAssertions;
using SeiyuuMoe.VndbBackgroundJobs.Application.Helpers;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Application.VndbBackgroundJobs
{
	public class VndbParserHelperTests
	{
		[Theory]
		[InlineData("")]
		[InlineData(null)]
		[InlineData("\n\n     \t ")]
		public void GenerateVndbVisualNovelImageUrlFromImageId_GivenNullOrWhiteSpace_ShouldReturnNull(string input)
		{
			// When
			var result = VndbParserHelper.GenerateVndbVisualNovelImageUrlFromImageId(input);

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public void GenerateVndbVisualNovelImageUrlFromImageId_GivenSmallImageId_ShouldBuildLink()
		{
			// Given
			const string imageId = "cv201";

			// When
			var result = VndbParserHelper.GenerateVndbVisualNovelImageUrlFromImageId(imageId);

			// Then
			result.Should().Be("https://s2.vndb.org/cv/01/201.jpg");
		}

		[Fact]
		public void GenerateVndbVisualNovelImageUrlFromImageId_GivenBigImageId_ShouldBuildLink()
		{
			// Given
			const string imageId = "cv55554";

			// When
			var result = VndbParserHelper.GenerateVndbVisualNovelImageUrlFromImageId(imageId);

			// Then
			result.Should().Be("https://s2.vndb.org/cv/54/55554.jpg");
		}

		[Fact]
		public void GenerateVndbCharacterImageUrlFromImageId_GivenSmallImageId_ShouldBuildLink()
		{
			// Given
			const string imageId = "ch201";

			// When
			var result = VndbParserHelper.GenerateVndbCharacterImageUrlFromImageId(imageId);

			// Then
			result.Should().Be("https://s2.vndb.org/ch/01/201.jpg");
		}

		[Fact]
		public void GenerateVndbCharacterImageUrlFromImageId_GivenBigImageId_ShouldBuildLink()
		{
			// Given
			const string imageId = "ch55554";

			// When
			var result = VndbParserHelper.GenerateVndbCharacterImageUrlFromImageId(imageId);

			// Then
			result.Should().Be("https://s2.vndb.org/ch/54/55554.jpg");
		}

		[Theory]
		[InlineData("")]
		[InlineData(null)]
		[InlineData("\n\n     \t ")]
		public void GenerateVndbCharacterImageUrlFromImageId_GivenNullOrWhiteSpace_ShouldReturnNull(string input)
		{
			// When
			var result = VndbParserHelper.GenerateVndbCharacterImageUrlFromImageId(input);

			// Then
			result.Should().BeNull();
		}
	}
}
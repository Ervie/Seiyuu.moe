using FluentAssertions;
using FluentAssertions.Execution;
using SeiyuuMoe.Application.SeiyuuComparison.Extensions;
using SeiyuuMoe.Tests.Unit.Builders.ComparisonEntities;
using System;
using System.Collections.Generic;
using System.Text;
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
	}
}

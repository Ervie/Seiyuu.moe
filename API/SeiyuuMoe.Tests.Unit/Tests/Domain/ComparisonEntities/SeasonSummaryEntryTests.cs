using FluentAssertions;
using SeiyuuMoe.Tests.Unit.Builders.ComparisonEntities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tests.Domain.ComparisonEntities
{
	public class SeasonSummaryEntryTests
	{
		[Fact]
		public void GetTotalSignificanceValue_GivenEmptyEntry_ShouldThrowException()
		{
			var seasonSummaryEntry = new SeasonSummaryEntryBuilder().Build();

			Func<decimal> func = () => seasonSummaryEntry.GetTotalSignificanceValue();

			func.Should().ThrowExactly<NullReferenceException>();
		}

		[Fact]
		public void GetTotalSignificanceValue_GivenNoPairs_ShouldReturnZero()
		{
			var seasonSummaryEntry = new SeasonSummaryEntryBuilder()
				.WithSeiyuu(x => x.Build())
				.WithAnime(x => x.Build())
				.WithCharacter(x => x.Build())
				.Build();

			var result = seasonSummaryEntry.GetTotalSignificanceValue();

			result.Should().Be(0);
		}

		[Fact]
		public void GetTotalSignificanceValue_GivenAnimeWithZeroPopularity_ShouldReturnZero()
		{
			var seasonSummaryEntry = new SeasonSummaryEntryBuilder()
				.WithSeiyuu(x => x.Build())
				.WithAnime(x => x.WithPopularity(0).Build())
				.WithCharacter(x => x.WithPopularity(1).Build())
				.Build();

			var result = seasonSummaryEntry.GetTotalSignificanceValue();

			result.Should().Be(0);
		}

		[Fact]
		public void GetTotalSignificanceValue_GivenCharacterWithZeroPopularity_ShouldReturnZero()
		{
			var seasonSummaryEntry = new SeasonSummaryEntryBuilder()
				.WithSeiyuu(x => x.Build())
				.WithAnime(x => x.WithPopularity(0).Build())
				.WithCharacter(x => x.WithPopularity(1).Build())
				.Build();

			var result = seasonSummaryEntry.GetTotalSignificanceValue();

			result.Should().Be(0);
		}

		[Theory]
		[InlineData(1, 1, 0.001)]
		[InlineData(999, 1, 0.999)]
		[InlineData(1000, 1, 1)]
		[InlineData(1001, 1, 1.001)]
		[InlineData(2001, 1, 2.001)]
		[InlineData(999, 10, 9.99)]
		[InlineData(1000, 10, 10)]
		[InlineData(1001, 1000, 1001)]
		[InlineData(100000, 500, 50000)]
		[InlineData(99999, 999, 99899.001)]
		public void GetTotalSignificanceValue_GivenCharacterNotZeroPopularities_ShouldReturnCorrectValue(int animePopularity, int characterPopularity, decimal expectedResult)
		{
			var seasonSummaryEntry = new SeasonSummaryEntryBuilder()
				.WithSeiyuu(x => x.Build())
				.WithAnime(x => x.WithPopularity(animePopularity).Build())
				.WithCharacter(x => x.WithPopularity(characterPopularity).Build())
				.Build();

			var result = seasonSummaryEntry.GetTotalSignificanceValue();

			result.Should().Be(expectedResult);
		}

	}
}

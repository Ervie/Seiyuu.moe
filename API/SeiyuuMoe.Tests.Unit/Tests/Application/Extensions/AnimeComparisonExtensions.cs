using FluentAssertions;
using FluentAssertions.Execution;
using SeiyuuMoe.AnimeComparison;
using SeiyuuMoe.Application.AnimeComparison.Extensions;
using SeiyuuMoe.Tests.Unit.Builders.ComparisonEntities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tests.Application.Extensions
{
	public class AnimeComparisonExtensions
	{
		[Fact]
		public void ToAnimeComparisonEntryDto_GivenAnimeComparisonEntryWithNoSeiyuu_ShouldThrowNullException()
		{
			// Given
			var animeComparison = new AnimeComparisonEntryBuilder().Build();

			// When
			Func<AnimeComparisonEntryDto> func = () => animeComparison.ToAnimeComparisonEntryDto();

			// Then
			func.Should().Throw<NullReferenceException>();
		}

		[Fact]
		public void ToAnimeComparisonEntryDto_GivenEmptyAnimeComparisonEntry_ShouldReturnEmptyAnimeComparisonEntryDto()
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
	}
}

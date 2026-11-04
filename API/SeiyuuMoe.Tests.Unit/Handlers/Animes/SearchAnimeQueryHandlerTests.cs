using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using SeiyuuMoe.Application.Animes;
using SeiyuuMoe.Application.Animes.SearchAnime;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.WebEssentials;
using SeiyuuMoe.Tests.Common.Builders.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Application.Animes
{
	public class SearchAnimeQueryHandlerTests
	{
		[Fact]
		public async Task HandleAsync_GivenRepositoryReturnEmptyCollection_ShouldReturnEmptyResults()
		{
			// Given
			var query = new SearchAnimeQuery();
			var mockRepository = Substitute.For<IAnimeRepository>();
			mockRepository.GetOrderedPageByPopularityAsync(Arg.Any<Expression<Func<Anime, bool>>>(), 0, 10)
				.Returns(new PagedResult<Anime> { Results = new List<Anime>() });
			var mockAnimeSearchCriteriaService = Substitute.For<IAnimeSearchCriteriaService>();
			mockAnimeSearchCriteriaService.BuildExpression(query).Returns(f => true);
			var handler = new SearchAnimeQueryHandler(mockRepository, mockAnimeSearchCriteriaService);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			await mockRepository.Received(1).GetOrderedPageByPopularityAsync(Arg.Any<Expression<Func<Anime, bool>>>(), 0, 10);
			result.Results.Should().BeEmpty();
		}

		[Fact]
		public async Task HandleAsync_GivenRepositoryReturnCollection_ShouldReturnResponse()
		{
			// Given
			var query = new SearchAnimeQuery();
			var mockRepository = Substitute.For<IAnimeRepository>();
			mockRepository.GetOrderedPageByPopularityAsync(Arg.Any<Expression<Func<Anime, bool>>>(), 0, 10)
				.Returns(new PagedResult<Anime>
				{
					Results = new List<Anime>
					{
						new AnimeBuilder().WithMalId(default).Build()
					}
				});
			var mockAnimeSearchCriteriaService = Substitute.For<IAnimeSearchCriteriaService>();
			mockAnimeSearchCriteriaService.BuildExpression(query).Returns(f => true);
			var handler = new SearchAnimeQueryHandler(mockRepository, mockAnimeSearchCriteriaService);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			await mockRepository.Received(1).GetOrderedPageByPopularityAsync(Arg.Any<Expression<Func<Anime, bool>>>(), 0, 10);
			using (new AssertionScope())
			{
				result.Should().NotBeNull();
				result.Results.Should().NotBeNull();
			}
		}
	}
}
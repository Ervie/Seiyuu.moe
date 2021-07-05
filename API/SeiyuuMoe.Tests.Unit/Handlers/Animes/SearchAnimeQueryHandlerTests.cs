using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
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
			var mockRepository = new Mock<IAnimeRepository>();
			mockRepository.Setup(x => x.GetOrderedPageByPopularityAsync(It.IsAny<Expression<Func<Anime, bool>>>(), 0, 10))
				.ReturnsAsync(new PagedResult<Anime> { Results = new List<Anime>() });
			var mockAnimeSearchCriteriaService = new Mock<IAnimeSearchCriteriaService>();
			mockAnimeSearchCriteriaService.Setup(x => x.BuildExpression(query)).Returns(f => true);
			var handler = new SearchAnimeQueryHandler(mockRepository.Object, mockAnimeSearchCriteriaService.Object);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			mockRepository.Verify(x => x.GetOrderedPageByPopularityAsync(It.IsAny<Expression<Func<Anime, bool>>>(), 0, 10), Times.Once);
			result.Results.Should().BeEmpty();
		}

		[Fact]
		public async Task HandleAsync_GivenRepositoryReturnCollection_ShouldReturnResponse()
		{
			// Given
			var query = new SearchAnimeQuery();
			var mockRepository = new Mock<IAnimeRepository>();
			mockRepository.Setup(x => x.GetOrderedPageByPopularityAsync(It.IsAny<Expression<Func<Anime, bool>>>(), 0, 10))
				.ReturnsAsync(new PagedResult<Anime>
				{
					Results = new List<Anime>
					{
						new AnimeBuilder().WithMalId(default).Build()
					}
				});
			var mockAnimeSearchCriteriaService = new Mock<IAnimeSearchCriteriaService>();
			mockAnimeSearchCriteriaService.Setup(x => x.BuildExpression(query)).Returns(f => true);
			var handler = new SearchAnimeQueryHandler(mockRepository.Object, mockAnimeSearchCriteriaService.Object);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			mockRepository.Verify(x => x.GetOrderedPageByPopularityAsync(It.IsAny<Expression<Func<Anime, bool>>>(), 0, 10), Times.Once);
			using (new AssertionScope())
			{
				result.Should().NotBeNull();
				result.Results.Should().NotBeNull();
			}
		}
	}
}
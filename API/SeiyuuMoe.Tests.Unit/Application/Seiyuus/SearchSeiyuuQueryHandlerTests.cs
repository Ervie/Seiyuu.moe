using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using SeiyuuMoe.Application.Seiyuus;
using SeiyuuMoe.Application.Seiyuus.SearchSeiyuu;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.WebEssentials;
using SeiyuuMoe.Tests.Common.Builders.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tests.Application.Seiyuus
{
	public class SearchSeiyuuQueryHandlerTests
	{
		[Fact]
		public async Task HandleAsync_GivenRepositoryReturnEmptyCollection_ShouldReturnEmptyResults()
		{
			// Given
			var query = new SearchSeiyuuQuery();
			var mockRepository = new Mock<ISeiyuuRepository>();
			mockRepository.Setup(x => x.GetOrderedPageByPopularityAsync(It.IsAny<Expression<Func<Seiyuu, bool>>>(), 0, 10))
				.ReturnsAsync(new PagedResult<Seiyuu> { Results = new List<Seiyuu>() });
			var mockAnimeSearchCriteriaService = new Mock<ISeiyuuSearchCriteriaService>();
			mockAnimeSearchCriteriaService.Setup(x => x.BuildExpression(query)).Returns(f => true);
			var handler = new SearchSeiyuuQueryHandler(mockRepository.Object, mockAnimeSearchCriteriaService.Object);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			mockRepository.Verify(x => x.GetOrderedPageByPopularityAsync(It.IsAny<Expression<Func<Seiyuu, bool>>>(), 0, 10), Times.Once);
			using (new AssertionScope())
			{
				result.Found.Should().BeTrue();
				result.Payload.Results.Should().BeEmpty();
			}
		}

		[Fact]
		public async Task HandleAsync_GivenRepositoryReturnCollection_ShouldReturnResponse()
		{
			// Given
			var query = new SearchSeiyuuQuery();
			var mockRepository = new Mock<ISeiyuuRepository>();
			mockRepository.Setup(x => x.GetOrderedPageByPopularityAsync(It.IsAny<Expression<Func<Seiyuu, bool>>>(), 0, 10))
				.ReturnsAsync(new PagedResult<Seiyuu>
				{
					Results = new List<Seiyuu>
					{
						new SeiyuuBuilder().WithMalId(default).Build()
					}
				});
			var mockAnimeSearchCriteriaService = new Mock<ISeiyuuSearchCriteriaService>();
			mockAnimeSearchCriteriaService.Setup(x => x.BuildExpression(query)).Returns(f => true);
			var handler = new SearchSeiyuuQueryHandler(mockRepository.Object, mockAnimeSearchCriteriaService.Object);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			mockRepository.Verify(x => x.GetOrderedPageByPopularityAsync(It.IsAny<Expression<Func<Seiyuu, bool>>>(), 0, 10), Times.Once);
			using (new AssertionScope())
			{
				result.Found.Should().BeTrue();
				result.Payload.Should().NotBeNull();
				result.Payload.Results.Should().NotBeNull();
			}
		}
	}
}
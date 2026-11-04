using FluentAssertions;
using NSubstitute;
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

namespace SeiyuuMoe.Tests.Application.Seiyuus
{
	public class SearchSeiyuuQueryHandlerTests
	{
		[Fact]
		public async Task HandleAsync_GivenRepositoryReturnEmptyCollection_ShouldReturnEmptyResults()
		{
			// Given
			var query = new SearchSeiyuuQuery();
			var mockRepository = Substitute.For<ISeiyuuRepository>();
			mockRepository.GetOrderedPageByPopularityAsync(Arg.Any<Expression<Func<Seiyuu, bool>>>(), 0, 10)
				.Returns(new PagedResult<Seiyuu> { Results = new List<Seiyuu>() });
			var mockAnimeSearchCriteriaService = Substitute.For<ISeiyuuSearchCriteriaService>();
			mockAnimeSearchCriteriaService.BuildExpression(query).Returns(f => true);
			var handler = new SearchSeiyuuQueryHandler(mockRepository, mockAnimeSearchCriteriaService);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			await mockRepository.Received(1).GetOrderedPageByPopularityAsync(Arg.Any<Expression<Func<Seiyuu, bool>>>(), 0, 10);
			result.Results.Should().BeEmpty();
		}

		[Fact]
		public async Task HandleAsync_GivenRepositoryReturnCollection_ShouldReturnResponse()
		{
			// Given
			var query = new SearchSeiyuuQuery();
			var mockRepository = Substitute.For<ISeiyuuRepository>();
			mockRepository.GetOrderedPageByPopularityAsync(Arg.Any<Expression<Func<Seiyuu, bool>>>(), 0, 10)
				.Returns(new PagedResult<Seiyuu>
				{
					Results = new List<Seiyuu>
					{
						new SeiyuuBuilder().WithMalId(default).Build()
					}
				});
			var mockAnimeSearchCriteriaService = Substitute.For<ISeiyuuSearchCriteriaService>();
			mockAnimeSearchCriteriaService.BuildExpression(query).Returns(f => true);
			var handler = new SearchSeiyuuQueryHandler(mockRepository, mockAnimeSearchCriteriaService);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			await mockRepository.Received(1).GetOrderedPageByPopularityAsync(Arg.Any<Expression<Func<Seiyuu, bool>>>(), 0, 10);
			result.Results.Should().NotBeNull();
		}
	}
}
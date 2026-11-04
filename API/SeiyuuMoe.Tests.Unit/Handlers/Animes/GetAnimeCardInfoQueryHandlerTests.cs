using FluentAssertions;
using NSubstitute;
using SeiyuuMoe.Application.Animes;
using SeiyuuMoe.Application.Animes.GetAnimeCardInfo;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Tests.Common.Builders.Model;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Application.Animes
{
	public class GetAnimeCardInfoQueryHandlerTests
	{
		[Fact]
		public async Task HandleAsync_GivenRepositoryReturnNull_ShouldReturnNotFoundResponse()
		{
			// Given
			const long animeMalId = 0;
			var mockRepository = Substitute.For<IAnimeRepository>();
			var handler = new GetAnimeCardInfoQueryHandler(mockRepository);

			// When
			var result = await handler.HandleAsync(new GetAnimeCardInfoQuery(animeMalId));

			// Then
			await mockRepository.Received(1).GetAsync(animeMalId);
			result.Should().BeNull();
		}

		[Fact]
		public async Task HandleAsync_GivenRepositoryReturnAnime_ShouldReturnResponse()
		{
			// Given
			const long animeMalId = 0;
			var mockRepository = Substitute.For<IAnimeRepository>();
			mockRepository.GetAsync(animeMalId)
				.Returns(new AnimeBuilder().WithMalId(animeMalId).Build());
			var handler = new GetAnimeCardInfoQueryHandler(mockRepository);

			// When
			var result = await handler.HandleAsync(new GetAnimeCardInfoQuery(animeMalId));

			// Then
			await mockRepository.Received(1).GetAsync(animeMalId);
			result.Should().NotBeNull().And.BeOfType(typeof(AnimeCardDto));
		}
	}
}
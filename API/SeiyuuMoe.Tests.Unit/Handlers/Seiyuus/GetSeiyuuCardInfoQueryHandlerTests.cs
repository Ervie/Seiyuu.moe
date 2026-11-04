using FluentAssertions;
using NSubstitute;
using SeiyuuMoe.Application.Seiyuus;
using SeiyuuMoe.Application.Seiyuus.GetSeiyuuCardInfo;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Tests.Common.Builders.Model;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Application.Seiyuus
{
	public class GetSeiyuuCardInfoQueryHandlerTests
	{
		[Fact]
		public async Task HandleAsync_GivenRepositoryReturnNull_ShouldReturnNotFoundResponse()
		{
			// Given
			const long seiyuuMalId = 0;
			var mockRepository = Substitute.For<ISeiyuuRepository>();
			var handler = new GetSeiyuuCardInfoQueryHandler(mockRepository);

			// When
			var result = await handler.HandleAsync(new GetSeiyuuCardInfoQuery(seiyuuMalId));

			// Then
			await mockRepository.Received(1).GetAsync(seiyuuMalId);
			result.Should().BeNull();
		}

		[Fact]
		public async Task HandleAsync_GivenRepositoryReturnAnime_ShouldReturnResponse()
		{
			// Given
			const long seiyuuMalId = 0;
			var mockRepository = Substitute.For<ISeiyuuRepository>();
			mockRepository.GetAsync(seiyuuMalId)
				.Returns(new SeiyuuBuilder().WithMalId(seiyuuMalId).Build());
			var handler = new GetSeiyuuCardInfoQueryHandler(mockRepository);

			// When
			var result = await handler.HandleAsync(new GetSeiyuuCardInfoQuery(seiyuuMalId));

			// Then
			await mockRepository.Received(1).GetAsync(seiyuuMalId);
			result.Should().NotBeNull().And.BeOfType(typeof(SeiyuuCardDto));
		}
	}
}
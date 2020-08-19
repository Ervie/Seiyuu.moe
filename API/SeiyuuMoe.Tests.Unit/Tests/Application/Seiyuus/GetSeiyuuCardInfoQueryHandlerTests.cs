using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using SeiyuuMoe.Application.Seiyuus;
using SeiyuuMoe.Application.Seiyuus.GetSeiyuuCardInfo;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Tests.Unit.Builders.Model;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tests.Application.Seiyuus
{
	public class GetSeiyuuCardInfoQueryHandlerTests
	{
		[Fact]
		public async Task HandleAsync_GivenRepositoryReturnNull_ShouldReturnNotFoundResponse()
		{
			// Given
			const long seiyuuMalId = 0;
			var mockRepository = new Mock<ISeiyuuRepository>();
			var handler = new GetSeiyuuCardInfoQueryHandler(mockRepository.Object);

			// When
			var result = await handler.HandleAsync(new GetSeiyuuCardInfoQuery(seiyuuMalId));

			// Then
			mockRepository.Verify(x => x.GetAsync(seiyuuMalId), Times.Once);
			using (new AssertionScope())
			{
				result.Found.Should().BeFalse();
				result.Payload.Should().BeNull();
			}
		}

		[Fact]
		public async Task HandleAsync_GivenRepositoryReturnAnime_ShouldReturnResponse()
		{
			// Given
			const long seiyuuMalId = 0;
			var mockRepository = new Mock<ISeiyuuRepository>();
			mockRepository.Setup(x => x.GetAsync(seiyuuMalId)).ReturnsAsync(new SeiyuuBuilder().WithMalId(seiyuuMalId).Build());
			var handler = new GetSeiyuuCardInfoQueryHandler(mockRepository.Object);

			// When
			var result = await handler.HandleAsync(new GetSeiyuuCardInfoQuery(seiyuuMalId));

			// Then
			mockRepository.Verify(x => x.GetAsync(seiyuuMalId), Times.Once);
			using (new AssertionScope())
			{
				result.Found.Should().BeTrue();
				result.Payload.Should().NotBeNull().And.BeOfType(typeof(SeiyuuCardDto));
			}
		}
	}
}
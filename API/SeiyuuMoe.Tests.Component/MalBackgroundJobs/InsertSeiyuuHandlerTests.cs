using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using SeiyuuMoe.Domain.S3;
using SeiyuuMoe.Infrastructure.Animes;
using SeiyuuMoe.Infrastructure.Characters;
using SeiyuuMoe.Infrastructure.Context;
using SeiyuuMoe.Infrastructure.Jikan;
using SeiyuuMoe.Infrastructure.Seasons;
using SeiyuuMoe.Infrastructure.Seiyuus;
using SeiyuuMoe.MalBackgroundJobs.Application.Handlers;
using SeiyuuMoe.Tests.Common.Builders.Jikan;
using SeiyuuMoe.Tests.Common.Helpers;
using SeiyuuMoe.Tests.Common.Stubs;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Component.MalBackgroundJobs
{
	public class InsertSeiyuuHandlerTests
	{
		[Fact]
		public async Task HandleAsync_GivenEmptyDatabaseAndBatchSizeOne_ShouldCallJikanOnceWithNextId()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build(), new S3ServiceStub(1), 1);

			// When
			await handler.HandleAsync();

			// Then
			jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(2), Times.Once);
		}

		[Fact]
		public async Task HandleAsync_GivenEmptyDatabaseAndBatchSizeOneAndBiggerId_ShouldCallJikanOnceWithNextId()
		{
			// Given
			var lastId = 10000;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build(), new S3ServiceStub(lastId), 1);

			// When
			await handler.HandleAsync();

			// Then
			jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(lastId + 1), Times.Once);
		}

		[Fact]
		public async Task HandleAsync_GivenEmptyDatabaseAndBatchSizeFive_ShouldCallJikanFiveTimesWithNextIds()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build(), new S3ServiceStub(1), 5);

			// When
			await handler.HandleAsync();

			// Then
			using (new AssertionScope())
			{
				jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(2), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(3), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(4), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(5), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(6), Times.Once);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenEmptyDatabaseAndBatchSizeOne_LastInsertedSeiyuuIdShouldBeBiggerByOne()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder();
			var s3Service = new S3ServiceStub(1);

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build(), s3Service, 1);

			// When
			await handler.HandleAsync();

			// Then
			var state = await s3Service.GetBgJobsStateAsync(It.IsAny<string>());
			state.LastCheckedSeiyuuMalId.Should().Be(2);
		}

		[Fact]
		public async Task HandleAsync_GivenEmptyDatabaseAndBatchSizeFive_LastInsertedSeiyuuIdShouldBeBiggerByFive()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder();
			var s3Service = new S3ServiceStub(1);

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build(), s3Service, 5);

			// When
			await handler.HandleAsync();

			// Then
			var state = await s3Service.GetBgJobsStateAsync(It.IsAny<string>());
			state.LastCheckedSeiyuuMalId.Should().Be(6);
		}

		private InsertSeiyuuHandler CreateHandler(SeiyuuMoeContext dbContext, JikanService jikanService, IS3Service s3Service, int insertSeiyuuBatchSize)
		{
			var animeRepository = new AnimeRepository(dbContext);
			var seiyuuRepository = new SeiyuuRepository(dbContext);
			var characterRepository = new CharacterRepository(dbContext);
			var animeRoleRepository = new AnimeRoleRepository(dbContext);
			var seasonRepository = new SeasonRepository(dbContext);

			return new InsertSeiyuuHandler(
				insertSeiyuuBatchSize,
				0,
				seiyuuRepository,
				seasonRepository,
				characterRepository,
				animeRepository,
				animeRoleRepository,
				jikanService,
				s3Service
			);
		}
	}
}
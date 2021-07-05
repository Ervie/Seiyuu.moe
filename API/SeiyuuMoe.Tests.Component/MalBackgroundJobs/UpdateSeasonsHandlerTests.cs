using FluentAssertions;
using FluentAssertions.Execution;
using JikanDotNet;
using JikanDotNet.Exceptions;
using SeiyuuMoe.Domain.Services;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Database.Seasons;
using SeiyuuMoe.MalBackgroundJobs.Application.Handlers;
using SeiyuuMoe.Tests.Common.Builders.Jikan;
using SeiyuuMoe.Tests.Common.Builders.Model;
using SeiyuuMoe.Tests.Common.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Component.MalBackgroundJobs
{
	public class UpdateSeasonsHandlerTests
	{
		[Fact]
		public async Task HandleAsync_GivenEmptyDatabase_ShouldAddNewSeason()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();

			var returnedLastSeason = new SeasonArchive
			{
				Season = new List<Seasons> { Seasons.Winter },
				Year = 2000
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithLastSeasonArchiveReturned(returnedLastSeason);
			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			// When
			await handler.HandleAsync();

			// Then
			dbContext.AnimeSeasons.Should().ContainSingle();
		}

		[Fact]
		public async Task HandleAsync_GivenNullReturned_ShouldNotAddNewSeason()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();

			var jikanServiceBuilder = new JikanServiceBuilder();
			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			// When
			await handler.HandleAsync();

			// Then
			dbContext.AnimeSeasons.Should().BeEmpty();
		}

		[Fact]
		public async Task HandleAsync_GivenExceptionFromJikan_ShouldNotAddNewSeason()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();

			var returnedLastSeason = new SeasonArchive
			{
				Season = new List<Seasons> { Seasons.Winter },
				Year = 2000
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithGetSeasonArchiveThrowing();
			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			// When
			var action = handler.Awaiting(x => x.HandleAsync());

			// Then
			using (new AssertionScope())
			{
				await action.Should().ThrowExactlyAsync<JikanRequestException>();
				dbContext.AnimeSeasons.Should().BeEmpty();
			}
		}

		[Fact]
		public async Task HandleAsync_GivenAlreadyExistingSeason_ShouldNotAddNewSeason()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();

			var returnedLastSeason = new SeasonArchive
			{
				Season = new List<Seasons> { Seasons.Winter },
				Year = 2000
			};

			var season = new SeasonBuilder()
				.WithId(1)
				.WithYear(2000)
				.WithName("Winter")
				.Build();

			await dbContext.AddAsync(season);
			await dbContext.SaveChangesAsync();

			var jikanServiceBuilder = new JikanServiceBuilder().WithLastSeasonArchiveReturned(returnedLastSeason);
			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			// When
			await handler.HandleAsync();

			// Then
			dbContext.AnimeSeasons.Should().ContainSingle();
		}

		[Fact]
		public async Task HandleAsync_GivenNewSeason_ShouldNotAddNewSeason()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();

			var returnedLastSeason = new SeasonArchive
			{
				Season = new List<Seasons> { Seasons.Spring },
				Year = 2000
			};

			var season = new SeasonBuilder()
				.WithId(1)
				.WithYear(2000)
				.WithName("Winter")
				.Build();

			await dbContext.AddAsync(season);
			await dbContext.SaveChangesAsync();

			var jikanServiceBuilder = new JikanServiceBuilder().WithLastSeasonArchiveReturned(returnedLastSeason);
			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			// When
			await handler.HandleAsync();

			// Then
			dbContext.AnimeSeasons.Should().HaveCount(2);
		}

		[Fact]
		public async Task HandleAsync_GivenMultipleSeasonsInYear_ShouldAddNewer()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();

			var returnedLastSeason = new SeasonArchive
			{
				Season = new List<Seasons> { Seasons.Winter, Seasons.Spring },
				Year = 2000
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithLastSeasonArchiveReturned(returnedLastSeason);
			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			// When
			await handler.HandleAsync();

			// Then
			using (new AssertionScope())
			{
				dbContext.AnimeSeasons.Should().ContainSingle();
				dbContext.AnimeSeasons.Single().Name.Should().Be("Spring");
				dbContext.AnimeSeasons.Single().Year.Should().Be(2000);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenMultipleSeasonsInYearAlreadyExisting_ShouldNotAddNewSeason()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();

			var returnedLastSeason = new SeasonArchive
			{
				Season = new List<Seasons> { Seasons.Winter, Seasons.Spring, Seasons.Summer, Seasons.Fall },
				Year = 2000
			};

			var seasons = new List<Domain.Entities.AnimeSeason>
			{
				new SeasonBuilder().WithId(10).WithYear(2000).WithName("Winter").Build(),
				new SeasonBuilder().WithId(11).WithYear(2000).WithName("Spring").Build(),
				new SeasonBuilder().WithId(13).WithYear(2000).WithName("Summer").Build(),
				new SeasonBuilder().WithId(14).WithYear(2000).WithName("Fall").Build(),
			};

			await dbContext.AddRangeAsync(seasons);
			await dbContext.SaveChangesAsync();

			var jikanServiceBuilder = new JikanServiceBuilder().WithLastSeasonArchiveReturned(returnedLastSeason);
			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			// When
			await handler.HandleAsync();

			// Then
			dbContext.AnimeSeasons.Should().HaveCount(4);
		}

		private UpdateSeasonsHandler CreateHandler(SeiyuuMoeContext dbcontext, IMalApiService apiService)
		{
			var seasonRepository = new SeasonRepository(dbcontext);

			return new UpdateSeasonsHandler(seasonRepository, apiService);
		}
	}
}
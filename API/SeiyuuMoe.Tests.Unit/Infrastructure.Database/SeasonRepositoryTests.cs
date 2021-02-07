using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Infrastructure.Database.Animes;
using SeiyuuMoe.Tests.Common.Builders.Model;
using SeiyuuMoe.Tests.Common.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tests.Infrastructure.Database
{
	public class SeasonRepositoryTests
	{
		[Fact]
		public async Task AddAsync_GivenEmptySeason_ShouldAddAnime()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRepository(dbContext);

			// When
			await repository.AddAsync(new SeasonBuilder().Build());

			// Then
			var allSeasons = await dbContext.AnimeSeasons.ToListAsync();

			allSeasons.Should().ContainSingle();
		}

		[Fact]
		public async Task AddAsync_GivenAnime_ShouldAddAnime()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRepository(dbContext);

			const string expectedName = "ExpectedName";
			const int expectedYear = 2000;

			var season = new SeasonBuilder()
				.WithName(expectedName)
				.WithYear(expectedYear)
				.Build();

			// When
			await repository.AddAsync(season);

			// Then
			var allSeasons = await dbContext.AnimeSeasons.ToListAsync();
			var newSeason = await dbContext.AnimeSeasons.FirstOrDefaultAsync();

			using (new AssertionScope())
			{
				allSeasons.Should().ContainSingle();

				newSeason.Should().NotBeNull();
				newSeason.Name.Should().Be(expectedName);
				newSeason.Year.Should().Be(expectedYear);
			}
		}

		[Fact]
		public async Task AddAsync_GivenDuplicatedKeySeason_ShouldThrowException()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRepository(dbContext);

			await repository.AddAsync(new SeasonBuilder().WithId(1).Build());

			// When
			Func<Task> func = repository.Awaiting(x => x.AddAsync(new SeasonBuilder().WithId(1).Build()));

			// Then
			func.Should().Throw<Exception>();
		}

		[Fact]
		public async Task GetAsync_GivenNoSeasons_ShouldReturnNull()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRepository(dbContext);

			// When
			var result = await repository.GetAsync(x => true);

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetAsync_GivenSingleSeasons_ShouldReturn()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRepository(dbContext);
			var season1 = new SeasonBuilder().WithName("Test1").Build();

			await dbContext.AnimeSeasons.AddAsync(season1);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAsync(x => true);

			// Then
			result.Name.Should().Be("Test1");
		}

		[Fact]
		public async Task GetAsync_GivenSingleNotMatchingSeason_ShouldReturnNull()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRepository(dbContext);
			var season1 = new SeasonBuilder().WithName("Test1").Build();

			await dbContext.AnimeSeasons.AddAsync(season1);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAsync(x => x.Name.EndsWith("3"));

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetAsync_GivenMultipleNotMatchingSeason_ShouldReturnNull()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRepository(dbContext);
			var season1 = new SeasonBuilder().WithName("Test1").Build();
			var season2 = new SeasonBuilder().WithName("Test2").Build();
			var season3 = new SeasonBuilder().WithName("Test3").Build();

			await dbContext.AnimeSeasons.AddAsync(season1);
			await dbContext.AnimeSeasons.AddAsync(season2);
			await dbContext.AnimeSeasons.AddAsync(season3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAsync(x => x.Name.EndsWith("5"));

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetAsync_GivenMultipleSeasonsWithPredicate_ShouldReturn()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRepository(dbContext);
			var season1 = new SeasonBuilder().WithName("Test1").Build();
			var season2 = new SeasonBuilder().WithName("Test2").Build();
			var season3 = new SeasonBuilder().WithName("Test3").Build();

			await dbContext.AnimeSeasons.AddAsync(season1);
			await dbContext.AnimeSeasons.AddAsync(season2);
			await dbContext.AnimeSeasons.AddAsync(season3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAsync(x => x.Name.EndsWith("3"));

			// Then
			result.Name.Should().Be("Test3");
		}
	}
}
using FluentAssertions;
using SeiyuuMoe.Infrastructure.Animes;
using SeiyuuMoe.Tests.Unit.Builders.Model;
using SeiyuuMoe.Tests.Unit.Helpers;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tests.Infrastructure
{
	public class AnimeStatusRepositoryTests
	{
		[Fact]
		public async Task GetByNameAsync_GivenEmptyString_ShouldReturnNull()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeStatusRepository(dbContext);

			// When
			var result = await repository.GetByNameAsync(string.Empty);

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetByNameAsync_GivenNoStatuses_ShouldReturnNull()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeStatusRepository(dbContext);

			// When
			var result = await repository.GetByNameAsync("Airing");

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetByNameAsync_GivenNoExistingStatus_ShouldReturnNull()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeStatusRepository(dbContext);
			var status = new AnimeStatusBuilder().WithName("To be aired").Build();

			await dbContext.AnimeStatuses.AddAsync(status);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetByNameAsync("Airing");

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetByNameAsync_GivenExistingStatus_ShouldReturnResult()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeStatusRepository(dbContext);
			var status = new AnimeStatusBuilder().WithName("Airing").Build();

			await dbContext.AnimeStatuses.AddAsync(status);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetByNameAsync("Airing");

			// Then
			result.Should().NotBeNull();
			result.Description.Should().Be("Airing");
		}
	}
}
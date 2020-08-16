using FluentAssertions;
using SeiyuuMoe.Infrastructure.Animes;
using SeiyuuMoe.Tests.Unit.Builders.Model;
using SeiyuuMoe.Tests.Unit.Helpers;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tests.Infrastructure
{
	public class AnimeTypeRepositoryTests
	{
		[Fact]
		public async Task GetByNameAsync_GivenEmptyString_ShouldReturnNull()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeTypeRepository(dbContext);

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
			var repository = new AnimeTypeRepository(dbContext);

			// When
			var result = await repository.GetByNameAsync("TV");

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetByNameAsync_GivenNoExistingStatus_ShouldReturnNull()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeTypeRepository(dbContext);
			var type = new AnimeTypeBuilder().WithName("Movie").Build();

			await dbContext.AnimeTypes.AddAsync(type);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetByNameAsync("TV");

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetByNameAsync_GivenNoExistingStatus_ShouldReturnResult()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeTypeRepository(dbContext);
			var type = new AnimeTypeBuilder().WithName("TV").Build();

			await dbContext.AnimeTypes.AddAsync(type);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetByNameAsync("TV");

			// Then
			result.Should().NotBeNull();
			result.Description.Should().Be("TV");
		}
	}
}
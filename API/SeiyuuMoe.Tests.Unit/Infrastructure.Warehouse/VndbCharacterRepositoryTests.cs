using FluentAssertions;
using FluentAssertions.Execution;
using SeiyuuMoe.Infrastructure.Warehouse.Repositories;
using SeiyuuMoe.Infrastructure.Warehouse.VndbEntities;
using SeiyuuMoe.Tests.Common.Helpers;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Infrastructure.Warehouse
{
	public class VndbCharacterRepositoryTests
	{
		[Fact]
		public async Task GetCountAsync_GivenEmptyTable_ShouldReturnZero()
		{
			// Given
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbCharacterRepository(warehouseContext);

			// When
			var count = await repository.GetCountAsync();

			// Then
			count.Should().Be(0);
		}

		[Fact]
		public async Task GetCountAsync_GivenSingleCharacter_ShouldReturnOne()
		{
			// Given
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbCharacterRepository(warehouseContext);

			var character = new VndbCharacter();
			await warehouseContext.Characters.AddAsync(character);
			await warehouseContext.SaveChangesAsync();

			// When
			var count = await repository.GetCountAsync();

			// Then
			count.Should().Be(1);
		}

		[Fact]
		public async Task GetCountAsync_GivenMultipleCharacters_ShouldReturnThree()
		{
			// Given
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbCharacterRepository(warehouseContext);

			var character1 = new VndbCharacter();
			var character2 = new VndbCharacter();
			var character3 = new VndbCharacter();
			await warehouseContext.Characters.AddAsync(character1);
			await warehouseContext.Characters.AddAsync(character2);
			await warehouseContext.Characters.AddAsync(character3);
			await warehouseContext.SaveChangesAsync();

			// When
			var count = await repository.GetCountAsync();

			// Then
			count.Should().Be(3);
		}

		[Fact]
		public async Task GetOrderedPageByAsync_GivenEmptyTable_ShouldReturnEmpty()
		{
			// Given
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbCharacterRepository(warehouseContext);

			// When
			var result = await repository.GetOrderedPageByAsync();

			// Then
			using (new AssertionScope())
			{
				result.TotalCount.Should().Be(0);
				result.Results.Should().BeEmpty();
			}
		}

		[Fact]
		public async Task GetOrderedPageAsync_GivenMultipleAnime_ShouldReturnAll()
		{
			// Given
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbCharacterRepository(warehouseContext);

			var character1 = new VndbCharacter();
			var character2 = new VndbCharacter();
			var character3 = new VndbCharacter();
			await warehouseContext.Characters.AddAsync(character1);
			await warehouseContext.Characters.AddAsync(character2);
			await warehouseContext.Characters.AddAsync(character3);
			await warehouseContext.SaveChangesAsync();

			// When
			var result = await repository.GetOrderedPageByAsync();

			// Then
			using (new AssertionScope())
			{
				result.TotalCount.Should().Be(3);
				result.Results.Should().HaveCount(3);
			}
		}

		[Fact]
		public async Task GetOrderedPageAsync_GivenMultipleWithPagesize_ShouldReturnOnlyOnePage()
		{
			// Given
			const int pageSize = 2;

			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbCharacterRepository(warehouseContext);

			var character1 = new VndbCharacter();
			var character2 = new VndbCharacter();
			var character3 = new VndbCharacter();
			await warehouseContext.Characters.AddAsync(character1);
			await warehouseContext.Characters.AddAsync(character2);
			await warehouseContext.Characters.AddAsync(character3);
			await warehouseContext.SaveChangesAsync();

			// When
			var result = await repository.GetOrderedPageByAsync(0, pageSize);

			// Then
			using (new AssertionScope())
			{
				result.TotalCount.Should().Be(3);
				result.Results.Should().HaveCount(pageSize);
				result.PageSize.Should().Be(2);
			}
		}

		[Fact]
		public async Task GetOrderedPageAsync_GivenMultipleWithPagesizeAndPage_ShouldReturnOnlyOnePage()
		{
			// Given
			const int pageSize = 2;
			const int pageNumber = 1;

			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbCharacterRepository(warehouseContext);

			var character1 = new VndbCharacter();
			var character2 = new VndbCharacter();
			var character3 = new VndbCharacter();
			await warehouseContext.Characters.AddAsync(character1);
			await warehouseContext.Characters.AddAsync(character2);
			await warehouseContext.Characters.AddAsync(character3);
			await warehouseContext.SaveChangesAsync();

			// When
			var result = await repository.GetOrderedPageByAsync(pageNumber, pageSize);

			// Then
			using (new AssertionScope())
			{
				result.TotalCount.Should().Be(3);
				result.Results.Should().HaveCount(1);
				result.PageSize.Should().Be(pageSize);
				result.Page.Should().Be(pageNumber);
			}
		}
	}
}
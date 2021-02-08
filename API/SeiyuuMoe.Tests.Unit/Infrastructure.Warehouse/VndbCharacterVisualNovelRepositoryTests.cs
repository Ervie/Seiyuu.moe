using FluentAssertions;
using SeiyuuMoe.Infrastructure.Warehouse.Repositories;
using SeiyuuMoe.Infrastructure.Warehouse.VndbEntities;
using SeiyuuMoe.Tests.Common.Helpers;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Infrastructure.Warehouse
{
	public class VndbCharacterVisualNovelRepositoryTests
	{
		[Fact]
		public async Task GetAsync_GivenEmptyTable_ShouldReturnNull()
		{
			// Given
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbCharacterVisualNovelRepository(warehouseContext);

			// When
			var count = await repository.GetAsync(1, 1);

			// Then
			count.Should().BeNull();
		}
	}
}
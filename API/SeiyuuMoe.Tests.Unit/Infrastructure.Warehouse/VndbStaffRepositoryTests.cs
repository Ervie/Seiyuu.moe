using FluentAssertions;
using FluentAssertions.Execution;
using SeiyuuMoe.Infrastructure.Warehouse.Repositories;
using SeiyuuMoe.Infrastructure.Warehouse.VndbEntities;
using SeiyuuMoe.Tests.Common.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Infrastructure.Warehouse
{
	public class VndbStaffRepositoryTests
	{
		[Fact]
		public async Task GetSeiyuuAsync_GivenEmptyTable_ShouldThrow()
		{
			// Given
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbStaffRepository(warehouseContext);

			// When
			var func = repository.Awaiting(x => x.GetSeiyuuAsync(1));

			// Then
			await func.Should().ThrowAsync<Exception>();
		}

		[Fact]
		public async Task GetSeiyuuAsync_GivenMatchingSeiyuu_ShouldReturnWithStaffAlias()
		{
			// Given
			const string language = "ja";
			const string description = "Test description";
			const int vndbId = 10;
			const string name = "Test name";
			const string originalName = "Test original name";

			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbStaffRepository(warehouseContext);

			var seiyuu = new VndbStaff
			{
				Id = vndbId,
				Description = description,
				Language = language,
				MainAlias = new VndbStaffAlias
				{
					Name = name,
					OriginalName = originalName,
				}
			};
			await warehouseContext.AddAsync(seiyuu);
			await warehouseContext.SaveChangesAsync();

			// When
			var result = await repository.GetSeiyuuAsync(vndbId);

			// Then
			using (new AssertionScope())
			{
				result.Description.Should().Be(description);
				result.Language.Should().Be(language);
				result.MainAlias.Name.Should().Be(name);
				result.MainAlias.OriginalName.Should().Be(originalName);
			}
		}
	}
}
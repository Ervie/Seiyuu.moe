using FluentAssertions;
using FluentAssertions.Execution;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Database.Seiyuus;
using SeiyuuMoe.Infrastructure.Warehouse;
using SeiyuuMoe.Infrastructure.Warehouse.Repositories;
using SeiyuuMoe.Infrastructure.Warehouse.VndbEntities;
using SeiyuuMoe.Tests.Common.Builders.Model;
using SeiyuuMoe.Tests.Common.Helpers;
using SeiyuuMoe.VndbBackgroundJobs.Application.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Component.VndbBackgroundJobs
{
	public class MatchSeiyuuHandlerTests
	{
		[Fact]
		public async Task HandleAsync_GivenEmptyWarehouseAndDb_ShouldNotUpdate()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();

			var handler = CreateHandler(dbContext, warehouseContext);

			// When
			await handler.HandleAsync();

			// Then
			dbContext.Seiyuus.Should().BeEmpty();
		}

		[Fact]
		public async Task HandleAsync_GivenEmptyWarehouse_ShouldNotUpdate()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();

			var seiyuuId = Guid.NewGuid();
			var seiyuu = new SeiyuuBuilder()
				.WithId(seiyuuId)
				.Build();

			await dbContext.AddAsync(seiyuu);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, warehouseContext);

			// When
			await handler.HandleAsync();

			// Then
			seiyuu = dbContext.Seiyuus.First();
			seiyuu.VndbId.Should().BeNull();
		}

		[Fact]
		public async Task HandleAsync_GivenAliasWithoutRoles_ShouldNotUpdate()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();

			const string seiyuuName = "沢城 みゆき";

			var vndbSeiyuuId = 10;
			var alias = new VndbStaffAlias { Id = vndbSeiyuuId, OriginalName = seiyuuName };
			var vndbSeiyuu = new VndbStaff { Id = vndbSeiyuuId };
			await warehouseContext.AddAsync(alias);
			await warehouseContext.AddAsync(vndbSeiyuu);
			await warehouseContext.SaveChangesAsync();

			var seiyuuId = Guid.NewGuid();
			var seiyuu = new SeiyuuBuilder()
				.WithId(seiyuuId)
				.WithJapaneseName(seiyuuName)
				.Build();

			await dbContext.AddAsync(seiyuu);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, warehouseContext);

			// When
			await handler.HandleAsync();

			// Then
			seiyuu = dbContext.Seiyuus.First();
			seiyuu.VndbId.Should().BeNull();
		}

		[Fact]
		public async Task HandleAsync_GivenAliasWithRolesOfAnotherNationality_ShouldNotUpdate()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();

			const string seiyuuName = "沢城 みゆき";

			var vndbSeiyuuId = 10;
			var roles = new List<VndbVisualNovelSeiyuu> { new VndbVisualNovelSeiyuu { } };
			var alias = new VndbStaffAlias { Id = vndbSeiyuuId, OriginalName = seiyuuName, Roles = roles };
			var vndbSeiyuu = new VndbStaff { Id = vndbSeiyuuId, Language = "en" };
			await warehouseContext.AddAsync(alias);
			await warehouseContext.AddAsync(vndbSeiyuu);
			await warehouseContext.SaveChangesAsync();

			var seiyuuId = Guid.NewGuid();
			var seiyuu = new SeiyuuBuilder()
				.WithId(seiyuuId)
				.WithJapaneseName(seiyuuName)
				.Build();

			await dbContext.AddAsync(seiyuu);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, warehouseContext);

			// When
			await handler.HandleAsync();

			// Then
			seiyuu = dbContext.Seiyuus.First();
			seiyuu.VndbId.Should().BeNull();
		}

		[Fact]
		public async Task HandleAsync_GivenAliasWithRolesSeiyuuAlreadyMatched_ShouldNotUpdate()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();

			const string seiyuuName = "沢城 みゆき";
			const string seiyuuNameInDb = "沢城 みゆ";

			var vndbSeiyuuId = 10;
			var roles = new List<VndbVisualNovelSeiyuu> { new VndbVisualNovelSeiyuu { } };
			var alias = new VndbStaffAlias { Id = vndbSeiyuuId, OriginalName = seiyuuName, Roles = roles };
			var vndbSeiyuu = new VndbStaff { Id = vndbSeiyuuId, Language = "ja" };
			await warehouseContext.AddAsync(alias);
			await warehouseContext.AddAsync(vndbSeiyuu);
			await warehouseContext.SaveChangesAsync();

			var seiyuuId = Guid.NewGuid();
			var seiyuu = new SeiyuuBuilder()
				.WithId(seiyuuId)
				.WithVndbId(vndbSeiyuuId)
				.WithJapaneseName(seiyuuNameInDb)
				.Build();

			await dbContext.AddAsync(seiyuu);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, warehouseContext);

			// When
			await handler.HandleAsync();

			// Then
			seiyuu = dbContext.Seiyuus.First();
			using (new AssertionScope())
			{
				seiyuu.VndbId.Should().Be(vndbSeiyuuId);
				seiyuu.KanjiName.Should().Be(seiyuuNameInDb);
			}
		}

		private MatchSeiyuuHandler CreateHandler(SeiyuuMoeContext seiyuuMoeContext, WarehouseDbContext warehouseDbContext)
		{
			var seiyuuRepository = new SeiyuuRepository(seiyuuMoeContext);
			var vndbStaffRepository = new VndbStaffRepository(warehouseDbContext);
			var vndbStaffAliasRepository = new VndbStaffAliasRepository(warehouseDbContext);

			return new MatchSeiyuuHandler(vndbStaffAliasRepository, vndbStaffRepository, seiyuuRepository);
		}
	}
}
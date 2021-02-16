using FluentAssertions;
using FluentAssertions.Execution;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Database.VisualNovels;
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
	public class AddOrUpdateVisualNovelsHandlerTests
	{
		[Fact]
		public async Task HandleAsync_GivenEmptyWarehouse_ShouldNotInsertAnything()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();

			var handler = CreateHandler(dbContext, warehouseContext);

			// When
			await handler.HandleAsync();

			// Then
			dbContext.VisualNovels.Should().BeEmpty();
		}

		[Fact]
		public async Task HandleAsync_GivenSingleVnInWarehouse_ShouldInsertSingle()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();

			const int vnVndbId = 10;
			const string vnTitle = "TestTitle";
			const string vnAlias = "TestAlias";
			const string vnDescription = "TestDescription";
			const string vnTitleOriginal = "TestTitleOriginal";
			const int vnVoteCount = 3000;

			var vndbVn = new VndbVisualNovel
			{
				Id = vnVndbId,
				Title = vnTitle,
				Alias = vnAlias,
				Description = vnDescription,
				TitleOriginal = vnTitleOriginal,
				VoteCount = vnVoteCount
			};
			await warehouseContext.AddAsync(vndbVn);
			await warehouseContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, warehouseContext);

			// When
			await handler.HandleAsync();

			// Then
			using (new AssertionScope())
			{
				dbContext.VisualNovels.Should().ContainSingle();

				var newVn = dbContext.VisualNovels.First();
				newVn.VndbId.Should().Be(vnVndbId);
				newVn.Title.Should().Be(vnTitle);
				newVn.Alias.Should().Be(vnAlias);
				newVn.TitleOriginal.Should().Be(vnTitleOriginal);
				newVn.Popularity.Should().Be(vnVoteCount);
				newVn.About.Should().Be(vnDescription);
				newVn.ImageUrl.Should().BeNull();
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSingleVnWithImageInWarehouse_ShouldInsertSingleWithParsedImageUrl()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();

			const int vnVndbId = 10;
			const string imageId = "cv1234";

			var vndbVn = new VndbVisualNovel
			{
				Id = vnVndbId,
				Image = imageId
			};
			await warehouseContext.AddAsync(vndbVn);
			await warehouseContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, warehouseContext);

			// When
			await handler.HandleAsync();

			// Then
			using (new AssertionScope())
			{
				dbContext.VisualNovels.Should().ContainSingle();

				var newVn = dbContext.VisualNovels.First();
				newVn.VndbId.Should().Be(vnVndbId);
				newVn.ImageUrl.Should().Be("https://s2.vndb.org/cv/34/1234.jpg");
			}
		}

		[Fact]
		public async Task HandleAsync_GivenMultipleVnInWarehouse_ShouldInsertMultiple()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();

			var visualNovelsInWarehouse = new List<VndbVisualNovel>
			{
				new VndbVisualNovel { Id = 1 },
				new VndbVisualNovel { Id = 2 },
				new VndbVisualNovel { Id = 3 },
			};
			await warehouseContext.AddRangeAsync(visualNovelsInWarehouse);
			await warehouseContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, warehouseContext);

			// When
			await handler.HandleAsync();

			// Then
			dbContext.VisualNovels.Should().HaveSameCount(visualNovelsInWarehouse);
		}

		[Fact]
		public async Task HandleAsync_GivenSingleVnInWarehouseAlreadyInserted_ShouldUpdate()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();

			var vnId = Guid.NewGuid();
			const int vnVndbId = 10;
			const string vnTitle = "TestTitle";
			const string vnAlias = "TestAlias";
			const string vnDescription = "TestDescription";
			const string vnTitleOriginal = "TestTitleOriginal";
			const string imageId = "cv1234";
			const int vnVoteCount = 3000;

			var vndbVn = new VndbVisualNovel
			{
				Id = vnVndbId,
				Title = vnTitle,
				Alias = vnAlias,
				Description = vnDescription,
				TitleOriginal = vnTitleOriginal,
				VoteCount = vnVoteCount,
				Image = imageId
			};
			await warehouseContext.AddAsync(vndbVn);
			await warehouseContext.SaveChangesAsync();

			var existingVn = new VisualNovelBuilder()
				.WithId(vnId)
				.WithVndbId(vnVndbId)
				.Build();
			await dbContext.AddAsync(existingVn);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, warehouseContext);

			// When
			await handler.HandleAsync();

			// Then
			using (new AssertionScope())
			{
				dbContext.VisualNovels.Should().ContainSingle();

				var newVn = dbContext.VisualNovels.First();
				newVn.Id.Should().Be(vnId);
				newVn.VndbId.Should().Be(vnVndbId);
				newVn.Title.Should().Be(vnTitle);
				newVn.Alias.Should().Be(vnAlias);
				newVn.TitleOriginal.Should().Be(vnTitleOriginal);
				newVn.Popularity.Should().Be(vnVoteCount);
				newVn.About.Should().Be(vnDescription);
				newVn.ImageUrl.Should().Be("https://s2.vndb.org/cv/34/1234.jpg");
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSingleVnInWarehouseOtherThanInserted_ShouldInsertSingle()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();

			var vnId = Guid.NewGuid();
			const int vnVndbId = 10;

			var vndbVn = new VndbVisualNovel
			{
				Id = vnVndbId,
			};
			await warehouseContext.AddAsync(vndbVn);
			await warehouseContext.SaveChangesAsync();

			var existingVn = new VisualNovelBuilder()
				.WithId(vnId)
				.WithVndbId(20)
				.Build();
			await dbContext.AddAsync(existingVn);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, warehouseContext);

			// When
			await handler.HandleAsync();

			// Then
			dbContext.VisualNovels.Should().HaveCount(2);
		}

		[Fact]
		public async Task HandleAsync_GivenMultipleVnInWarehouseOtherThanInserted_ShouldInsertMultiple()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();

			var visualNovelsInWarehouse = new List<VndbVisualNovel>
			{
				new VndbVisualNovel { Id = 1 },
				new VndbVisualNovel { Id = 2 },
				new VndbVisualNovel { Id = 3 },
			};
			await warehouseContext.AddRangeAsync(visualNovelsInWarehouse);
			await warehouseContext.SaveChangesAsync();

			var existingVn = new VisualNovelBuilder()
				.WithId(Guid.NewGuid())
				.WithVndbId(4)
				.Build();
			await dbContext.AddAsync(existingVn);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, warehouseContext);

			// When
			await handler.HandleAsync();

			// Then
			dbContext.VisualNovels.Should().HaveCount(4);
		}

		[Fact]
		public async Task HandleAsync_GivenMultipleVnInWarehouseSomeInDb_ShouldInsertNewAndUpdateOld()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();

			var visualNovelsInWarehouse = new List<VndbVisualNovel>
			{
				new VndbVisualNovel { Id = 1, Title = "Title 1" },
				new VndbVisualNovel { Id = 2, Title = "Title 2 new" },
				new VndbVisualNovel { Id = 3, Title = "Title 3" },
			};
			await warehouseContext.AddRangeAsync(visualNovelsInWarehouse);
			await warehouseContext.SaveChangesAsync();

			var existingVn = new VisualNovelBuilder()
				.WithId(Guid.NewGuid())
				.WithVndbId(2)
				.WithTitle("Title 2 new")
				.Build();
			await dbContext.AddAsync(existingVn);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, warehouseContext);

			// When
			await handler.HandleAsync();

			// Then
			using (new AssertionScope())
			{
				dbContext.VisualNovels.Should().HaveSameCount(visualNovelsInWarehouse);

				var updated = dbContext.VisualNovels.First();
				updated.VndbId.Should().Be(2);
				updated.Title.Should().Be("Title 2 new");
				updated.ImageUrl.Should().BeNull();
			}
		}

		private AddOrUpdateVisualNovelsHandler CreateHandler(SeiyuuMoeContext seiyuuMoeContext, WarehouseDbContext warehouseDbContext)
		{
			var visualNovelRepository = new VisualNovelRepository(seiyuuMoeContext);
			var vndbVisualNovelRepository = new VndbVisualNovelRepository(warehouseDbContext);

			return new AddOrUpdateVisualNovelsHandler(visualNovelRepository, vndbVisualNovelRepository);
		}
	}
}
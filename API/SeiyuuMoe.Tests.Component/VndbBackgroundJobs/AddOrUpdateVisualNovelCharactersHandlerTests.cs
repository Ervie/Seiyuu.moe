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
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Component.VndbBackgroundJobs
{
	public class AddOrUpdateVisualNovelCharactersHandlerTests
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
		public async Task HandleAsync_GivenSingleVnCharacterInWarehouse_ShouldInsertSingle()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();

			const int characterVndbId = 10;
			const string characterName = "CharacterName";
			const string vnAlias = "TestAlias";
			const string characterDescription = "CharacterDescription";
			const string characterNameOriginal = "CharacterNameOriginal";

			var vndbCharacter = new VndbCharacter
			{
				Id = characterVndbId,
				Name = characterName,
				Alias = vnAlias,
				Description = characterDescription,
				NameOriginal = characterNameOriginal,
			};
			await warehouseContext.AddAsync(vndbCharacter);
			await warehouseContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, warehouseContext);

			// When
			await handler.HandleAsync();

			// Then
			using (new AssertionScope())
			{
				dbContext.VisualNovelCharacters.Should().ContainSingle();

				var newVn = dbContext.VisualNovelCharacters.First();
				newVn.VndbId.Should().Be(characterVndbId);
				newVn.Name.Should().Be(characterName);
				newVn.KanjiName.Should().Be(characterNameOriginal);
				newVn.About.Should().Be(characterDescription);
				newVn.ImageUrl.Should().BeNull();
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSingleVnCharacterWithImageInWarehouse_ShouldInsertSingleWithParsedImageUrl()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();

			const int characterVndbId = 10;
			const string imageId = "ch4321";

			var vndbCharacter = new VndbCharacter
			{
				Id = characterVndbId,
				Image = imageId
			};
			await warehouseContext.AddAsync(vndbCharacter);
			await warehouseContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, warehouseContext);

			// When
			await handler.HandleAsync();

			// Then
			using (new AssertionScope())
			{
				dbContext.VisualNovelCharacters.Should().ContainSingle();

				var newVn = dbContext.VisualNovelCharacters.First();
				newVn.VndbId.Should().Be(characterVndbId);
				newVn.ImageUrl.Should().Be("https://s2.vndb.org/ch/21/4321.jpg");
			}
		}

		[Fact]
		public async Task HandleAsync_GivenMultipleVnCharactersInWarehouse_ShouldInsertMultiple()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();

			var vnCharactersInWarehouse = new List<VndbCharacter>
			{
				new VndbCharacter { Id = 1 },
				new VndbCharacter { Id = 2 },
				new VndbCharacter { Id = 3 },
			};
			await warehouseContext.AddRangeAsync(vnCharactersInWarehouse);
			await warehouseContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, warehouseContext);

			// When
			await handler.HandleAsync();

			// Then
			dbContext.VisualNovelCharacters.Should().HaveSameCount(vnCharactersInWarehouse);
		}

		[Fact]
		public async Task HandleAsync_GivenSingleVnCharacterInWarehouseAlreadyInserted_ShouldUpdate()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();

			var characterId = Guid.NewGuid();
			const int characterVndbId = 10;
			const string characterName = "CharacterName";
			const string vnAlias = "TestAlias";
			const string characterDescription = "CharacterDescription";
			const string characterNameOriginal = "CharacterNameOriginal";
			const string imageId = "ch4321";

			var vndbCharacter = new VndbCharacter
			{
				Id = characterVndbId,
				Name = characterName,
				Alias = vnAlias,
				Description = characterDescription,
				NameOriginal = characterNameOriginal,
				Image = imageId
			};
			await warehouseContext.AddAsync(vndbCharacter);
			await warehouseContext.SaveChangesAsync();

			var existingVn = new VisualNovelCharacterBuilder()
				.WithId(characterId)
				.WithVndbId(characterVndbId)
				.Build();
			await dbContext.AddAsync(existingVn);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, warehouseContext);

			// When
			await handler.HandleAsync();

			// Then
			using (new AssertionScope())
			{
				dbContext.VisualNovelCharacters.Should().ContainSingle();

				var newVn = dbContext.VisualNovelCharacters.First();
				newVn.VndbId.Should().Be(characterVndbId);
				newVn.Name.Should().Be(characterName);
				newVn.KanjiName.Should().Be(characterNameOriginal);
				newVn.About.Should().Be(characterDescription);
				newVn.ImageUrl.Should().Be("https://s2.vndb.org/ch/21/4321.jpg");
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSingleVnCharacterInWarehouseOtherThanInserted_ShouldInsertSingle()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();

			var characterId = Guid.NewGuid();
			const int characterVndbId = 10;

			var vndbCharacter = new VndbCharacter
			{
				Id = characterVndbId,
			};
			await warehouseContext.AddAsync(vndbCharacter);
			await warehouseContext.SaveChangesAsync();

			var existingVn = new VisualNovelCharacterBuilder()
				.WithId(characterId)
				.WithVndbId(20)
				.Build();
			await dbContext.AddAsync(existingVn);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, warehouseContext);

			// When
			await handler.HandleAsync();

			// Then
			dbContext.VisualNovelCharacters.Should().HaveCount(2);
		}

		[Fact]
		public async Task HandleAsync_GivenMultipleVnCharacterInWarehouseOtherThanInserted_ShouldInsertMultiple()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();

			var vnCharactersInWarehouse = new List<VndbCharacter>
			{
				new VndbCharacter { Id = 1 },
				new VndbCharacter { Id = 2 },
				new VndbCharacter { Id = 3 },
			};
			await warehouseContext.AddRangeAsync(vnCharactersInWarehouse);
			await warehouseContext.SaveChangesAsync();

			var existingVn = new VisualNovelCharacterBuilder()
				.WithId(Guid.NewGuid())
				.WithVndbId(4)
				.Build();
			await dbContext.AddAsync(existingVn);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, warehouseContext);

			// When
			await handler.HandleAsync();

			// Then
			dbContext.VisualNovelCharacters.Should().HaveCount(4);
		}

		[Fact]
		public async Task HandleAsync_GivenMultipleVnCharacterInWarehouseSomeInDb_ShouldInsertNewAndUpdateOld()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();

			var charactersInWarehouse = new List<VndbCharacter>
			{
				new VndbCharacter { Id = 1, Name = "Name 1" },
				new VndbCharacter { Id = 2, Name = "Name 2 new" },
				new VndbCharacter { Id = 3, Name = "Name 3" },
			};
			await warehouseContext.AddRangeAsync(charactersInWarehouse);
			await warehouseContext.SaveChangesAsync();

			var existingVn = new VisualNovelCharacterBuilder()
				.WithId(Guid.NewGuid())
				.WithVndbId(2)
				.WithName("Name 2")
				.Build();
			await dbContext.AddAsync(existingVn);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, warehouseContext);

			// When
			await handler.HandleAsync();

			// Then
			using (new AssertionScope())
			{
				dbContext.VisualNovelCharacters.Should().HaveSameCount(charactersInWarehouse);

				var updated = dbContext.VisualNovelCharacters.First();
				updated.VndbId.Should().Be(2);
				updated.Name.Should().Be("Name 2 new");
				updated.ImageUrl.Should().BeNull();
			}
		}

		private AddOrUpdateVisualNovelCharactersHandler CreateHandler(SeiyuuMoeContext seiyuuMoeContext, WarehouseDbContext warehouseDbContext)
		{
			var visualNovelCharacterRepository = new VisualNovelCharacterRepository(seiyuuMoeContext);
			var vndbCharacterRepository = new VndbCharacterRepository(warehouseDbContext);

			return new AddOrUpdateVisualNovelCharactersHandler(visualNovelCharacterRepository, vndbCharacterRepository);
		}
	}
}

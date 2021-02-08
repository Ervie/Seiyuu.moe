using FluentAssertions;
using SeiyuuMoe.Infrastructure.Database.VisualNovels;
using SeiyuuMoe.Tests.Common.Builders.Model;
using SeiyuuMoe.Tests.Common.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Infrastructure.Database
{
	public class VisualNovelRoleRepositoryTests
	{
		[Fact]
		public async Task InsertRoleAsync_GivenEmptyRole_ShouldAddRole()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new VisualNovelRoleRepository(dbContext);

			// When
			await repository.InsertRoleAsync(new VisualNovelRoleBuilder().Build());

			// Then
			dbContext.VisualNovelRoles.Should().ContainSingle();
		}

		[Fact]
		public async Task RoleExistsAsync_GivenEmptyTable_ShouldReturnFalse()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new VisualNovelRoleRepository(dbContext);

			// When
			var result = await repository.RoleExistsAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

			// Then
			result.Should().BeFalse();
		}

		[Fact]
		public async Task RoleExistsAsync_GivenOnlyPartialMatch_ShouldReturnFalse()
		{
			// Given
			var characterId = Guid.NewGuid();

			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new VisualNovelRoleRepository(dbContext);

			var visualNovelRole = new VisualNovelRoleBuilder().WithCharacterId(characterId).Build();
			await dbContext.VisualNovelRoles.AddAsync(visualNovelRole);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.RoleExistsAsync(Guid.NewGuid(), characterId, Guid.NewGuid());

			// Then
			result.Should().BeFalse();
		}

		[Fact]
		public async Task RoleExistsAsync_GivenFullMatch_ShouldReturnTrue()
		{
			// Given
			var characterId = Guid.NewGuid();
			var visualNovelId = Guid.NewGuid();
			var seiyuuId = Guid.NewGuid();

			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new VisualNovelRoleRepository(dbContext);

			var visualNovelRole = new VisualNovelRoleBuilder()
				.WithCharacterId(characterId)
				.WithVisualNovelId(visualNovelId)
				.WithSeiyuuId(seiyuuId)
				.Build();
			await dbContext.VisualNovelRoles.AddAsync(visualNovelRole);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.RoleExistsAsync(visualNovelId, characterId, seiyuuId);

			// Then
			result.Should().BeTrue();
		}
	}
}
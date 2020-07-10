using FluentAssertions;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Infrastructure.Seiyuus;
using SeiyuuMoe.Tests.Unit.Builders.Model;
using SeiyuuMoe.Tests.Unit.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tests.Infrastructure
{
	public class SeiyuuRoleRepositoryTests
	{
		[Fact]
		public async Task GetAllSeiyuuRolesAsync_GivenNoRoles_ShouldReturnEmpty()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRoleRepository(dbContext);

			// When
			var result = await repository.GetAllSeiyuuRolesAsync(1, false);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllSeiyuuRolesAsync_GivenNoRolesForAnime_ShouldReturnEmpty()
		{
			// Given
			const int seiyuuMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRoleRepository(dbContext);

			var seiyuu = new SeiyuuBuilder().WithMalId(2).Build();

			await dbContext.AddAsync(seiyuu);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllSeiyuuRolesAsync(seiyuuMalId, false);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllSeiyuuRolesAsync_GivenAnimeWithSingleRole_ShouldReturnSingle()
		{
			// Given
			const int seiyuuMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRoleRepository(dbContext);

			var role = new RoleBuilder()
				.WithSeiyuu(
					x => x.WithMalId(seiyuuMalId)
				)
				.WithLanguage(x => x.WithName("Japanese").WithId(1))
				.Build();

			await dbContext.AddAsync(role);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllSeiyuuRolesAsync(seiyuuMalId, false);

			// Then
			result.Should().ContainSingle();
		}

		[Fact]
		public async Task GetAllSeiyuuRolesAsync_GivenAnimeWithSingleRoleInLanguageOtherThanJapanese_ShouldReturnEmpty()
		{
			// Given
			const int seiyuuMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRoleRepository(dbContext);

			var role = new RoleBuilder()
				.WithSeiyuu(
					x => x.WithMalId(seiyuuMalId)
				)
				.WithLanguage(x => x.WithName("Test").WithId(9999))
				.Build();

			await dbContext.AddAsync(role);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllSeiyuuRolesAsync(seiyuuMalId, false);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllSeiyuuRolesAsync_GivenMultipleRoles_ShouldReturnMultiple()
		{
			// Given
			const int animeMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRoleRepository(dbContext);

			var japanese = new LanguageBuilder().WithId(1).WithName("Japanese").Build();
			var seiyuu = new SeiyuuBuilder().WithMalId(animeMalId).Build();
			seiyuu.Role = new List<Role>
			{
				new RoleBuilder().WithLanguage(japanese).Build(),
				new RoleBuilder().WithLanguage(japanese).Build(),
				new RoleBuilder().WithLanguage(japanese).Build(),
				new RoleBuilder().WithLanguage(japanese).Build(),
				new RoleBuilder().WithLanguage(japanese).Build(),
			};

			await dbContext.AddAsync(seiyuu);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllSeiyuuRolesAsync(animeMalId, false);

			// Then
			result.Should().HaveCount(5);
		}

		[Fact]
		public async Task GetAllSeiyuuRolesAsync_GivenSeiyuuWithMultipleRolesAndMainRolesOnly_ShouldReturnPartial()
		{
			// Given
			const int seiyuuMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRoleRepository(dbContext);

			var japanese = new LanguageBuilder().WithId(1).WithName("Japanese").Build();
			var seiyuu = new SeiyuuBuilder().WithMalId(seiyuuMalId).Build();
			var mainRole = new RoleTypeBuilder().WithName("Main").WithId(1).Build();
			var supportingRole = new RoleTypeBuilder().WithName("Supporting").WithId(2).Build();

			seiyuu.Role = new List<Role>
			{
				new RoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build()
			};

			await dbContext.AddAsync(seiyuu);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllSeiyuuRolesAsync(seiyuuMalId, true);

			// Then
			result.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetAllSeiyuuRolesAsync_GivenSeiyuuWithMultipleRolesAndMainRolesOnlyAndInDifferentLanguages_ShouldReturnPartial()
		{
			// Given
			const int seiyuuMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRoleRepository(dbContext);

			var japanese = new LanguageBuilder().WithId(1).WithName("Japanese").Build();
			var korean = new LanguageBuilder().WithId(2).WithName("Korean").Build();
			var seiyuu = new SeiyuuBuilder().WithMalId(seiyuuMalId).Build();
			var mainRole = new RoleTypeBuilder().WithName("Main").WithId(1).Build();
			var supportingRole = new RoleTypeBuilder().WithName("Supporting").WithId(2).Build();

			seiyuu.Role = new List<Role>
			{
				new RoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build(),
				new RoleBuilder().WithLanguage(korean).WithRoleType(mainRole).Build(),
				new RoleBuilder().WithLanguage(korean).WithRoleType(supportingRole).Build(),
				new RoleBuilder().WithLanguage(korean).WithRoleType(supportingRole).Build(),
				new RoleBuilder().WithLanguage(korean).WithRoleType(supportingRole).Build(),
				new RoleBuilder().WithLanguage(korean).WithRoleType(mainRole).Build()
			};

			await dbContext.AddAsync(seiyuu);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllSeiyuuRolesAsync(seiyuuMalId, true);

			// Then
			result.Should().HaveCount(2);
		}
	}
}
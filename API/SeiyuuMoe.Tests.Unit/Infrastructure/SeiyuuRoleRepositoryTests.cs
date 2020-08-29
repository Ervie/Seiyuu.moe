using FluentAssertions;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Infrastructure.Seiyuus;
using SeiyuuMoe.Tests.Common.Builders.Model;
using SeiyuuMoe.Tests.Common.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tests.Infrastructure
{
	public class SeiyuuRoleRepositoryTests
	{
		[Fact]
		public async Task GetAllSeiyuuRolesByMalIdAsync_GivenNoRoles_ShouldReturnEmpty()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRoleRepository(dbContext);

			// When
			var result = await repository.GetAllSeiyuuRolesByMalIdAsync(1, false);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllSeiyuuRolesByMalIdAsync_GivenNoRolesForAnime_ShouldReturnEmpty()
		{
			// Given
			const int seiyuuMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRoleRepository(dbContext);

			var seiyuu = new SeiyuuBuilder().WithMalId(2).Build();

			await dbContext.AddAsync(seiyuu);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllSeiyuuRolesByMalIdAsync(seiyuuMalId, false);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllSeiyuuRolesByMalIdAsync_GivenAnimeWithSingleRole_ShouldReturnSingle()
		{
			// Given
			const int seiyuuMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRoleRepository(dbContext);

			var role = new RoleBuilder()
				.WithSeiyuu(
					x => x.WithMalId(seiyuuMalId)
				)
				.WithLanguage(x => x.WithDescription("Japanese").WithId(1))
				.Build();

			await dbContext.AddAsync(role);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllSeiyuuRolesByMalIdAsync(seiyuuMalId, false);

			// Then
			result.Should().ContainSingle();
		}

		[Fact]
		public async Task GetAllSeiyuuRolesByMalIdAsync_GivenAnimeWithSingleRoleInLanguageOtherThanJapanese_ShouldReturnEmpty()
		{
			// Given
			const int seiyuuMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRoleRepository(dbContext);

			var role = new RoleBuilder()
				.WithSeiyuu(
					x => x.WithMalId(seiyuuMalId)
				)
				.WithLanguage(x => x.WithDescription("Test").WithId(9999))
				.Build();

			await dbContext.AddAsync(role);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllSeiyuuRolesByMalIdAsync(seiyuuMalId, false);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllSeiyuuRolesByMalIdAsync_GivenMultipleRoles_ShouldReturnMultiple()
		{
			// Given
			const int animeMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRoleRepository(dbContext);

			var japanese = new LanguageBuilder().WithId(1).WithDescription("Japanese").Build();
			var seiyuu = new SeiyuuBuilder().WithMalId(animeMalId).Build();
			seiyuu.Role = new List<AnimeRole>
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
			var result = await repository.GetAllSeiyuuRolesByMalIdAsync(animeMalId, false);

			// Then
			result.Should().HaveCount(5);
		}

		[Fact]
		public async Task GetAllSeiyuuRolesByMalIdAsync_GivenSeiyuuWithMultipleRolesAndMainRolesOnly_ShouldReturnPartial()
		{
			// Given
			const int seiyuuMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRoleRepository(dbContext);

			var japanese = new LanguageBuilder().WithId(1).WithDescription("Japanese").Build();
			var seiyuu = new SeiyuuBuilder().WithMalId(seiyuuMalId).Build();
			var mainRole = new RoleTypeBuilder().WithDescription("Main").WithId(1).Build();
			var supportingRole = new RoleTypeBuilder().WithDescription("Supporting").WithId(2).Build();

			seiyuu.Role = new List<AnimeRole>
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
			var result = await repository.GetAllSeiyuuRolesByMalIdAsync(seiyuuMalId, true);

			// Then
			result.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetAllSeiyuuRolesByMalIdAsync_GivenSeiyuuWithMultipleRolesAndMainRolesOnlyAndInDifferentLanguages_ShouldReturnPartial()
		{
			// Given
			const int seiyuuMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRoleRepository(dbContext);

			var japanese = new LanguageBuilder().WithId(1).WithDescription("Japanese").Build();
			var korean = new LanguageBuilder().WithId(2).WithDescription("Korean").Build();
			var seiyuu = new SeiyuuBuilder().WithMalId(seiyuuMalId).Build();
			var mainRole = new RoleTypeBuilder().WithDescription("Main").WithId(1).Build();
			var supportingRole = new RoleTypeBuilder().WithDescription("Supporting").WithId(2).Build();

			seiyuu.Role = new List<AnimeRole>
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
			var result = await repository.GetAllSeiyuuRolesByMalIdAsync(seiyuuMalId, true);

			// Then
			result.Should().HaveCount(2);
		}
	}
}
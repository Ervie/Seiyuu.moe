using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Repositories.Repositories;
using SeiyuuMoe.Tests.Unit.Builders.Model;
using SeiyuuMoe.Tests.Unit.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tests.Infrastructure
{
	public class AnimeRoleRepositoryTests
	{
		[Fact]
		public async Task AddAsync_GivenEmptyRole_ShouldAddRole()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRoleRepository(dbContext);

			// When
			await repository.AddAsync(new RoleBuilder().Build());

			// Then
			var allRoles = await dbContext.Role.ToListAsync();

			allRoles.Should().ContainSingle();
		}

		[Fact]
		public async Task AddAsync_GivenAnime_ShouldAddAnime()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRoleRepository(dbContext);

			const string expectedAnimeTitle = "expectedTitle";
			const string expectedSeiyuuName = "expectedSeiyuuName";
			const string expectedcharacterName = "expectedCharacterName";
			const string expectedRoleTypeName = "Main";
			const string expectedLanguageName = "Japanese";

			var role = new RoleBuilder()
				.WithAnime(x => x.WithTitle(expectedAnimeTitle))
				.WithSeiyuu(x => x.WithName(expectedSeiyuuName))
				.WithCharacter(x => x.WithName(expectedcharacterName))
				.WithRoleType(x => x.WithName(expectedRoleTypeName))
				.WithLanguage(x => x.WithName(expectedLanguageName))
				.Build();

			// When
			await repository.AddAsync(role);

			// Then
			var allroles = await dbContext.Role.ToListAsync();
			var newRole = await dbContext.Role
				.Include(x => x.Anime)
				.Include(x => x.Character)
				.Include(x => x.Seiyuu)
				.Include(x => x.Language)
				.Include(x => x.RoleType)
				.FirstOrDefaultAsync();

			using (new AssertionScope())
			{
				allroles.Should().ContainSingle();

				newRole.Should().NotBeNull();
				newRole.Anime.Should().NotBeNull();
				newRole.Anime.Title.Should().Be(expectedAnimeTitle);
				newRole.Character.Should().NotBeNull();
				newRole.Character.Name.Should().Be(expectedcharacterName);
				newRole.Seiyuu.Should().NotBeNull();
				newRole.Seiyuu.Name.Should().Be(expectedSeiyuuName);
				newRole.Language.Should().NotBeNull();
				newRole.Language.Name.Should().Be(expectedLanguageName);
				newRole.RoleType.Should().NotBeNull();
				newRole.RoleType.Name.Should().Be(expectedRoleTypeName);
			}
		}

		[Fact]
		public async Task AddAsync_GivenExistingRoleWithSameValues_ShouldAdd()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRoleRepository(dbContext);

			var role1 = new RoleBuilder()
				.Build();

			var role2 = new RoleBuilder()
				.Build();

			await repository.AddAsync(role1);

			// When
			Func<Task> func = repository.Awaiting(x => x.AddAsync(role2));

			// Then
			func.Should().NotThrow<Exception>();
		}

		[Fact]
		public async Task GetAllRolesInAnimeAsync_GivenNoRoles_ShouldReturnEmpty()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRoleRepository(dbContext);

			// When
			var result = await repository.GetAllRolesInAnimeAsync(1);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllRolesInAnimeAsync_GivenNoRolesForAnime_ShouldReturnEmpty()
		{
			// Given
			const int animeMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRoleRepository(dbContext);

			var anime = new AnimeBuilder().WithMalId(2).Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRolesInAnimeAsync(animeMalId);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllRolesInAnimeAsync_GivenAnimeWithSingleRole_ShouldReturnSingle()
		{
			// Given
			const int animeMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRoleRepository(dbContext);

			var role = new RoleBuilder()
				.WithAnime(
					x => x.WithMalId(animeMalId)
				)
				.WithLanguage(x => x.WithName("Japanese").WithId(1))
				.Build();

			await dbContext.AddAsync(role);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRolesInAnimeAsync(animeMalId);

			// Then
			result.Should().ContainSingle();
		}

		[Fact]
		public async Task GetAllRolesInAnimeAsync_GivenAnimeWithSingleRoleInLanguageOtherThanJapanese_ShouldReturnEmpty()
		{
			// Given
			const int animeMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRoleRepository(dbContext);

			var role = new RoleBuilder()
				.WithAnime(
					x => x.WithMalId(animeMalId)
				)
				.WithLanguage(x => x.WithName("Test").WithId(9999))
				.Build();

			await dbContext.AddAsync(role);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRolesInAnimeAsync(animeMalId);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllRolesInAnimeAsync_GivenMultipleRoles_ShouldReturnMultiple()
		{
			// Given
			const int animeMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRoleRepository(dbContext);

			var japanese = new LanguageBuilder().WithId(1).WithName("Japanese").Build();
			var anime = new AnimeBuilder().WithMalId(animeMalId).Build();
			anime.Role = new List<Role>
			{
				new RoleBuilder().WithLanguage(japanese).Build(),
				new RoleBuilder().WithLanguage(japanese).Build(),
				new RoleBuilder().WithLanguage(japanese).Build(),
				new RoleBuilder().WithLanguage(japanese).Build(),
				new RoleBuilder().WithLanguage(japanese).Build(),
			};

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRolesInAnimeAsync(animeMalId);

			// Then
			result.Should().HaveCount(5);
		}
	}
}
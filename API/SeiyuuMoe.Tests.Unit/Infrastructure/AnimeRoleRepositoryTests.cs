using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Infrastructure.Database.Animes;
using SeiyuuMoe.Tests.Common.Builders.Model;
using SeiyuuMoe.Tests.Common.Helpers;
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
			await repository.AddAsync(new AnimeRoleBuilder().Build());

			// Then
			var allRoles = await dbContext.AnimeRoles.ToListAsync();

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
			var expectedLanguage = LanguageId.Japanese;

			var role = new AnimeRoleBuilder()
				.WithAnime(x => x.WithTitle(expectedAnimeTitle))
				.WithSeiyuu(x => x.WithName(expectedSeiyuuName))
				.WithCharacter(x => x.WithName(expectedcharacterName))
				.WithRoleType(x => x.WithDescription(expectedRoleTypeName))
				.WithLanguage(x => x.WithLanguageId(expectedLanguage))
				.Build();

			// When
			await repository.AddAsync(role);

			// Then
			var allroles = await dbContext.AnimeRoles.ToListAsync();
			var newRole = await dbContext.AnimeRoles
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
				newRole.Language.Id.Should().Be(expectedLanguage);
				newRole.RoleType.Should().NotBeNull();
				newRole.RoleType.Description.Should().Be(expectedRoleTypeName);
			}
		}

		[Fact]
		public async Task AddAsync_GivenExistingRoleWithSameValues_ShouldAdd()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRoleRepository(dbContext);

			var role1 = new AnimeRoleBuilder()
				.Build();

			var role2 = new AnimeRoleBuilder()
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
			var animeId = Guid.NewGuid();
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRoleRepository(dbContext);

			// When
			var result = await repository.GetAllRolesInAnimeAsync(animeId);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllRolesInAnimeAsync_GivenNoRolesForAnime_ShouldReturnEmpty()
		{
			// Given
			var animeId = Guid.NewGuid();
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRoleRepository(dbContext);

			var anime = new AnimeBuilder().WithMalId(2).WithId(animeId).Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRolesInAnimeAsync(animeId);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllRolesInAnimeAsync_GivenAnimeWithSingleRole_ShouldReturnSingle()
		{
			// Given
			const int animeMalId = 1;
			var animeId = Guid.NewGuid();
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRoleRepository(dbContext);

			var role = new AnimeRoleBuilder()
				.WithAnime(
					x => x.WithMalId(animeMalId).WithId(animeId)
				)
				.WithLanguage(x => x.WithLanguageId(LanguageId.Japanese))
				.Build();

			await dbContext.AddAsync(role);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRolesInAnimeAsync(animeId);

			// Then
			result.Should().ContainSingle();
		}

		[Fact]
		public async Task GetAllRolesInAnimeAsync_GivenAnimeWithSingleRoleInLanguageOtherThanJapanese_ShouldReturnEmpty()
		{
			// Given
			const int animeMalId = 1;
			var animeId = Guid.NewGuid();
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRoleRepository(dbContext);

			var role = new AnimeRoleBuilder()
				.WithAnime(
					x => x.WithMalId(animeMalId).WithId(animeId)
				)
				.WithLanguage(x => x.WithLanguageId(LanguageId.Korean))
				.Build();

			await dbContext.AddAsync(role);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRolesInAnimeAsync(animeId);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllRolesInAnimeAsync_GivenMultipleRoles_ShouldReturnMultiple()
		{
			// Given
			const int animeMalId = 1;
			var animeId = Guid.NewGuid();
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRoleRepository(dbContext);

			var japanese = new LanguageBuilder().WithLanguageId(LanguageId.Japanese).Build();
			var anime = new AnimeBuilder().WithMalId(animeMalId).WithId(animeId).Build();
			anime.Role = new List<AnimeRole>
			{
				new AnimeRoleBuilder().WithLanguage(japanese).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).Build(),
			};

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRolesInAnimeAsync(animeId);

			// Then
			result.Should().HaveCount(5);
		}
	}
}
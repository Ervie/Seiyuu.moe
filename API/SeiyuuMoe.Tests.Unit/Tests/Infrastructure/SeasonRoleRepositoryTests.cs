using FluentAssertions;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Infrastructure.Seasons;
using SeiyuuMoe.Tests.Unit.Builders.Model;
using SeiyuuMoe.Tests.Unit.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tests.Infrastructure
{
	public class SeasonRoleRepositoryTests
	{
		[Fact]
		public async Task GetAllRolesInSeason_GivenNoRoles_ShouldReturnEmpty()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRoleRepository(dbContext);

			// When
			var result = await repository.GetAllRolesInSeason(new List<long> { 1 }, false);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllRolesInSeason_GivenNoRolesInSeason_ShouldReturnEmpty()
		{
			// Given
			const int animeMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRoleRepository(dbContext);

			var anime = new RoleBuilder()
				.WithAnime(x => x.WithMalId(2))
				.WithLanguage(x => x.WithName("Japanese").WithId(1))
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRolesInSeason(new List<long> { animeMalId }, false);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllRolesInSeason_GivenSeasonWithSingleAnimeWithSingleRole_ShouldReturnSingle()
		{
			// Given
			const int animeMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRoleRepository(dbContext);

			var role = new RoleBuilder()
				.WithAnime(x => x.WithMalId(animeMalId))
				.WithLanguage(x => x.WithName("Japanese").WithId(1))
				.Build();

			await dbContext.AddAsync(role);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRolesInSeason(new List<long> { animeMalId }, false);

			// Then
			result.Should().ContainSingle();
		}

		[Fact]
		public async Task GetAllRolesInSeason_GivenSeasonWithSingleAnimeWithSingleRoleInLanguageOtherThanJapanese_ShouldReturnEmpty()
		{
			// Given
			const int animeMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRoleRepository(dbContext);

			var role = new RoleBuilder()
				.WithAnime(x => x.WithMalId(animeMalId))
				.WithLanguage(x => x.WithName("Test").WithId(9999))
				.Build();

			await dbContext.AddAsync(role);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRolesInSeason(new List<long> { animeMalId }, false);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllRolesInSeason_GivenSeasonWithSingleAnimeWithMultipleRoles_ShouldReturnMultiple()
		{
			// Given
			const int animeMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRoleRepository(dbContext);

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
			var result = await repository.GetAllRolesInSeason(new List<long> { animeMalId }, false);

			// Then
			result.Should().HaveCount(5);
		}

		[Fact]
		public async Task GetAllRolesInSeason_GivenSeasonWithSingleAnimeWithMultipleRolesAndMainRolesOnly_ShouldReturnPartial()
		{
			// Given
			const int animeMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRoleRepository(dbContext);

			var japanese = new LanguageBuilder().WithId(1).WithName("Japanese").Build();
			var anime = new AnimeBuilder().WithMalId(animeMalId).Build();
			var mainRole = new RoleTypeBuilder().WithName("Main").WithId(1).Build();
			var supportingRole = new RoleTypeBuilder().WithName("Supporting").WithId(2).Build();

			anime.Role = new List<Role>
			{
				new RoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build()
			};

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRolesInSeason(new List<long> { animeMalId }, true);

			// Then
			result.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetAllRolesInSeason_GivenSeasonWithSingleAnimeWithMultipleRolesInDifferentLanguagesAndMainRolesOnly_ShouldReturnPartial()
		{
			// Given
			const int animeMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRoleRepository(dbContext);

			var japanese = new LanguageBuilder().WithId(1).WithName("Japanese").Build();
			var korean = new LanguageBuilder().WithId(2).WithName("Korean").Build();
			var anime = new AnimeBuilder().WithMalId(animeMalId).Build();
			var mainRole = new RoleTypeBuilder().WithName("Main").WithId(1).Build();
			var supportingRole = new RoleTypeBuilder().WithName("Supporting").WithId(2).Build();

			anime.Role = new List<Role>
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

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRolesInSeason(new List<long> { animeMalId }, true);

			// Then
			result.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetAllRolesInSeason_GivenSeasonWithMultipleAnime_ShouldReturnPartial()
		{
			// Given
			const int animeMalId1 = 1;
			const int animeMalId2 = 2;
			const int animeMalId3 = 3;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRoleRepository(dbContext);

			var japanese = new LanguageBuilder().WithId(1).WithName("Japanese").Build();
			var mainRole = new RoleTypeBuilder().WithName("Main").WithId(1).Build();
			var supportingRole = new RoleTypeBuilder().WithName("Supporting").WithId(2).Build();

			var anime1 = new AnimeBuilder().WithMalId(animeMalId1).Build();

			anime1.Role = new List<Role>
			{
				new RoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build(),
			};

			var anime2 = new AnimeBuilder().WithMalId(animeMalId2).Build();

			anime2.Role = new List<Role>
			{
				new RoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build(),
			};

			var anime3 = new AnimeBuilder().WithMalId(animeMalId3).Build();

			anime3.Role = new List<Role>
			{
				new RoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new RoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build(),
			};

			await dbContext.AddAsync(anime1);
			await dbContext.AddAsync(anime2);
			await dbContext.AddAsync(anime3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRolesInSeason(new List<long> { animeMalId1, animeMalId2 }, true);

			// Then
			result.Should().HaveCount(4);
		}
	}
}
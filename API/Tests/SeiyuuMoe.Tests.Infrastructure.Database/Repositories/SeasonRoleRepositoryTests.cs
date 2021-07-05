using FluentAssertions;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Infrastructure.Database.Seasons;
using SeiyuuMoe.Tests.Common.Builders.Model;
using SeiyuuMoe.Tests.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Infrastructure.Database
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
			var result = await repository.GetAllRolesInSeason(new List<Guid> { Guid.NewGuid() }, false);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllRolesInSeason_GivenNoRolesInSeason_ShouldReturnEmpty()
		{
			// Given
			var animeId = Guid.NewGuid();
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRoleRepository(dbContext);

			var anime = new AnimeRoleBuilder()
				.WithAnime(x => x.WithId(Guid.NewGuid()))
				.WithLanguage(x => x.WithLanguageId(LanguageId.Japanese))
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRolesInSeason(new List<Guid> { animeId }, false);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllRolesInSeason_GivenSeasonWithSingleAnimeWithSingleRole_ShouldReturnSingle()
		{
			// Given
			var animeId = Guid.NewGuid();
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRoleRepository(dbContext);

			var role = new AnimeRoleBuilder()
				.WithAnime(x => x.WithId(animeId))
				.WithLanguage(x => x.WithLanguageId(LanguageId.Japanese))
				.WithRoleType(x => x.WithId(AnimeRoleTypeId.Main))
				.Build();

			await dbContext.AddAsync(role);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRolesInSeason(new List<Guid> { animeId }, false);

			// Then
			result.Should().ContainSingle();
		}

		[Fact]
		public async Task GetAllRolesInSeason_GivenSeasonWithSingleAnimeWithSingleRoleInLanguageOtherThanJapanese_ShouldReturnEmpty()
		{
			// Given
			var animeId = Guid.NewGuid();
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRoleRepository(dbContext);

			var role = new AnimeRoleBuilder()
				.WithAnime(x => x.WithId(animeId))
				.WithLanguage(x => x.WithLanguageId(LanguageId.Korean))
				.Build();

			await dbContext.AddAsync(role);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRolesInSeason(new List<Guid> { animeId }, false);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllRolesInSeason_GivenSeasonWithSingleAnimeWithMultipleRoles_ShouldReturnMultiple()
		{
			// Given
			var animeId = Guid.NewGuid();
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRoleRepository(dbContext);

			var japanese = new LanguageBuilder().WithLanguageId(LanguageId.Japanese).Build();
			var anime = new AnimeBuilder().WithId(animeId).Build();
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
			var result = await repository.GetAllRolesInSeason(new List<Guid> { animeId }, false);

			// Then
			result.Should().HaveCount(5);
		}

		[Fact]
		public async Task GetAllRolesInSeason_GivenSeasonWithSingleAnimeWithMultipleRolesAndMainRolesOnly_ShouldReturnPartial()
		{
			// Given
			var animeId = Guid.NewGuid();
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRoleRepository(dbContext);

			var japanese = new LanguageBuilder().WithLanguageId(LanguageId.Japanese).Build();
			var anime = new AnimeBuilder().WithId(animeId).Build();
			var mainRole = new AnimeRoleTypeBuilder().WithId(AnimeRoleTypeId.Main).Build();
			var supportingRole = new AnimeRoleTypeBuilder().WithId(AnimeRoleTypeId.Supporting).Build();

			anime.Role = new List<AnimeRole>
			{
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build()
			};

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRolesInSeason(new List<Guid> { animeId }, true);

			// Then
			result.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetAllRolesInSeason_GivenSeasonWithSingleAnimeWithMultipleRolesInDifferentLanguagesAndMainRolesOnly_ShouldReturnPartial()
		{
			// Given
			var animeId = Guid.NewGuid();
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRoleRepository(dbContext);

			var japanese = new LanguageBuilder().WithLanguageId(LanguageId.Japanese).Build();
			var korean = new LanguageBuilder().WithLanguageId(LanguageId.Korean).Build();
			var anime = new AnimeBuilder().WithId(animeId).Build();
			var mainRole = new AnimeRoleTypeBuilder().WithId(AnimeRoleTypeId.Main).Build();
			var supportingRole = new AnimeRoleTypeBuilder().WithId(AnimeRoleTypeId.Supporting).Build();

			anime.Role = new List<AnimeRole>
			{
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build(),
				new AnimeRoleBuilder().WithLanguage(korean).WithRoleType(mainRole).Build(),
				new AnimeRoleBuilder().WithLanguage(korean).WithRoleType(supportingRole).Build(),
				new AnimeRoleBuilder().WithLanguage(korean).WithRoleType(supportingRole).Build(),
				new AnimeRoleBuilder().WithLanguage(korean).WithRoleType(supportingRole).Build(),
				new AnimeRoleBuilder().WithLanguage(korean).WithRoleType(mainRole).Build()
			};

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRolesInSeason(new List<Guid> { animeId }, true);

			// Then
			result.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetAllRolesInSeason_GivenSeasonWithMultipleAnime_ShouldReturnPartial()
		{
			// Given
			var animeId1 = Guid.NewGuid();
			var animeId2 = Guid.NewGuid();
			var animeId3 = Guid.NewGuid();
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeasonRoleRepository(dbContext);

			var japanese = new LanguageBuilder().WithLanguageId(LanguageId.Japanese).Build();
			var mainRole = new AnimeRoleTypeBuilder().WithId(AnimeRoleTypeId.Main).Build();
			var supportingRole = new AnimeRoleTypeBuilder().WithId(AnimeRoleTypeId.Supporting).Build();

			var anime1 = new AnimeBuilder().WithId(animeId1).Build();

			anime1.Role = new List<AnimeRole>
			{
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build(),
			};

			var anime2 = new AnimeBuilder().WithId(animeId2).Build();

			anime2.Role = new List<AnimeRole>
			{
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build(),
			};

			var anime3 = new AnimeBuilder().WithId(animeId3).Build();

			anime3.Role = new List<AnimeRole>
			{
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(supportingRole).Build(),
				new AnimeRoleBuilder().WithLanguage(japanese).WithRoleType(mainRole).Build(),
			};

			await dbContext.AddAsync(anime1);
			await dbContext.AddAsync(anime2);
			await dbContext.AddAsync(anime3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRolesInSeason(new List<Guid> { animeId1, animeId2 }, true);

			// Then
			result.Should().HaveCount(4);
		}
	}
}
using FluentAssertions;
using SeiyuuMoe.Application.AnimeComparisons.CompareAnime;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Infrastructure.Animes;
using SeiyuuMoe.Infrastructure.Context;
using SeiyuuMoe.Tests.Common.Builders.Model;
using SeiyuuMoe.Tests.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Component.AnimeComparisons
{
	public class CompareAnimeQueryHandlerTests
	{
		[Fact]
		public async Task HandleAsync_GivenEmptyDatabase_ShouldReturnEmptyResults()
		{
			// Given
			var query = new CompareAnimeQuery { AnimeMalIds = new List<long> { 1, 2 } };
			var dbContext = InMemoryDbProvider.GetDbContext();

			var handler = CreateHandler(dbContext);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task HandleAsync_GivenSingleAnimeIdQueryDatabase_ShouldReturnEmptyResults()
		{
			// Given
			var query = new CompareAnimeQuery { AnimeMalIds = new List<long> { 1 } };
			var dbContext = InMemoryDbProvider.GetDbContext();

			var onePieceAnime = new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(3).WithTitle("One Piece").Build();
			var animeRoles = new List<AnimeRole>
			{
				new AnimeRoleBuilder()
					.WithAnime(onePieceAnime)
					.WithSeiyuu(x => x.WithId(Guid.NewGuid()).WithMalId(1).WithName("Miyuki Sawashiro").Build())
					.WithCharacter(x => x.WithId(Guid.NewGuid()).WithMalId(1).WithName("Charlotte Pudding").Build())
					.Build(),
				new AnimeRoleBuilder()
					.WithAnime(onePieceAnime)
					.WithSeiyuu(x => x.WithId(Guid.NewGuid()).WithMalId(2).WithName("Chiwa Saito").Build())
					.WithCharacter(x => x.WithId(Guid.NewGuid()).WithMalId(1).WithName("Chimney").Build())
					.Build(),
			};

			await dbContext.AddRangeAsync(animeRoles);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task HandleAsync_GivenNoCommonSeiyuu_ShouldReturnEmptyResults()
		{
			// Given
			var query = new CompareAnimeQuery { AnimeMalIds = new List<long> { 1, 2 } };
			var dbContext = InMemoryDbProvider.GetDbContext();

			var animeRoles = new List<AnimeRole>
			{
				new AnimeRoleBuilder()
					.WithAnime(new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(1).WithTitle("One Piece").Build())
					.WithSeiyuu(x => x.WithId(Guid.NewGuid()).WithMalId(1).WithName("Miyuki Sawashiro").Build())
					.WithCharacter(x => x.WithId(Guid.NewGuid()).WithMalId(1).WithName("Charlotte Pudding").Build())
					.Build(),
				new AnimeRoleBuilder()
					.WithAnime(new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(2).WithTitle("Bakemonogatari").Build())
					.WithSeiyuu(x => x.WithId(Guid.NewGuid()).WithMalId(2).WithName("Chiwa Saito").Build())
					.WithCharacter(x => x.WithId(Guid.NewGuid()).WithMalId(1).WithName("Hitagi Senjougahara").Build())
					.Build()
			};

			await dbContext.AddRangeAsync(animeRoles);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task HandleAsync_GivenSingleCommonSeiyuu_ShouldReturnSingleResult()
		{
			// Given
			var query = new CompareAnimeQuery { AnimeMalIds = new List<long> { 1, 2 } };
			var dbContext = InMemoryDbProvider.GetDbContext();

			var miyukiSawashiro = new SeiyuuBuilder().WithId(Guid.NewGuid()).WithMalId(1).WithName("Miyuki Sawashiro").Build();
			var japanese = new LanguageBuilder().WithLanguageId(LanguageId.Japanese).Build();
			var animeRoles = new List<AnimeRole>
			{
				new AnimeRoleBuilder()
					.WithAnime(new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(1).WithTitle("One Piece").Build())
					.WithSeiyuu(miyukiSawashiro)
					.WithCharacter(x => x.WithId(Guid.NewGuid()).WithMalId(1).WithName("Charlotte Pudding").Build())
					.WithLanguage(japanese)
					.Build(),
				new AnimeRoleBuilder()
					.WithAnime(new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(2).WithTitle("Bakemonogatari").Build())
					.WithSeiyuu(miyukiSawashiro)
					.WithCharacter(x => x.WithId(Guid.NewGuid()).WithMalId(2).WithName("Kanbaru Suruga").Build())
					.WithLanguage(japanese)
					.Build()
			};

			await dbContext.AddRangeAsync(animeRoles);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			result.Should().ContainSingle();
		}

		[Fact]
		public async Task HandleAsync_GivenMultipleCommonSeiyuu_ShouldReturnMultipleResults()
		{
			// Given
			var query = new CompareAnimeQuery { AnimeMalIds = new List<long> { 1, 2 } };
			var dbContext = InMemoryDbProvider.GetDbContext();

			var miyukiSawashiro = new SeiyuuBuilder().WithId(Guid.NewGuid()).WithMalId(1).WithName("Miyuki Sawashiro").Build();
			var chiwaSaito = new SeiyuuBuilder().WithId(Guid.NewGuid()).WithMalId(2).WithName("Chiwa Saito").Build();
			var onePieceAnime = new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(1).WithTitle("One Piece").Build();
			var bakemonogatariAnime = new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(2).WithTitle("Bakemonogatari").Build();
			var japanese = new LanguageBuilder().WithLanguageId(LanguageId.Japanese).Build();
			var animeRoles = new List<AnimeRole>
			{
				new AnimeRoleBuilder()
					.WithAnime(onePieceAnime)
					.WithSeiyuu(miyukiSawashiro)
					.WithCharacter(x => x.WithId(Guid.NewGuid()).WithMalId(1).WithName("Charlotte Pudding").Build())
					.WithLanguage(japanese)
					.Build(),
				new AnimeRoleBuilder()
					.WithAnime(bakemonogatariAnime)
					.WithSeiyuu(miyukiSawashiro)
					.WithCharacter(x => x.WithId(Guid.NewGuid()).WithMalId(2).WithName("Kanbaru Suruga").Build())
					.WithLanguage(japanese)
					.Build(),
				new AnimeRoleBuilder()
					.WithAnime(onePieceAnime)
					.WithSeiyuu(chiwaSaito)
					.WithCharacter(x => x.WithId(Guid.NewGuid()).WithMalId(1).WithName("Chimney").Build())
					.WithLanguage(japanese)
					.Build(),
				new AnimeRoleBuilder()
					.WithAnime(bakemonogatariAnime)
					.WithSeiyuu(chiwaSaito)
					.WithCharacter(x => x.WithId(Guid.NewGuid()).WithMalId(2).WithName("Hitagi Senjougahara").Build())
					.WithLanguage(japanese)
					.Build()
			};

			await dbContext.AddRangeAsync(animeRoles);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			result.Should().HaveCount(2);
		}

		private CompareAnimeQueryHandler CreateHandler(SeiyuuMoeContext seiyuuMoeContext)
		{
			var animeRepository = new AnimeRoleRepository(seiyuuMoeContext);

			return new CompareAnimeQueryHandler(animeRepository);
		}
	}
}
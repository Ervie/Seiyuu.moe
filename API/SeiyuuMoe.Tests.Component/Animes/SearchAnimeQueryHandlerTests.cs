using FluentAssertions;
using FluentAssertions.Execution;
using SeiyuuMoe.Application.Animes;
using SeiyuuMoe.Application.Animes.SearchAnime;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Infrastructure.Database.Animes;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Tests.Common.Builders.Model;
using SeiyuuMoe.Tests.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Component.Animes
{
	public class SearchAnimeQueryHandlerTests
	{
		[Fact]
		public async Task HandleAsync_GivenEmptyDatabase_ShouldReturnEmptyResults()
		{
			// Given
			var query = new SearchAnimeQuery();
			var dbContext = InMemoryDbProvider.GetDbContext();
			var handler = CreateHandler(dbContext);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			result.Results.Should().BeEmpty();
		}

		[Fact]
		public async Task HandleAsync_GivenNoMatchingAnime_ShouldReturnEmptyResults()
		{
			// Given
			var query = new SearchAnimeQuery { Title = "Gundam" };
			var dbContext = InMemoryDbProvider.GetDbContext();

			var animes = new List<Anime>
			{
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(1).WithTitle("Naruto").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(2).WithTitle("Bleach").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(3).WithTitle("One Piece").Build(),
			};
			await dbContext.AddRangeAsync(animes);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			result.Results.Should().BeEmpty();
		}

		[Fact]
		public async Task HandleAsync_GivenSingleMatchingTitleAnime_ShouldReturnSingleResult()
		{
			// Given
			var query = new SearchAnimeQuery { Title = "Gundam" };
			var dbContext = InMemoryDbProvider.GetDbContext();

			var animes = new List<Anime>
			{
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(1).WithTitle("Naruto").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(2).WithTitle("Zeta Gundam").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(3).WithTitle("One Piece").Build(),
			};
			await dbContext.AddRangeAsync(animes);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			result.Results.Should().ContainSingle();
		}

		[Fact]
		public async Task HandleAsync_GivenMultipleMatchingTitleAnime_ShouldReturnMultipleResult()
		{
			// Given
			var query = new SearchAnimeQuery { Title = "Gundam" };
			var dbContext = InMemoryDbProvider.GetDbContext();

			var animes = new List<Anime>
			{
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(1).WithTitle("Gundam ZZ").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(2).WithTitle("Zeta Gundam").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(3).WithTitle("Turn A Gundam").Build(),
			};
			await dbContext.AddRangeAsync(animes);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			result.Results.Should().HaveSameCount(animes);
		}

		[Fact]
		public async Task HandleAsync_GivenOver10MatchingTitleAnime_ShouldReturnOnly10Results()
		{
			// Given
			const int defaultResultLimit = 10;
			var query = new SearchAnimeQuery { Title = "Gundam" };
			var dbContext = InMemoryDbProvider.GetDbContext();

			var animes = new List<Anime>
			{
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(1).WithTitle("Gundam ZZ").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(2).WithTitle("Zeta Gundam").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(3).WithTitle("Turn A Gundam").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(4).WithTitle("Gundam Seed").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(5).WithTitle("Gundam Seed Destiny").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(6).WithTitle("Gundam 00").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(7).WithTitle("Gundam G no Reconguista").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(8).WithTitle("Gundam Wind").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(9).WithTitle("Mobile Fighter G Gundam").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(10).WithTitle("Victory Gundam").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(11).WithTitle("Gundam 0079").Build(),
			};
			await dbContext.AddRangeAsync(animes);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			result.Results.Should().HaveCount(defaultResultLimit);
		}

		[Fact]
		public async Task HandleAsync_GivenOver10MatchingTitleAnime_ShouldReturnTenMostPopular()
		{
			// Given
			var query = new SearchAnimeQuery { Title = "Gundam" };
			var dbContext = InMemoryDbProvider.GetDbContext();

			var animes = new List<Anime>
			{
				new AnimeBuilder().WithId(Guid.NewGuid()).WithPopularity(1).WithMalId(1).WithTitle("Gundam ZZ").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithPopularity(2).WithMalId(2).WithTitle("Zeta Gundam").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithPopularity(3).WithMalId(3).WithTitle("Turn A Gundam").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithPopularity(4).WithMalId(4).WithTitle("Gundam Seed").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithPopularity(5).WithMalId(5).WithTitle("Gundam Seed Destiny").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithPopularity(6).WithMalId(6).WithTitle("Gundam 00").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithPopularity(7).WithMalId(7).WithTitle("Gundam G no Reconguista").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithPopularity(8).WithMalId(8).WithTitle("Gundam Wind").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithPopularity(9).WithMalId(9).WithTitle("Mobile Fighter G Gundam").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithPopularity(10).WithMalId(10).WithTitle("Victory Gundam").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithPopularity(11).WithMalId(11).WithTitle("Gundam 0079").Build(),
			};
			await dbContext.AddRangeAsync(animes);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			result.Results.Should().NotContain(x => x.MalId == 1);
		}

		[Fact]
		public async Task HandleAsync_GivenMatchingSynonymsAnime_ShouldReturnMultipleResult()
		{
			// Given
			var query = new SearchAnimeQuery { Title = "Ghost" };
			var dbContext = InMemoryDbProvider.GetDbContext();

			var animes = new List<Anime>
			{
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(1).WithTitle("Naruto").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(2).WithTitle("Bakemonogatari").WithTitleSynonyms("Ghostory").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(3).WithTitle("One Piece").Build(),
			};
			await dbContext.AddRangeAsync(animes);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			result.Results.Should().ContainSingle().Which.MalId.Should().Be(2);
		}

		[Fact]
		public async Task HandleAsync_GivenMatchingEnglishAnime_ShouldReturnMultipleResult()
		{
			// Given
			var query = new SearchAnimeQuery { Title = "they cry" };
			var dbContext = InMemoryDbProvider.GetDbContext();

			var animes = new List<Anime>
			{
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(1).WithTitle("Naruto").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(2).WithTitle("Higurashi no Naku Koro ni").WithJapaneseTitle("When They Cry").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(3).WithTitle("One Piece").Build(),
			};
			await dbContext.AddRangeAsync(animes);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			result.Results.Should().ContainSingle().Which.MalId.Should().Be(2);
		}

		[Fact]
		public async Task HandleAsync_GivenMatchingJapaneseAnime_ShouldReturnMultipleResult()
		{
			// Given
			var query = new SearchAnimeQuery { Title = "ひぐらしのな" };
			var dbContext = InMemoryDbProvider.GetDbContext();

			var animes = new List<Anime>
			{
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(1).WithTitle("Naruto").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(2).WithTitle("Higurashi no Naku Koro ni")
					.WithJapaneseTitle("When They Cry").WithJapaneseTitle("ひぐらしのなく頃に").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(3).WithTitle("One Piece").Build(),
			};
			await dbContext.AddRangeAsync(animes);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			result.Results.Should().ContainSingle().Which.MalId.Should().Be(2);
		}

		[Fact]
		public async Task HandleAsync_GivenSingleMatchingTitleAnime_ShouldReturnMappedEntity()
		{
			// Given
			const int expectedMalId = 2;
			const string expectedTitle = "Zeta Gundam";
			const string expectedImageUrl = "https://some.url";

			var query = new SearchAnimeQuery { Title = "Gundam" };
			var dbContext = InMemoryDbProvider.GetDbContext();

			var animes = new List<Anime>
			{
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(1).WithTitle("Naruto").Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(expectedMalId).WithTitle(expectedTitle).WithImageUrl(expectedImageUrl).Build(),
				new AnimeBuilder().WithId(Guid.NewGuid()).WithMalId(3).WithTitle("One Piece").Build(),
			};
			await dbContext.AddRangeAsync(animes);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			using (new AssertionScope())
			{
				result.Results.First().MalId.Should().Be(expectedMalId);
				result.Results.First().Title.Should().Be(expectedTitle);
				result.Results.First().ImageUrl.Should().Be(expectedImageUrl);
			}
		}

		private SearchAnimeQueryHandler CreateHandler(SeiyuuMoeContext seiyuuMoeContext)
		{
			var animeRepository = new AnimeRepository(seiyuuMoeContext);
			var animeSearchCriteriaService = new AnimeSearchCriteriaService();

			return new SearchAnimeQueryHandler(animeRepository, animeSearchCriteriaService);
		}
	}
}
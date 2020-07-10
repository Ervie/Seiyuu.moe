using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Infrastructure.Animes;
using SeiyuuMoe.Tests.Unit.Builders.Model;
using SeiyuuMoe.Tests.Unit.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tests.Infrastructure
{
	public class AnimeRepositoryTests
	{
		[Fact]
		public async Task AddAsync_GivenEmptyAnime_ShouldAddAnime()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);

			// When
			await repository.AddAsync(new AnimeBuilder().Build());

			// Then
			var allAnime = await dbContext.Anime.ToListAsync();

			allAnime.Should().ContainSingle();
		}

		[Fact]
		public async Task AddAsync_GivenAnime_ShouldAddAnime()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);

			const string expectedTitle = "ExpectedTitle";
			const string expectedImageUrl = "ExpectedImageUrl";
			const string expectedJapaneseTitle = "期待される日本語タイトル";
			const string expectedTitleSynonyms = "expectedTitleSynonyms";
			const string expectedAbout = "ExpectedAbout";
			const string expectedType = "ExpectedType";
			const string expectedStatus = "ExpectedStatus";
			const long expectedMalId = 1;

			var anime = new AnimeBuilder()
				.WithTitle(expectedTitle)
				.WithImageUrl(expectedImageUrl)
				.WithMalId(expectedMalId)
				.WithJapaneseTitle(expectedJapaneseTitle)
				.WithTitleSynonyms(expectedTitleSynonyms)
				.WithAbout(expectedAbout)
				.WithAnimeType(at => at.WithName(expectedType))
				.WithAnimeStatus(at => at.WithName(expectedStatus))
				.Build();

			// When
			await repository.AddAsync(anime);

			// Then
			var allAnime = await dbContext.Anime.ToListAsync();
			var newAnime = await dbContext.Anime.FirstOrDefaultAsync();

			using (new AssertionScope())
			{
				allAnime.Should().ContainSingle();

				newAnime.Should().NotBeNull();
				newAnime.Title.Should().Be(expectedTitle);
				newAnime.ImageUrl.Should().Be(expectedImageUrl);
				newAnime.MalId.Should().Be(expectedMalId);
				newAnime.JapaneseTitle.Should().Be(expectedJapaneseTitle);
				newAnime.TitleSynonyms.Should().Be(expectedTitleSynonyms);
				newAnime.About.Should().Be(expectedAbout);
				newAnime.Type.Name.Should().Be(expectedType);
				newAnime.Status.Name.Should().Be(expectedStatus);
			}
		}

		[Fact]
		public async Task AddAsync_GivenDuplicatedKeyAnime_ShouldThrowException()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);

			await repository.AddAsync(new AnimeBuilder().WithMalId(1).Build());

			// When
			Func<Task> func = repository.Awaiting(x => x.AddAsync(new AnimeBuilder().WithMalId(1).Build()));

			// Then
			func.Should().Throw<Exception>();
		}

		[Fact]
		public void UpdateAsync_GivenNotExistingAnime_ShouldThrowExcepton()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);

			// When
			Func<Task> func = repository.Awaiting(x => x.UpdateAsync(new AnimeBuilder().WithMalId(1).Build()));

			// Then
			func.Should().Throw<Exception>();
		}

		[Fact]
		public async Task UpdateAsync_GivenExistingAnime_ShouldUpdate()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);
			var anime = new AnimeBuilder().WithTitle("Test").Build();

			await dbContext.Anime.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			anime.Title = "Updated";

			// When
			await repository.UpdateAsync(anime);

			// Then
			var allAnimes = await dbContext.Anime.ToListAsync();

			allAnimes.Should().ContainSingle().Which.Title.Should().Be("Updated");
		}

		[Fact]
		public async Task GetAsync_GivenNoAnime_ShouldReturnNull()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);

			// When
			var result = await repository.GetAsync(1);

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetAsync_GivenAnimeWithOtherId_ShouldReturnNull()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);
			var anime1 = new AnimeBuilder().WithMalId(1).Build();

			await dbContext.Anime.AddAsync(anime1);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAsync(2);

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetAsync_GivenExistingAnimeWithId_ShouldReturnResult()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);
			var anime1 = new AnimeBuilder().WithMalId(1).Build();

			await dbContext.Anime.AddAsync(anime1);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAsync(1);

			// Then
			result.Should().NotBeNull();
		}

		[Fact]
		public async Task GetAsync_GivenMultipleExistingAnimeOneOfWhichWithId_ShouldReturnResult()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);
			var anime1 = new AnimeBuilder().WithMalId(1).Build();
			var anime2 = new AnimeBuilder().WithMalId(2).Build();
			var anime3 = new AnimeBuilder().WithMalId(3).Build();
			var anime4 = new AnimeBuilder().WithMalId(4).Build();
			var anime5 = new AnimeBuilder().WithMalId(5).Build();

			await dbContext.Anime.AddAsync(anime1);
			await dbContext.Anime.AddAsync(anime2);
			await dbContext.Anime.AddAsync(anime3);
			await dbContext.Anime.AddAsync(anime4);
			await dbContext.Anime.AddAsync(anime5);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAsync(3);

			// Then
			result.Should().NotBeNull();
			result.MalId.Should().Be(3);
		}

		[Fact]
		public async Task GetAsync_GivenExistingAnimeWithId_ShouldReturnResultWithEntities()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);
			var anime = new AnimeBuilder()
				 .WithTitle("Test")
				 .WithAnimeStatus(x => x.WithName("Airing"))
				 .WithAnimeType(x => x.WithName("TV"))
				 .WithTitle("ExpectedTitle")
				 .WithImageUrl("ExpectedImageUrl")
				 .WithAbout("ExpectedAbout")
				 .WithMalId(1)
				 .Build();

			await dbContext.Anime.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAsync(1);

			// Then
			using (new AssertionScope())
			{
				result.Should().NotBeNull();
				result.MalId.Should().Be(1);
				result.Title.Should().Be("ExpectedTitle");
				result.ImageUrl.Should().Be("ExpectedImageUrl");
				result.About.Should().Be("ExpectedAbout");
				result.Type.Should().NotBeNull();
				result.Type.Name.Should().Be("TV");
				result.Status.Should().NotBeNull();
				result.Status.Name.Should().Be("Airing");
			}
		}

		[Fact]
		public async Task GetAllAsync_GivenNoAnime_ShouldReturnEmpty()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);

			// When
			var result = await repository.GetAllAsync();

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllAsync_GivenSingleAnime_ShouldReturnSingle()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);
			var anime = new AnimeBuilder().WithTitle("Test").Build();

			await dbContext.Anime.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllAsync();

			// Then
			result.Should().ContainSingle().Which.Title.Should().Be("Test");
		}

		[Fact]
		public async Task GetAllAsync_GivenSingleAnimeWithSubEntities_ShouldReturnSingleAnimeWithSubentities()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);
			var anime = new AnimeBuilder()
				.WithTitle("Test")
				.WithAnimeStatus(x => x.WithName("Airing"))
				.WithAnimeType(x => x.WithName("TV"))
				.WithSeason(x => x.WithName("Spring").WithYear(2020).Build())
				.Build();

			await dbContext.Anime.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllAsync();

			// Then
			result.Should().ContainSingle();

			var addedAnime = result.FirstOrDefault();

			using (new AssertionScope())
			{
				addedAnime.Should().NotBeNull();
				addedAnime.Type.Should().NotBeNull();
				addedAnime.Type.Name.Should().Be("TV");
				addedAnime.Status.Should().NotBeNull();
				addedAnime.Status.Name.Should().Be("Airing");
				addedAnime.Season.Should().NotBeNull();
				addedAnime.Season.Name.Should().Be("Spring");
				addedAnime.Season.Year.Should().Be(2020);
			}
		}

		[Fact]
		public async Task GetAllAsync_GivenMultipleAnime_ShouldReturnMultiple()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);
			var anime1 = new AnimeBuilder().WithTitle("Test1").Build();
			var anime2 = new AnimeBuilder().WithTitle("Test2").Build();
			var anime3 = new AnimeBuilder().WithTitle("Test3").Build();

			await dbContext.Anime.AddAsync(anime1);
			await dbContext.Anime.AddAsync(anime2);
			await dbContext.Anime.AddAsync(anime3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllAsync();

			// Then
			result.Should().HaveCount(3);
		}

		[Fact]
		public async Task GetAllAsync_GivenMultipleAnimeWithEmptyPredicate_ShouldReturnAll()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);
			var anime1 = new AnimeBuilder().WithTitle("Test1").Build();
			var anime2 = new AnimeBuilder().WithTitle("Test2").Build();
			var anime3 = new AnimeBuilder().WithTitle("Test3").Build();

			await dbContext.Anime.AddAsync(anime1);
			await dbContext.Anime.AddAsync(anime2);
			await dbContext.Anime.AddAsync(anime3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllAsync(x => true);

			// Then
			result.Should().HaveCount(3);
		}

		[Fact]
		public async Task GetAllAsync_GivenMultipleAnimeWithPredicate_ShouldReturnPartial()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);
			var anime1 = new AnimeBuilder().WithTitle("Test1").Build();
			var anime2 = new AnimeBuilder().WithTitle("Test2").Build();
			var anime3 = new AnimeBuilder().WithTitle("Test3").Build();

			await dbContext.Anime.AddAsync(anime1);
			await dbContext.Anime.AddAsync(anime2);
			await dbContext.Anime.AddAsync(anime3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllAsync(x => x.Title.EndsWith("3"));

			// Then
			result.Should().ContainSingle();
		}

		[Fact]
		public async Task CountAsync_GivenNoAnime_ShouldReturnZero()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);

			// When
			var result = await repository.GetAnimeCountAsync();

			// Then
			result.Should().Be(0);
		}

		[Fact]
		public async Task CountAsync_GivenSingleAnime_ShouldReturnOne()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);
			var anime1 = new AnimeBuilder().WithTitle("Test1").Build();

			await dbContext.Anime.AddAsync(anime1);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAnimeCountAsync();

			// Then
			result.Should().Be(1);
		}

		[Fact]
		public async Task CountAsync_GivenMultipleAnime_ShouldReturnCount()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);
			var anime1 = new AnimeBuilder().WithTitle("Test1").Build();
			var anime2 = new AnimeBuilder().WithTitle("Test2").Build();
			var anime3 = new AnimeBuilder().WithTitle("Test3").Build();

			await dbContext.Anime.AddAsync(anime1);
			await dbContext.Anime.AddAsync(anime2);
			await dbContext.Anime.AddAsync(anime3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAnimeCountAsync();

			// Then
			result.Should().Be(3);
		}

		[Fact]
		public async Task GetOrderedPageAsync_GivenNoAnime_ShouldReturnEmpty()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);

			// When
			var result = await repository.GetOrderedPageAsync(x => true);

			// Then
			using (new AssertionScope())
			{
				result.TotalCount.Should().Be(0);
				result.Results.Should().BeEmpty();
			}
		}

		[Fact]
		public async Task GetOrderedPageAsync_GivenMultipleAnime_ShouldReturnAll()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);
			var anime1 = new AnimeBuilder().WithTitle("Test1").Build();
			var anime2 = new AnimeBuilder().WithTitle("Test2").Build();
			var anime3 = new AnimeBuilder().WithTitle("Test3").Build();

			await dbContext.Anime.AddAsync(anime1);
			await dbContext.Anime.AddAsync(anime2);
			await dbContext.Anime.AddAsync(anime3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetOrderedPageAsync(x => true);

			// Then
			using (new AssertionScope())
			{
				result.TotalCount.Should().Be(3);
				result.Results.Should().HaveCount(3);
			}
		}

		[Fact]
		public async Task GetOrderedPageAsync_GivenMultipleWithPagesize_ShouldReturnOnlyOnePage()
		{
			// Given
			const int pageSize = 2;

			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);
			var anime1 = new AnimeBuilder().WithTitle("Test1").Build();
			var anime2 = new AnimeBuilder().WithTitle("Test2").Build();
			var anime3 = new AnimeBuilder().WithTitle("Test3").Build();

			await dbContext.Anime.AddAsync(anime1);
			await dbContext.Anime.AddAsync(anime2);
			await dbContext.Anime.AddAsync(anime3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetOrderedPageAsync(x => true, 0, pageSize);

			// Then// Then
			using (new AssertionScope())
			{
				result.TotalCount.Should().Be(3);
				result.Results.Should().HaveCount(pageSize);
				result.PageSize.Should().Be(2);
			}
		}

		[Fact]
		public async Task GetOrderedPageAsync_GivenMultipleWithPagesizeAndPage_ShouldReturnOnlyOnePage()
		{
			// Given
			const int pageSize = 2;
			const int pageNumber = 1;

			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);
			var anime1 = new AnimeBuilder().WithTitle("Test1").Build();
			var anime2 = new AnimeBuilder().WithTitle("Test2").Build();
			var anime3 = new AnimeBuilder().WithTitle("Test3").Build();

			await dbContext.Anime.AddAsync(anime1);
			await dbContext.Anime.AddAsync(anime2);
			await dbContext.Anime.AddAsync(anime3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetOrderedPageAsync(x => true, pageNumber, pageSize);

			// Then// Then
			using (new AssertionScope())
			{
				result.TotalCount.Should().Be(3);
				result.Results.Should().HaveCount(1);
				result.PageSize.Should().Be(pageSize);
				result.Page.Should().Be(pageNumber);
			}
		}

		[Fact]
		public async Task GetOrderedPageAsync_GivenMultipleWithPredicate_ShouldReturnOnlyOnePage()
		{
			// Given
			const int pageSize = 2;

			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);
			var anime1 = new AnimeBuilder().WithTitle("Test1").Build();
			var anime2 = new AnimeBuilder().WithTitle("Test2").Build();
			var anime3 = new AnimeBuilder().WithTitle("Test3").Build();

			await dbContext.Anime.AddAsync(anime1);
			await dbContext.Anime.AddAsync(anime2);
			await dbContext.Anime.AddAsync(anime3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetOrderedPageAsync(x => x.Title.EndsWith("1"), 0, pageSize);

			// Then// Then
			using (new AssertionScope())
			{
				result.TotalCount.Should().Be(1);
				result.Results.Should().HaveCount(1);
				result.PageSize.Should().Be(pageSize);
			}
		}

		[Fact]
		public async Task GetOrderedPageAsync_GivenMultipleWithPredicate_ShouldReturnEmpty()
		{
			// Given
			const int pageSize = 2;
			const int pageNumber = 2;

			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new AnimeRepository(dbContext);
			var anime1 = new AnimeBuilder().WithTitle("Test1").Build();
			var anime2 = new AnimeBuilder().WithTitle("Test2").Build();
			var anime3 = new AnimeBuilder().WithTitle("Test3").Build();

			await dbContext.Anime.AddAsync(anime1);
			await dbContext.Anime.AddAsync(anime2);
			await dbContext.Anime.AddAsync(anime3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetOrderedPageAsync(x => x.Title.EndsWith("1"), pageNumber, pageSize);

			// Then// Then
			using (new AssertionScope())
			{
				result.TotalCount.Should().Be(1);
				result.Results.Should().BeEmpty();
				result.Page.Should().Be(pageNumber);
				result.PageSize.Should().Be(pageSize);
			}
		}
	}
}
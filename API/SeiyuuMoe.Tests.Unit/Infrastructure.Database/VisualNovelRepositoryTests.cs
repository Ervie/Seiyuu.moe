using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Infrastructure.Database.VisualNovels;
using SeiyuuMoe.Tests.Common.Builders.Model;
using SeiyuuMoe.Tests.Common.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Infrastructure.Database
{
	public class VisualNovelRepositoryTests
	{
		[Fact]
		public async Task AddAsync_GivenEmptyVn_ShouldAddVisualNovel()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new VisualNovelRepository(dbContext);

			// When
			await repository.AddAsync(new VisualNovelBuilder().Build());

			// Then
			dbContext.VisualNovels.Should().ContainSingle();
		}

		[Fact]
		public async Task AddAsync_GivenVisualNovel_ShouldAddVisualNovel()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new VisualNovelRepository(dbContext);

			const string expectedTitle = "ExpectedTitle";
			const string expectedImageUrl = "ExpectedImageUrl";
			const string expectedJapaneseTitle = "期待される日本語タイトル";
			const string expectedTitleSynonyms = "expectedTitleSynonyms";
			const string expectedAbout = "ExpectedAbout";
			const long expectedVndbId = 1;

			var vn = new VisualNovelBuilder()
				.WithTitle(expectedTitle)
				.WithImageUrl(expectedImageUrl)
				.WithVndbId(expectedVndbId)
				.WithJapaneseTitle(expectedJapaneseTitle)
				.WithTitleSynonyms(expectedTitleSynonyms)
				.WithAbout(expectedAbout)
				.Build();

			// When
			await repository.AddAsync(vn);

			// Then
			var allVns = await dbContext.VisualNovels.ToListAsync();
			var newVn = await dbContext.VisualNovels.FirstOrDefaultAsync();

			using (new AssertionScope())
			{
				allVns.Should().ContainSingle();

				newVn.Should().NotBeNull();
				newVn.Title.Should().Be(expectedTitle);
				newVn.ImageUrl.Should().Be(expectedImageUrl);
				newVn.VndbId.Should().Be(expectedVndbId);
				newVn.TitleOriginal.Should().Be(expectedJapaneseTitle);
				newVn.Alias.Should().Be(expectedTitleSynonyms);
				newVn.About.Should().Be(expectedAbout);
			}
		}

		[Fact]
		public async Task AddAsync_GivenDuplicatedKeyVisualNovel_ShouldThrowException()
		{
			// Given
			var id = Guid.NewGuid();
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new VisualNovelRepository(dbContext);

			await repository.AddAsync(new VisualNovelBuilder().WithId(id).Build());

			// When
			Func<Task> func = repository.Awaiting(x => x.AddAsync(new VisualNovelBuilder().WithId(id).Build()));

			// Then
			func.Should().Throw<Exception>();
		}

		[Fact]
		public void UpdateAsync_GivenNotExistingVisualNovel_ShouldThrowExcepton()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new VisualNovelRepository(dbContext);

			// When
			Func<Task> func = repository.Awaiting(x => x.UpdateAsync(new VisualNovelBuilder().WithId(Guid.NewGuid()).Build()));

			// Then
			func.Should().Throw<Exception>();
		}

		[Fact]
		public async Task UpdateAsync_GivenExistingVisualNovel_ShouldUpdate()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new VisualNovelRepository(dbContext);
			var vn = new VisualNovelBuilder().WithTitle("Test").Build();

			await dbContext.VisualNovels.AddAsync(vn);
			await dbContext.SaveChangesAsync();

			vn.Title = "Updated";

			// When
			await repository.UpdateAsync(vn);

			// Then
			var allVns = await dbContext.VisualNovels.ToListAsync();

			using (new AssertionScope())
			{
				var updateVn = allVns.SingleOrDefault();
				updateVn.Should().NotBeNull();
				updateVn.Title.Should().Be("Updated");
				updateVn.ModificationDate.Should().HaveDay(DateTime.UtcNow.Day);
			}
		}

		[Fact]
		public async Task GetAsync_GivenNoVisualNovel_ShouldReturnNull()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new VisualNovelRepository(dbContext);

			// When
			var result = await repository.GetAsync(1);

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetAsync_GivenVisualNovelWithOtherId_ShouldReturnNull()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new VisualNovelRepository(dbContext);
			var vn1 = new VisualNovelBuilder().WithVndbId(1).Build();

			await dbContext.VisualNovels.AddAsync(vn1);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAsync(2);

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetAsync_GivenExistingVisualNovelWithVndbId_ShouldReturnResult()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new VisualNovelRepository(dbContext);
			var vn1 = new VisualNovelBuilder().WithVndbId(1).Build();

			await dbContext.VisualNovels.AddAsync(vn1);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAsync(1);

			// Then
			result.Should().NotBeNull();
		}

		[Fact]
		public async Task GetAsync_GivenMultipleExistingVisualNovelOneOfWhichWithVndbId_ShouldReturnResult()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new VisualNovelRepository(dbContext);
			var vn1 = new VisualNovelBuilder().WithVndbId(1).Build();
			var vn2 = new VisualNovelBuilder().WithVndbId(2).Build();
			var vn3 = new VisualNovelBuilder().WithVndbId(3).Build();
			var vn4 = new VisualNovelBuilder().WithVndbId(4).Build();
			var vn5 = new VisualNovelBuilder().WithVndbId(5).Build();

			await dbContext.VisualNovels.AddAsync(vn1);
			await dbContext.VisualNovels.AddAsync(vn2);
			await dbContext.VisualNovels.AddAsync(vn3);
			await dbContext.VisualNovels.AddAsync(vn4);
			await dbContext.VisualNovels.AddAsync(vn5);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAsync(3);

			// Then
			result.Should().NotBeNull();
			result.VndbId.Should().Be(3);
		}

		[Fact]
		public async Task GetAsync_GivenExistingVisualNovelWithVndbId_ShouldReturnResultWithEntities()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new VisualNovelRepository(dbContext);
			var vn = new VisualNovelBuilder()
				 .WithTitle("Test")
				 .WithTitle("ExpectedTitle")
				 .WithImageUrl("ExpectedImageUrl")
				 .WithAbout("ExpectedAbout")
				 .WithVndbId(1)
				 .Build();

			await dbContext.VisualNovels.AddAsync(vn);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAsync(1);

			// Then
			using (new AssertionScope())
			{
				result.Should().NotBeNull();
				result.VndbId.Should().Be(1);
				result.Title.Should().Be("ExpectedTitle");
				result.ImageUrl.Should().Be("ExpectedImageUrl");
				result.About.Should().Be("ExpectedAbout");
			}
		}
	}
}
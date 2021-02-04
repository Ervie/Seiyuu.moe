using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Infrastructure.Database.Seiyuus;
using SeiyuuMoe.Tests.Common.Builders.Model;
using SeiyuuMoe.Tests.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tests.Infrastructure
{
	public class SeiyuuRepositoryTests
	{
		[Fact]
		public async Task AddAsync_GivenEmptySeiyuu_ShouldAdd()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRepository(dbContext);

			// When
			await repository.AddAsync(new SeiyuuBuilder().Build());

			// Then
			var allSeiyuu = await dbContext.Seiyuus.ToListAsync();

			allSeiyuu.Should().ContainSingle();
		}

		[Fact]
		public async Task AddAsync_GivenSeiyuu_ShouldAdd()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRepository(dbContext);

			const string expectedName = "ExpectedName";
			const string expectedImageUrl = "ExpectedImageUrl";
			const string expectedKanjiName = "期待される日本語タイトル";
			const string expectedAbout = "ExpectedAbout";
			var expectedDateOfBirth = new DateTime(1990, 1, 1);
			const int expectedPopularity = 1000;
			const long expectedMalId = 1;

			var seiyuu = new SeiyuuBuilder()
				.WithName(expectedName)
				.WithImageUrl(expectedImageUrl)
				.WithMalId(expectedMalId)
				.WithJapaneseName(expectedKanjiName)
				.WithAbout(expectedAbout)
				.WithBirthday(expectedDateOfBirth)
				.WithPopularity(expectedPopularity)
				.Build();

			// When
			await repository.AddAsync(seiyuu);

			// Then
			var allAnime = await dbContext.Seiyuus.ToListAsync();
			var newSeiyuu = await dbContext.Seiyuus.FirstOrDefaultAsync();

			using (new AssertionScope())
			{
				allAnime.Should().ContainSingle();

				newSeiyuu.Should().NotBeNull();
				newSeiyuu.Name.Should().Be(expectedName);
				newSeiyuu.ImageUrl.Should().Be(expectedImageUrl);
				newSeiyuu.MalId.Should().Be(expectedMalId);
				newSeiyuu.KanjiName.Should().Be(expectedKanjiName);
				newSeiyuu.Birthday.Should().Be(expectedDateOfBirth);
				newSeiyuu.About.Should().Be(expectedAbout);
				newSeiyuu.Popularity.Should().Be(expectedPopularity);
			}
		}

		[Fact]
		public async Task AddAsync_GivenDuplicatedKey_ShouldThrowException()
		{
			// Given
			var animeId = Guid.NewGuid();
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRepository(dbContext);

			await repository.AddAsync(new SeiyuuBuilder().WithId(animeId).Build());

			// When
			Func<Task> func = repository.Awaiting(x => x.AddAsync(new SeiyuuBuilder().WithId(animeId).Build()));

			// Then
			func.Should().Throw<Exception>();
		}

		[Fact]
		public void UpdateAsync_GivenNotExistingSeiyuu_ShouldThrowExcepton()
		{
			// Given
			var id = Guid.NewGuid();
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRepository(dbContext);

			// When
			Func<Task> func = repository.Awaiting(x => x.UpdateAsync(new SeiyuuBuilder().WithId(id).Build()));

			// Then
			func.Should().Throw<Exception>();
		}

		[Fact]
		public async Task UpdateAsync_GivenExistingSeiyuu_ShouldUpdate()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRepository(dbContext);
			var seiyuu = new SeiyuuBuilder().WithName("Test").Build();

			await dbContext.Seiyuus.AddAsync(seiyuu);
			await dbContext.SaveChangesAsync();

			seiyuu.Name = "Updated";

			// When
			await repository.UpdateAsync(seiyuu);

			// Then
			var allSeiyuu = await dbContext.Seiyuus.ToListAsync();

			using (new AssertionScope())
			{
				var updatedSeiyuu = allSeiyuu.SingleOrDefault();
				updatedSeiyuu.Should().NotBeNull();
				updatedSeiyuu.Name.Should().Be("Updated");
				updatedSeiyuu.ModificationDate.Should().HaveDay(DateTime.UtcNow.Day);
			}
		}

		[Fact]
		public async Task CountAsync_GivenNoSeiyuu_ShouldReturnZero()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRepository(dbContext);

			// When
			var result = await repository.GetSeiyuuCountAsync();

			// Then
			result.Should().Be(0);
		}

		[Fact]
		public async Task CountAsync_GivenSingleSeiyuu_ShouldReturnOne()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRepository(dbContext);
			var seiyuu = new SeiyuuBuilder().WithName("Test1").Build();

			await dbContext.Seiyuus.AddAsync(seiyuu);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetSeiyuuCountAsync();

			// Then
			result.Should().Be(1);
		}

		[Fact]
		public async Task GetByMalIdAsync_GivenNoSeiyuu_ShouldReturnNull()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRepository(dbContext);

			// When
			var result = await repository.GetByMalIdAsync(1);

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetByMalIdAsync_GivenSeiyuuWithOtherId_ShouldReturnNull()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRepository(dbContext);
			var seiyuu = new SeiyuuBuilder().WithMalId(1).Build();

			await dbContext.Seiyuus.AddAsync(seiyuu);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetByMalIdAsync(2);

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetByMalIdAsync_GivenExistingSeiyuuWithId_ShouldReturnResult()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRepository(dbContext);
			var seiyuu = new SeiyuuBuilder().WithMalId(1).Build();

			await dbContext.Seiyuus.AddAsync(seiyuu);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetByMalIdAsync(1);

			// Then
			result.Should().NotBeNull();
		}

		[Fact]
		public async Task GetByMalIdAsync_GivenMultipleExistingSeiyuuOneOfWhichWithId_ShouldReturnResult()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRepository(dbContext);
			var seiyuu1 = new SeiyuuBuilder().WithMalId(1).Build();
			var seiyuu2 = new SeiyuuBuilder().WithMalId(2).Build();
			var seiyuu3 = new SeiyuuBuilder().WithMalId(3).Build();
			var seiyuu4 = new SeiyuuBuilder().WithMalId(4).Build();
			var seiyuu5 = new SeiyuuBuilder().WithMalId(5).Build();

			await dbContext.Seiyuus.AddAsync(seiyuu1);
			await dbContext.Seiyuus.AddAsync(seiyuu2);
			await dbContext.Seiyuus.AddAsync(seiyuu3);
			await dbContext.Seiyuus.AddAsync(seiyuu4);
			await dbContext.Seiyuus.AddAsync(seiyuu5);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetByMalIdAsync(3);

			// Then
			result.Should().NotBeNull();
			result.MalId.Should().Be(3);
		}

		[Fact]
		public async Task GetByMalIdAsync_GivenExistingSeiyuuWithId_ShouldReturnResultWithEntities()
		{
			// Given
			const string expectedName = "ExpectedName";
			const string expectedImageUrl = "ExpectedImageUrl";
			const string expectedKanjiName = "期待される日本語タイトル";
			const string expectedAbout = "ExpectedAbout";
			var expectedDateOfBirth = new DateTime(1990, 1, 1);
			const int expectedPopularity = 1000;
			const long expectedMalId = 1;

			var seiyuu = new SeiyuuBuilder()
				.WithName(expectedName)
				.WithImageUrl(expectedImageUrl)
				.WithMalId(expectedMalId)
				.WithJapaneseName(expectedKanjiName)
				.WithAbout(expectedAbout)
				.WithBirthday(expectedDateOfBirth)
				.WithPopularity(expectedPopularity)
				.Build();

			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRepository(dbContext);

			await dbContext.Seiyuus.AddAsync(seiyuu);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetByMalIdAsync(1);

			// Then
			using (new AssertionScope())
			{
				result.Should().NotBeNull();
				result.Name.Should().Be(expectedName);
				result.ImageUrl.Should().Be(expectedImageUrl);
				result.MalId.Should().Be(expectedMalId);
				result.KanjiName.Should().Be(expectedKanjiName);
				result.Birthday.Should().Be(expectedDateOfBirth);
				result.About.Should().Be(expectedAbout);
				result.Popularity.Should().Be(expectedPopularity);
			}
		}

		[Fact]
		public async Task GetAllIdsAsync_GivenNoSeiyuu_ShouldReturnEmpty()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRepository(dbContext);

			// When
			var result = await repository.GetAllIdsAsync();

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllIdsAsync_GivenSingleSeiyuu_ShouldReturnSingle()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRepository(dbContext);
			var seiyuu = new SeiyuuBuilder().WithName("Test1").Build();

			await dbContext.Seiyuus.AddAsync(seiyuu);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllIdsAsync();

			// Then
			result.Should().HaveCount(1);
		}

		[Fact]
		public async Task GetAllIdsAsync_GivenMultipleSeiyuu_ShouldReturnMultiple()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRepository(dbContext);
			var seiyuuCollection = new List<Seiyuu>
			{
				new SeiyuuBuilder().WithName("Test1").Build(),
				new SeiyuuBuilder().WithName("Test2").Build(),
				new SeiyuuBuilder().WithName("Test3").Build(),
				new SeiyuuBuilder().WithName("Test4").Build(),
				new SeiyuuBuilder().WithName("Test5").Build()
			};

			await dbContext.Seiyuus.AddRangeAsync(seiyuuCollection);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllIdsAsync();

			// Then
			result.Should().HaveCount(5);
		}

		[Fact]
		public async Task GetOrderedPageByPopularityAsync_GivenNoSeiyuu_ShouldReturnEmpty()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRepository(dbContext);

			// When
			var result = await repository.GetOrderedPageByPopularityAsync(x => true);

			// Then
			using (new AssertionScope())
			{
				result.TotalCount.Should().Be(0);
				result.Results.Should().BeEmpty();
			}
		}

		[Fact]
		public async Task GetOrderedPageByPopularityAsync_GivenMultipleSeiyuu_ShouldReturnAll()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRepository(dbContext);
			var seiyuu1 = new SeiyuuBuilder().WithName("Test1").Build();
			var seiyuu2 = new SeiyuuBuilder().WithName("Test2").Build();
			var seiyuu3 = new SeiyuuBuilder().WithName("Test3").Build();

			await dbContext.Seiyuus.AddAsync(seiyuu1);
			await dbContext.Seiyuus.AddAsync(seiyuu2);
			await dbContext.Seiyuus.AddAsync(seiyuu3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetOrderedPageByPopularityAsync(x => true);

			// Then
			using (new AssertionScope())
			{
				result.TotalCount.Should().Be(3);
				result.Results.Should().HaveCount(3);
			}
		}

		[Fact]
		public async Task GetOrderedPageByPopularityAsync_GivenMultipleWithPagesize_ShouldReturnOnlyOnePage()
		{
			// Given
			const int pageSize = 2;

			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRepository(dbContext);
			var seiyuu1 = new SeiyuuBuilder().WithName("Test1").Build();
			var seiyuu2 = new SeiyuuBuilder().WithName("Test2").Build();
			var seiyuu3 = new SeiyuuBuilder().WithName("Test3").Build();

			await dbContext.Seiyuus.AddAsync(seiyuu1);
			await dbContext.Seiyuus.AddAsync(seiyuu2);
			await dbContext.Seiyuus.AddAsync(seiyuu3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetOrderedPageByPopularityAsync(x => true, 0, pageSize);

			// Then// Then
			using (new AssertionScope())
			{
				result.TotalCount.Should().Be(3);
				result.Results.Should().HaveCount(pageSize);
				result.PageSize.Should().Be(2);
			}
		}

		[Fact]
		public async Task GetOrderedPageByPopularityAsync_GivenMultipleWithPagesizeAndPage_ShouldReturnOnlyOnePage()
		{
			// Given
			const int pageSize = 2;
			const int pageNumber = 1;

			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRepository(dbContext);
			var seiyuu1 = new SeiyuuBuilder().WithName("Test1").Build();
			var seiyuu2 = new SeiyuuBuilder().WithName("Test2").Build();
			var seiyuu3 = new SeiyuuBuilder().WithName("Test3").Build();

			await dbContext.Seiyuus.AddAsync(seiyuu1);
			await dbContext.Seiyuus.AddAsync(seiyuu2);
			await dbContext.Seiyuus.AddAsync(seiyuu3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetOrderedPageByPopularityAsync(x => true, pageNumber, pageSize);

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
		public async Task GetOrderedPageByPopularityAsync_GivenMultipleWithPredicate_ShouldReturnOnlyOnePage()
		{
			// Given
			const int pageSize = 2;

			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRepository(dbContext);
			var seiyuu1 = new SeiyuuBuilder().WithName("Test1").Build();
			var seiyuu2 = new SeiyuuBuilder().WithName("Test2").Build();
			var seiyuu3 = new SeiyuuBuilder().WithName("Test3").Build();

			await dbContext.Seiyuus.AddAsync(seiyuu1);
			await dbContext.Seiyuus.AddAsync(seiyuu2);
			await dbContext.Seiyuus.AddAsync(seiyuu3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetOrderedPageByPopularityAsync(x => x.Name.EndsWith("1"), 0, pageSize);

			// Then// Then
			using (new AssertionScope())
			{
				result.TotalCount.Should().Be(1);
				result.Results.Should().HaveCount(1);
				result.PageSize.Should().Be(pageSize);
			}
		}

		[Fact]
		public async Task GetOrderedPageByPopularityAsync_GivenMultipleWithPredicate_ShouldReturnEmpty()
		{
			// Given
			const int pageSize = 2;
			const int pageNumber = 2;

			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new SeiyuuRepository(dbContext);
			var seiyuu1 = new SeiyuuBuilder().WithName("Test1").Build();
			var seiyuu2 = new SeiyuuBuilder().WithName("Test2").Build();
			var seiyuu3 = new SeiyuuBuilder().WithName("Test3").Build();

			await dbContext.Seiyuus.AddAsync(seiyuu1);
			await dbContext.Seiyuus.AddAsync(seiyuu2);
			await dbContext.Seiyuus.AddAsync(seiyuu3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetOrderedPageByPopularityAsync(x => x.Name.EndsWith("1"), pageNumber, pageSize);

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
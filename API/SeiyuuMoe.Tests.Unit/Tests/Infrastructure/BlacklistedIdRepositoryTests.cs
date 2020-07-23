using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Infrastructure.Blacklisting;
using SeiyuuMoe.Tests.Unit.Builders.Model;
using SeiyuuMoe.Tests.Unit.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tests.Infrastructure
{
	public class BlacklistedIdRepositoryTests
	{
		[Fact]
		public async Task AddAsync_GivenEmptyBlacklistedId_ShouldAdd()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new BlacklistedIdRepository(dbContext);

			// When
			await repository.AddAsync(new BlacklistedIdBuilder().Build());

			// Then
			var allAnime = await dbContext.BlacklistedId.ToListAsync();

			allAnime.Should().ContainSingle();
		}

		[Fact]
		public async Task AddAsync_GivenBlacklistedId_ShouldAdd()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new BlacklistedIdRepository(dbContext);

			const string expectedEntityType = "ExpectedEntityType";
			const string expectedReason = "ExpectedReason";
			const long expectedMalId = 1;

			var blacklistedId = new BlacklistedIdBuilder()
				.WithEntityType(expectedEntityType)
				.WithReason(expectedReason)
				.WithMalId(expectedMalId)
				.Build();

			// When
			await repository.AddAsync(blacklistedId);

			// Then
			var allBlacklists = await dbContext.BlacklistedId.ToListAsync();
			var newBlacklistedId = await dbContext.BlacklistedId.FirstOrDefaultAsync();

			using (new AssertionScope())
			{
				allBlacklists.Should().ContainSingle();

				newBlacklistedId.Should().NotBeNull();
				newBlacklistedId.Reason.Should().Be(expectedReason);
				newBlacklistedId.EntityType.Should().Be(expectedEntityType);
				newBlacklistedId.MalId.Should().Be(expectedMalId);
			}
		}

		[Fact]
		public async Task AddAsync_GivenDuplicatedKeyAnime_ShouldThrowException()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new BlacklistedIdRepository(dbContext);

			await repository.AddAsync(new BlacklistedIdBuilder().WithId(1).Build());

			// When
			Func<Task> func = repository.Awaiting(x => x.AddAsync(new BlacklistedIdBuilder().WithId(1).Build()));

			// Then
			func.Should().Throw<Exception>();
		}
	}
}
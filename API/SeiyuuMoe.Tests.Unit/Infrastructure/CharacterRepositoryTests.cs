using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Infrastructure.Database.Animes;
using SeiyuuMoe.Tests.Common.Builders.Model;
using SeiyuuMoe.Tests.Common.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tests.Infrastructure
{
	public class CharacterRepositoryTests
	{
		[Fact]
		public async Task AddAsync_GivenEmptyCharacter_ShouldAdd()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new CharacterRepository(dbContext);

			// When
			await repository.AddAsync(new CharacterBuilder().Build());

			// Then
			var allAnime = await dbContext.AnimeCharacters.ToListAsync();

			allAnime.Should().ContainSingle();
		}

		[Fact]
		public async Task AddAsync_GivenCharacter_ShouldAdd()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new CharacterRepository(dbContext);

			const string expectedName = "ExpectedName";
			const string expectedImageUrl = "ExpectedImageUrl";
			const string expectedKanjiName = "期待される日本語タイトル";
			const string expectedNicknames = "expectedNicknames";
			const string expectedAbout = "ExpectedAbout";
			const long expectedMalId = 1;

			var character = new CharacterBuilder()
				.WithName(expectedName)
				.WithImageUrl(expectedImageUrl)
				.WithMalId(expectedMalId)
				.WithKanjiName(expectedKanjiName)
				.WithNicknames(expectedNicknames)
				.WithAbout(expectedAbout)
				.Build();

			// When
			await repository.AddAsync(character);

			// Then
			var allCharacters = await dbContext.AnimeCharacters.ToListAsync();
			var newCharacter = await dbContext.AnimeCharacters.FirstOrDefaultAsync();

			using (new AssertionScope())
			{
				allCharacters.Should().ContainSingle();

				newCharacter.Should().NotBeNull();
				newCharacter.Name.Should().Be(expectedName);
				newCharacter.ImageUrl.Should().Be(expectedImageUrl);
				newCharacter.MalId.Should().Be(expectedMalId);
				newCharacter.KanjiName.Should().Be(expectedKanjiName);
				newCharacter.Nicknames.Should().Be(expectedNicknames);
				newCharacter.About.Should().Be(expectedAbout);
			}
		}

		[Fact]
		public void UpdateAsync_GivenNotExistingCharacter_ShouldThrowExcepton()
		{
			// Given
			var id = Guid.NewGuid();
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new CharacterRepository(dbContext);

			// When
			Func<Task> func = repository.Awaiting(x => x.UpdateAsync(new CharacterBuilder().WithId(id).Build()));

			// Then
			func.Should().Throw<Exception>();
		}

		[Fact]
		public async Task UpdateAsync_GivenExistingCharacter_ShouldUpdate()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new CharacterRepository(dbContext);
			var character = new CharacterBuilder().WithName("Test").Build();

			await dbContext.AnimeCharacters.AddAsync(character);
			await dbContext.SaveChangesAsync();

			character.Name = "Updated";

			// When
			await repository.UpdateAsync(character);

			// Then
			var allCharacters = await dbContext.AnimeCharacters.ToListAsync();

			using (new AssertionScope())
			{
				var updatedCharacter = allCharacters.SingleOrDefault();
				updatedCharacter.Should().NotBeNull();
				updatedCharacter.Name.Should().Be("Updated");
				updatedCharacter.ModificationDate.Should().HaveDay(DateTime.UtcNow.Day);
			}
		}

		[Fact]
		public async Task AddAsync_GivenDuplicatedKeyCharacter_ShouldThrowException()
		{
			// Given
			var id = Guid.NewGuid();
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new CharacterRepository(dbContext);

			await repository.AddAsync(new CharacterBuilder().WithId(id).Build());

			// When
			Func<Task> func = repository.Awaiting(x => x.AddAsync(new CharacterBuilder().WithId(id).Build()));

			// Then
			func.Should().Throw<Exception>();
		}

		[Fact]
		public async Task GetAsync_GivenNoCharacter_ShouldReturnNull()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new CharacterRepository(dbContext);

			// When
			var result = await repository.GetAsync(1);

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetAsync_GivenCharacterWithOtherId_ShouldReturnNull()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new CharacterRepository(dbContext);
			var character = new CharacterBuilder().WithMalId(1).Build();

			await dbContext.AnimeCharacters.AddAsync(character);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAsync(2);

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetAsync_GivenExistingCharacterWithId_ShouldReturnResult()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new CharacterRepository(dbContext);
			var character = new CharacterBuilder().WithMalId(1).Build();

			await dbContext.AnimeCharacters.AddAsync(character);
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
			var repository = new CharacterRepository(dbContext);
			var character1 = new CharacterBuilder().WithMalId(1).Build();
			var character2 = new CharacterBuilder().WithMalId(2).Build();
			var character3 = new CharacterBuilder().WithMalId(3).Build();
			var character4 = new CharacterBuilder().WithMalId(4).Build();
			var character5 = new CharacterBuilder().WithMalId(5).Build();

			await dbContext.AnimeCharacters.AddAsync(character1);
			await dbContext.AnimeCharacters.AddAsync(character2);
			await dbContext.AnimeCharacters.AddAsync(character3);
			await dbContext.AnimeCharacters.AddAsync(character4);
			await dbContext.AnimeCharacters.AddAsync(character5);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAsync(3);

			// Then
			result.Should().NotBeNull();
			result.MalId.Should().Be(3);
		}

		[Fact]
		public async Task CountAsync_GivenNoCharacters_ShouldReturnZero()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new CharacterRepository(dbContext);

			// When
			var result = await repository.GetCountAsync();

			// Then
			result.Should().Be(0);
		}

		[Fact]
		public async Task CountAsync_GivenSingleCharacter_ShouldReturnOne()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new CharacterRepository(dbContext);
			var character = new CharacterBuilder().WithName("Naruto").Build();

			await dbContext.AnimeCharacters.AddAsync(character);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetCountAsync();

			// Then
			result.Should().Be(1);
		}

		[Fact]
		public async Task CountAsync_GivenMultipleCharacters_ShouldReturnCount()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new CharacterRepository(dbContext);
			var character1 = new CharacterBuilder().WithName("Naruto").Build();
			var character2 = new CharacterBuilder().WithName("Sasuke").Build();
			var character3 = new CharacterBuilder().WithName("Sakura").Build();

			await dbContext.AnimeCharacters.AddAsync(character1);
			await dbContext.AnimeCharacters.AddAsync(character2);
			await dbContext.AnimeCharacters.AddAsync(character3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetCountAsync();

			// Then
			result.Should().Be(3);
		}

		[Fact]
		public async Task GetPageAsync_GivenNoCharacter_ShouldReturnEmpty()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new CharacterRepository(dbContext);

			// When
			var result = await repository.GetPageAsync();

			// Then
			using (new AssertionScope())
			{
				result.TotalCount.Should().Be(0);
				result.Results.Should().BeEmpty();
			}
		}

		[Fact]
		public async Task GetPageAsync_GivenMultipleAnime_ShouldReturnAll()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new CharacterRepository(dbContext);
			var character1 = new CharacterBuilder().WithName("Test1").Build();
			var character2 = new CharacterBuilder().WithName("Test2").Build();
			var character3 = new CharacterBuilder().WithName("Test3").Build();

			await dbContext.AnimeCharacters.AddAsync(character1);
			await dbContext.AnimeCharacters.AddAsync(character2);
			await dbContext.AnimeCharacters.AddAsync(character3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetPageAsync();

			// Then
			using (new AssertionScope())
			{
				result.TotalCount.Should().Be(3);
				result.Results.Should().HaveCount(3);
			}
		}

		[Fact]
		public async Task GetPageAsync_GivenMultipleWithPagesize_ShouldReturnOnlyOnePage()
		{
			// Given
			const int pageSize = 2;

			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new CharacterRepository(dbContext);
			var character1 = new CharacterBuilder().WithName("Test1").Build();
			var character2 = new CharacterBuilder().WithName("Test2").Build();
			var character3 = new CharacterBuilder().WithName("Test3").Build();

			await dbContext.AnimeCharacters.AddAsync(character1);
			await dbContext.AnimeCharacters.AddAsync(character2);
			await dbContext.AnimeCharacters.AddAsync(character3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetPageAsync(0, pageSize);

			// Then// Then
			using (new AssertionScope())
			{
				result.TotalCount.Should().Be(3);
				result.Results.Should().HaveCount(pageSize);
				result.PageSize.Should().Be(2);
			}
		}

		[Fact]
		public async Task GetPageAsync_GivenMultipleWithPagesizeAndPage_ShouldReturnOnlyOnePage()
		{
			// Given
			const int pageSize = 2;
			const int pageNumber = 1;

			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new CharacterRepository(dbContext);
			var character1 = new CharacterBuilder().WithName("Test1").Build();
			var character2 = new CharacterBuilder().WithName("Test2").Build();
			var character3 = new CharacterBuilder().WithName("Test3").Build();

			await dbContext.AnimeCharacters.AddAsync(character1);
			await dbContext.AnimeCharacters.AddAsync(character2);
			await dbContext.AnimeCharacters.AddAsync(character3);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetPageAsync(pageNumber, pageSize);

			// Then// Then
			using (new AssertionScope())
			{
				result.TotalCount.Should().Be(3);
				result.Results.Should().HaveCount(1);
				result.PageSize.Should().Be(pageSize);
				result.Page.Should().Be(pageNumber);
			}
		}
	}
}
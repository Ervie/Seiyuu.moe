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
	public class VisualNovelCharacterRepositoryTests
	{
		[Fact]
		public async Task AddAsync_GivenEmptyCharacter_ShouldAdd()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new VisualNovelCharacterRepository(dbContext);

			// When
			await repository.AddAsync(new VisualNovelCharacterBuilder().Build());

			// Then
			dbContext.VisualNovelCharacters.Should().ContainSingle();
		}

		[Fact]
		public async Task AddAsync_GivenCharacter_ShouldAdd()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new VisualNovelCharacterRepository(dbContext);

			const string expectedName = "ExpectedName";
			const string expectedImageUrl = "ExpectedImageUrl";
			const string expectedKanjiName = "期待される日本語タイトル";
			const string expectedNicknames = "expectedNicknames";
			const string expectedAbout = "ExpectedAbout";
			const long expectedVndbId = 1;

			var character = new VisualNovelCharacterBuilder()
				.WithName(expectedName)
				.WithImageUrl(expectedImageUrl)
				.WithVndbId(expectedVndbId)
				.WithKanjiName(expectedKanjiName)
				.WithNicknames(expectedNicknames)
				.WithAbout(expectedAbout)
				.Build();

			// When
			await repository.AddAsync(character);

			// Then
			var allCharacters = await dbContext.VisualNovelCharacters.ToListAsync();
			var newCharacter = await dbContext.VisualNovelCharacters.FirstOrDefaultAsync();

			using (new AssertionScope())
			{
				allCharacters.Should().ContainSingle();

				newCharacter.Should().NotBeNull();
				newCharacter.Name.Should().Be(expectedName);
				newCharacter.ImageUrl.Should().Be(expectedImageUrl);
				newCharacter.VndbId.Should().Be(expectedVndbId);
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
			var repository = new VisualNovelCharacterRepository(dbContext);

			// When
			Func<Task> func = repository.Awaiting(x => x.UpdateAsync(new VisualNovelCharacterBuilder().WithId(id).Build()));

			// Then
			func.Should().Throw<Exception>();
		}

		[Fact]
		public async Task UpdateAsync_GivenExistingCharacter_ShouldUpdate()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new VisualNovelCharacterRepository(dbContext);
			var character = new VisualNovelCharacterBuilder().WithName("Test").Build();

			await dbContext.VisualNovelCharacters.AddAsync(character);
			await dbContext.SaveChangesAsync();

			character.Name = "Updated";

			// When
			await repository.UpdateAsync(character);

			// Then
			var allCharacters = await dbContext.VisualNovelCharacters.ToListAsync();

			using (new AssertionScope())
			{
				var updatedCharacter = allCharacters.SingleOrDefault();
				updatedCharacter.Should().NotBeNull();
				updatedCharacter.Name.Should().Be("Updated");
				updatedCharacter.ModificationDate.Should().HaveDay(DateTime.UtcNow.Day);
			}
		}

		[Fact]
		public async Task GetAsync_GivenNoCharacter_ShouldReturnNull()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var repository = new VisualNovelCharacterRepository(dbContext);

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
			var repository = new VisualNovelCharacterRepository(dbContext);
			var character = new VisualNovelCharacterBuilder().WithVndbId(1).Build();

			await dbContext.VisualNovelCharacters.AddAsync(character);
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
			var repository = new VisualNovelCharacterRepository(dbContext);
			var character = new VisualNovelCharacterBuilder().WithVndbId(1).Build();

			await dbContext.VisualNovelCharacters.AddAsync(character);
			await dbContext.SaveChangesAsync();

			// When
			var result = await repository.GetAsync(1);

			// Then
			result.Should().NotBeNull();
		}
	}
}
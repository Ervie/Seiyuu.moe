using FluentAssertions;
using FluentAssertions.Execution;
using SeiyuuMoe.Application.Animes.GetAnimeCardInfo;
using SeiyuuMoe.Infrastructure.Animes;
using SeiyuuMoe.Infrastructure.Context;
using SeiyuuMoe.Tests.Common.Builders.Model;
using SeiyuuMoe.Tests.Common.Helpers;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Component.Animes
{
	public class GetAnimeCardInfoQueryHandlerTests
	{
		[Fact]
		public async Task HandleAsync_GivenEmptyDatabase_ShouldReturnNotFoundResponse()
		{
			// Given
			const long animeMalId = 0;
			var dbContext = InMemoryDbProvider.GetDbContext();

			var handler = CreateHandler(dbContext);

			// When
			var result = await handler.HandleAsync(new GetAnimeCardInfoQuery(animeMalId));

			// Then
			using (new AssertionScope())
			{
				result.Found.Should().BeFalse();
				result.Payload.Should().BeNull();
			}
		}

		[Fact]
		public async Task HandleAsync_GivenExistingAnimeWithOtherMalId_ShouldReturnNotFoundResponse()
		{
			// Given
			const long expectedMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();


			var anime = new AnimeBuilder()
				.WithMalId(expectedMalId + 1)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext);

			// When
			var result = await handler.HandleAsync(new GetAnimeCardInfoQuery(expectedMalId));

			// Then
			using (new AssertionScope())
			{
				result.Found.Should().BeFalse();
				result.Payload.Should().BeNull();
			}
		}

		[Fact]
		public async Task HandleAsync_GivenExistingAnime_ShouldReturnAnimeCardDto()
		{
			// Given
			const string expectedTitle = "ExpectedTitle";
			const string expectedImageUrl = "ExpectedImageUrl";
			const string expectedJapaneseTitle = "期待される日本語タイトル";
			const string expectedTitleSynonyms = "expectedTitleSynonyms";
			const string expectedAbout = "ExpectedAbout";
			const string expectedType = "ExpectedAbout";
			const string expectedStatus = "ExpectedStatus";
			const long expectedMalId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();


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

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext);

			// When
			var result = await handler.HandleAsync(new GetAnimeCardInfoQuery(expectedMalId));

			// Then
			using (new AssertionScope())
			{
				result.Found.Should().BeTrue();
				result.Payload.Should().NotBeNull();

				result.Payload.Should().NotBeNull();
				result.Payload.Title.Should().Be(expectedTitle);
				result.Payload.ImageUrl.Should().Be(expectedImageUrl);
				result.Payload.MalId.Should().Be(expectedMalId);
				result.Payload.JapaneseTitle.Should().Be(expectedJapaneseTitle);
				result.Payload.TitleSynonyms.Should().Be(expectedTitleSynonyms);
				result.Payload.About.Should().Be(expectedAbout);
				result.Payload.Type.Should().Be(expectedType);
				result.Payload.Status.Should().Be(expectedStatus);
			}
		}

		private GetAnimeCardInfoQueryHandler CreateHandler(SeiyuuMoeContext seiyuuMoeContext)
		{
			var animeRepository = new AnimeRepository(seiyuuMoeContext);

			return new GetAnimeCardInfoQueryHandler(animeRepository);
		}
	}
}
using FluentAssertions;
using FluentAssertions.Execution;
using JikanDotNet;
using JikanDotNet.Exceptions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SeiyuuMoe.Domain.Services;
using SeiyuuMoe.Domain.SqsMessages;
using SeiyuuMoe.Infrastructure.Animes;
using SeiyuuMoe.Infrastructure.Context;
using SeiyuuMoe.Infrastructure.Seasons;
using SeiyuuMoe.MalBackgroundJobs.Application.Handlers;
using SeiyuuMoe.Tests.Common.Builders.Jikan;
using SeiyuuMoe.Tests.Common.Builders.Model;
using SeiyuuMoe.Tests.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Component.MalBackgroundJobs
{
	public class UpdateAnimeHandlerTests
	{
		[Fact]
		public async Task HandleAsync_GivenEmptyDatabase_ShouldNotThrowAndNotCallJikan()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			var command = new UpdateAnimeMessage
			{
				Id = Guid.NewGuid(),
				MalId = 1
			};

			// When
			var action = handler.Awaiting(x => x.HandleAsync(command));

			// Then
			using (new AssertionScope())
			{
				await action.Should().NotThrowAsync();
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnime(It.IsAny<long>()), Times.Never);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenNullResponseFromJikan_ShouldNotThrowAndCallJikan()
		{
			// Given
			const int malId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder().WithAnimeReturned(null);

			var anime = new AnimeBuilder()
				.WithMalId(malId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			var command = new UpdateAnimeMessage
			{
				Id = Guid.NewGuid(),
				MalId = malId
			};

			// When
			var action = handler.Awaiting(x => x.HandleAsync(command));

			// Then
			using (new AssertionScope())
			{
				await action.Should().NotThrowAsync();
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnime(malId), Times.Once);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenExceptionFromJikan_ShouldThrowAndCallJikan()
		{
			// Given
			const int malId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder().WithGetAnimeThrowing();

			var anime = new AnimeBuilder()
				.WithMalId(malId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			var command = new UpdateAnimeMessage
			{
				Id = Guid.NewGuid(),
				MalId = malId
			};

			// When
			var action = handler.Awaiting(x => x.HandleAsync(command));

			// Then
			using (new AssertionScope())
			{
				await action.Should().ThrowExactlyAsync<JikanRequestException>();
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnime(malId), Times.Once);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenCorrectResponse_ShouldUpdateBasicProperties()
		{
			// Given
			const int malId = 1;

			const string returnedTitle = "PostUpdateTitle";
			const string returnedAbout = "PostUpdateAbout";
			const string returnedEnglishTitle = "PostUpdateEnglish";
			const string returnedJapaneseTitle = "PostUpdateJapanese";
			const string returnedImageUrl = "PostUpdateImageUrl";
			const int returnedPopularity = 1;

			var returnedAnime = new Anime
			{
				Title = returnedTitle,
				Synopsis = returnedAbout,
				TitleEnglish = returnedEnglishTitle,
				TitleJapanese = returnedJapaneseTitle,
				ImageURL = returnedImageUrl,
				TitleSynonyms = new List<string>()
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithAnimeReturned(returnedAnime);

			var dbContext = InMemoryDbProvider.GetDbContext();
			var anime = new AnimeBuilder()
				.WithMalId(malId)
				.WithTitle("PreUpdateTitle")
				.WithAbout("PreUpdateAbout")
				.WithEnglishTitle("PreUpdateEnglish")
				.WithJapaneseTitle("PreUpdateJapaneses")
				.WithImageUrl("PreUpdateImage")
				.WithPopularity(0)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			var command = new UpdateAnimeMessage
			{
				Id = Guid.NewGuid(),
				MalId = malId
			};

			// When
			await handler.HandleAsync(command);

			// Then
			var updatedAnime = await dbContext.Animes.FirstAsync(x => x.MalId == malId);
			using (new AssertionScope())
			{
				updatedAnime.Title = returnedTitle;
				updatedAnime.About = returnedAbout;
				updatedAnime.EnglishTitle = returnedEnglishTitle;
				updatedAnime.KanjiTitle = returnedJapaneseTitle;
				updatedAnime.ImageUrl = returnedImageUrl;
				updatedAnime.Popularity = returnedPopularity;

				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnime(malId), Times.Once);
			}
		}

		private UpdateAnimeHandler CreateHandler(SeiyuuMoeContext dbcontext, IMalApiService apiService)
		{
			var animeRepository = new AnimeRepository(dbcontext);
			var seasonRepository = new SeasonRepository(dbcontext);

			return new UpdateAnimeHandler(animeRepository, seasonRepository, apiService);
		}
	}
}
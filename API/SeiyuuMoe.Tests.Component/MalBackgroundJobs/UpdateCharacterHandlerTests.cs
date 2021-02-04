using FluentAssertions;
using FluentAssertions.Execution;
using JikanDotNet;
using JikanDotNet.Exceptions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SeiyuuMoe.Domain.Services;
using SeiyuuMoe.Domain.SqsMessages;
using SeiyuuMoe.Infrastructure.Database.Animes;
using SeiyuuMoe.Infrastructure.Database.Context;
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
	public class UpdateCharacterHandlerTests
	{
		[Fact]
		public async Task HandleAsync_GivenEmptyDatabase_ShouldNotThrowAndNotCallJikan()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			var command = new UpdateCharacterMessage
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
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacter(It.IsAny<long>()), Times.Never);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenNullResponseFromJikan_ShouldNotThrowAndCallJikan()
		{
			// Given
			const int malId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder().WithCharacterReturned(null);

			var character = new CharacterBuilder()
				.WithMalId(malId)
				.Build();

			await dbContext.AddAsync(character);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			var command = new UpdateCharacterMessage
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
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacter(malId), Times.Once);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenExceptionFromJikan_ShouldThrowAndCallJikan()
		{
			// Given
			const int malId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder().WithGetCharacterThrowing();

			var anime = new CharacterBuilder()
				.WithMalId(malId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			var command = new UpdateCharacterMessage
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
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacter(malId), Times.Once);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenCorrectResponse_ShouldUpdateBasicProperties()
		{
			// Given
			const int malId = 1;

			const string returnedName = "PostUpdateName";
			const string returnedAbout = "PostUpdateAbout";
			const string returnedJapaneseName = "PostUpdateJapanese";
			const string returnedImageUrl = "PostUpdateImageUrl";
			const int returnedPopularity = 1;

			var returnedCharacter = new Character
			{
				Name = returnedName,
				About = returnedAbout,
				NameKanji = returnedJapaneseName,
				ImageURL = returnedImageUrl,
				Nicknames = new List<string>(),
				MemberFavorites = returnedPopularity
			};

			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder().WithCharacterReturned(returnedCharacter);

			var character = new CharacterBuilder()
				.WithMalId(malId)
				.WithName("PreUpdateName")
				.WithAbout("PreUpdateAbout")
				.WithKanjiName("PreUpdateJapanese")
				.WithImageUrl("PreUpdateImageUrl")
				.WithPopularity(0)
				.Build();

			await dbContext.AddAsync(character);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			var command = new UpdateCharacterMessage
			{
				Id = Guid.NewGuid(),
				MalId = malId
			};

			// When
			await handler.HandleAsync(command);

			// Then
			var updatedCharacter = await dbContext.AnimeCharacters.FirstAsync(x => x.MalId == malId);
			using (new AssertionScope())
			{
				updatedCharacter.Name.Should().Be(returnedName);
				updatedCharacter.About.Should().Be(returnedAbout);
				updatedCharacter.KanjiName.Should().Be(returnedJapaneseName);
				updatedCharacter.ImageUrl.Should().Be(returnedImageUrl);
				updatedCharacter.Popularity.Should().Be(returnedPopularity);

				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacter(malId), Times.Once);
			}
		}

		[Theory]
		[InlineData("")]
		[InlineData(null)]
		[InlineData("\t\t  \n \t   ")]
		[InlineData("https://cdn.myanimelist.net/images/questionmark_23.gif")]
		[InlineData("https://cdn.myanimelist.net/img/sp/icon/apple-touch-icon-256.png")]
		public async Task HandleAsync_GivenPlaceholderOrEmptyImageUrl_ShouldUpdateWithEmpty(string imageUrl)
		{
			// Given
			const int malId = 1;

			var returnedCharacter = new Character
			{
				Name = imageUrl
			};

			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder().WithCharacterReturned(returnedCharacter);

			var character = new CharacterBuilder()
				.WithMalId(malId)
				.WithName("PreUpdateName")
				.WithAbout("PreUpdateAbout")
				.WithKanjiName("PreUpdateJapanese")
				.WithImageUrl("PreUpdateImageUrl")
				.WithPopularity(0)
				.Build();

			await dbContext.AddAsync(character);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			var command = new UpdateCharacterMessage
			{
				Id = Guid.NewGuid(),
				MalId = malId
			};

			// When
			await handler.HandleAsync(command);

			// Then
			var updatedCharacter = await dbContext.AnimeCharacters.FirstAsync(x => x.MalId == malId);
			using (new AssertionScope())
			{
				updatedCharacter.ImageUrl.Should().BeEmpty();

				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacter(malId), Times.Once);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenManyNicknames_ShouldUpdateWithJoinedString()
		{
			// Given
			const int malId = 1;

			var nicknames = new List<string> { "Nickname 1", "Nickname 2", "Nickname 3" };

			var returnedCharacter = new Character
			{
				Nicknames = nicknames
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithCharacterReturned(returnedCharacter);

			var dbContext = InMemoryDbProvider.GetDbContext();
			var character = new CharacterBuilder()
				.WithMalId(malId)
				.WithName("PreUpdateName")
				.WithAbout("PreUpdateAbout")
				.WithKanjiName("PreUpdateJapanese")
				.WithImageUrl("PreUpdateImageUrl")
				.WithPopularity(0)
				.Build();

			await dbContext.AddAsync(character);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			var command = new UpdateCharacterMessage
			{
				Id = Guid.NewGuid(),
				MalId = malId
			};

			// When
			await handler.HandleAsync(command);

			// Then
			var updatedCharacter = await dbContext.AnimeCharacters.FirstAsync(x => x.MalId == malId);
			using (new AssertionScope())
			{
				updatedCharacter.Nicknames.Should().Be("Nickname 1;Nickname 2;Nickname 3");

				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacter(malId), Times.Once);
			}
		}

		private UpdateCharacterHandler CreateHandler(SeiyuuMoeContext dbcontext, IMalApiService apiService)
		{
			var characterRepository = new CharacterRepository(dbcontext);

			return new UpdateCharacterHandler(characterRepository, apiService);
		}
	}
}
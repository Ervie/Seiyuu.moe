using FluentAssertions;
using FluentAssertions.Execution;
using Tenrai;
using Tenrai.Exceptions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using SeiyuuMoe.Domain.Services;
using SeiyuuMoe.Domain.SqsMessages;
using SeiyuuMoe.Infrastructure.Database.Animes;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Database.Seasons;
using SeiyuuMoe.MalBackgroundJobs.Application.Handlers;
using SeiyuuMoe.Tests.Common.Builders.Tenrai;
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
		public async Task HandleAsync_GivenEmptyDatabase_ShouldNotThrowAndNotCallTenrai()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var tenraiServiceBuilder = new TenraiServiceBuilder();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build());

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
				await tenraiServiceBuilder.TenraiClient.DidNotReceive().GetAnimeAsync(Arg.Any<long>());
			}
		}

		[Fact]
		public async Task HandleAsync_GivenNullResponseFromTenrai_ShouldNotThrowAndCallTenrai()
		{
			// Given
			const int malId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var tenraiServiceBuilder = new TenraiServiceBuilder().WithAnimeReturned(null);

			var anime = new AnimeBuilder()
				.WithMalId(malId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build());

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
				await tenraiServiceBuilder.TenraiClient.Received(1).GetAnimeAsync(malId);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenExceptionFromTenrai_ShouldThrowAndCallTenrai()
		{
			// Given
			const int malId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var tenraiServiceBuilder = new TenraiServiceBuilder().WithGetAnimeThrowing();

			var anime = new AnimeBuilder()
				.WithMalId(malId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build());

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
				await action.Should().ThrowExactlyAsync<TenraiRequestException>();
				await tenraiServiceBuilder.TenraiClient.Received(1).GetAnimeAsync(malId);
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
				Synopsis = returnedAbout,
				Titles = AnimeTitlesBuilder.Build(returnedTitle, returnedEnglishTitle, returnedJapaneseTitle),
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = returnedImageUrl }
				},
				Members = returnedPopularity
			};

			var tenraiServiceBuilder = new TenraiServiceBuilder().WithAnimeReturned(returnedAnime);

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

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build());

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
				updatedAnime.Title.Should().Be(returnedTitle);
				updatedAnime.About.Should().Be(returnedAbout);
				updatedAnime.EnglishTitle.Should().Be(returnedEnglishTitle);
				updatedAnime.KanjiTitle.Should().Be(returnedJapaneseTitle);
				updatedAnime.ImageUrl.Should().Be(returnedImageUrl);
				updatedAnime.Popularity.Should().Be(returnedPopularity);

				await tenraiServiceBuilder.TenraiClient.Received(1).GetAnimeAsync(malId);
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

			var returnedAnime = new Anime
			{
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = imageUrl }
				}
			};

			var tenraiServiceBuilder = new TenraiServiceBuilder().WithAnimeReturned(returnedAnime);

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

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build());

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
				updatedAnime.ImageUrl.Should().BeEmpty();

				await tenraiServiceBuilder.TenraiClient.Received(1).GetAnimeAsync(malId);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenManyTitleSynonyms_ShouldUpdateWithJoinedString()
		{
			// Given
			const int malId = 1;

			var returnedSynonyms = new List<string> { "Synonym 1", "Synonym 2", "Synonym 3" };

			var returnedAnime = new Anime
			{
				Titles = AnimeTitlesBuilder.Build(titleSynonyms: returnedSynonyms)
			};

			var tenraiServiceBuilder = new TenraiServiceBuilder().WithAnimeReturned(returnedAnime);

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

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build());

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
				updatedAnime.TitleSynonyms.Should().Be("Synonym 1;Synonym 2;Synonym 3");

				await tenraiServiceBuilder.TenraiClient.Received(1).GetAnimeAsync(malId);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenUnknownStatusAndType_ShouldNotUpdateStatusAndType()
		{
			// Given
			const int malId = 1;

			var returnedAnime = new Anime
			{
				Type = "test type",
				Status = "test status"
			};

			var tenraiServiceBuilder = new TenraiServiceBuilder().WithAnimeReturned(returnedAnime);

			var dbContext = InMemoryDbProvider.GetDbContext();
			var anime = new AnimeBuilder()
				.WithMalId(malId)
				.WithTitle("PreUpdateTitle")
				.WithAbout("PreUpdateAbout")
				.WithEnglishTitle("PreUpdateEnglish")
				.WithJapaneseTitle("PreUpdateJapaneses")
				.WithImageUrl("PreUpdateImage")
				.WithAnimeStatus(x => x.WithId(Domain.Entities.AnimeStatusId.CurrentlyAiring))
				.WithAnimeType(x => x.WithId(Domain.Entities.AnimeTypeId.TV))
				.WithPopularity(0)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build());

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
				updatedAnime.TypeId.Should().Be(Domain.Entities.AnimeTypeId.TV);
				updatedAnime.StatusId.Should().Be(Domain.Entities.AnimeStatusId.CurrentlyAiring);

				await tenraiServiceBuilder.TenraiClient.Received(1).GetAnimeAsync(malId);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenCorrectStatusAndType_ShouldUpdateWithStatusAndType()
		{
			// Given
			const int malId = 1;

			var returnedAnime = new Anime
			{
				Type = "Movie",
				Status = "Not yet aired"
			};

			var tenraiServiceBuilder = new TenraiServiceBuilder().WithAnimeReturned(returnedAnime);

			var dbContext = InMemoryDbProvider.GetDbContext();
			var anime = new AnimeBuilder()
				.WithMalId(malId)
				.WithTitle("PreUpdateTitle")
				.WithAbout("PreUpdateAbout")
				.WithEnglishTitle("PreUpdateEnglish")
				.WithJapaneseTitle("PreUpdateJapaneses")
				.WithImageUrl("PreUpdateImage")
				.WithAnimeStatus(x => x.WithName("Airing"))
				.WithAnimeType(x => x.WithId(Domain.Entities.AnimeTypeId.TV))
				.WithPopularity(0)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build());

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
				updatedAnime.TypeId.Should().Be(Domain.Entities.AnimeTypeId.Movie);
				updatedAnime.StatusId.Should().Be(Domain.Entities.AnimeStatusId.Notyetaired);

				await tenraiServiceBuilder.TenraiClient.Received(1).GetAnimeAsync(malId);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSeasonNotInDB_ShouldNotUpdateSeasonId()
		{
			// Given
			const int malId = 1;

			var returnedAnime = new Anime
			{
				Season = Season.Winter,
				Year = 2001
			};

			var tenraiServiceBuilder = new TenraiServiceBuilder().WithAnimeReturned(returnedAnime);

			var dbContext = InMemoryDbProvider.GetDbContext();
			var anime = new AnimeBuilder()
				.WithMalId(malId)
				.WithTitle("PreUpdateTitle")
				.WithAbout("PreUpdateAbout")
				.WithEnglishTitle("PreUpdateEnglish")
				.WithJapaneseTitle("PreUpdateJapaneses")
				.WithImageUrl("PreUpdateImage")
				.WithAnimeStatus(x => x.WithName("Airing"))
				.WithAnimeType(x => x.WithId(Domain.Entities.AnimeTypeId.TV))
				.WithPopularity(0)
				.Build();

			var season = new SeasonBuilder()
				.WithId(10)
				.WithYear(2000)
				.WithName("Winter")
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.AddAsync(season);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build());

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
				updatedAnime.SeasonId.Should().BeNull();

				await tenraiServiceBuilder.TenraiClient.Received(1).GetAnimeAsync(malId);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenCorrectSeason_ShouldUpdateSeasonId()
		{
			// Given
			const int malId = 1;

			var returnedAnime = new Anime
			{
				Season = Season.Winter,
				Year = 2000
			};

			var tenraiServiceBuilder = new TenraiServiceBuilder().WithAnimeReturned(returnedAnime);

			var dbContext = InMemoryDbProvider.GetDbContext();
			var anime = new AnimeBuilder()
				.WithMalId(malId)
				.WithTitle("PreUpdateTitle")
				.WithAbout("PreUpdateAbout")
				.WithEnglishTitle("PreUpdateEnglish")
				.WithJapaneseTitle("PreUpdateJapaneses")
				.WithImageUrl("PreUpdateImage")
				.WithAnimeStatus(x => x.WithName("Airing"))
				.WithAnimeType(x => x.WithId(Domain.Entities.AnimeTypeId.TV))
				.WithPopularity(0)
				.Build();

			var season = new SeasonBuilder()
				.WithId(10)
				.WithYear(2000)
				.WithName("Winter")
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.AddAsync(season);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build());

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
				updatedAnime.SeasonId.Should().Be(10);

				await tenraiServiceBuilder.TenraiClient.Received(1).GetAnimeAsync(malId);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenBothCorrectSeasonAndAiringDate_ShouldUpdateSeasonIdBySeason()
		{
			// Given
			const int malId = 1;

			var returnedAnime = new Anime
			{
				Season = Season.Winter,
				Year = 2000,
				Aired = new TimePeriod
				{
					From = new DateTime(2001, 1, 1)
				}
			};

			var tenraiServiceBuilder = new TenraiServiceBuilder().WithAnimeReturned(returnedAnime);

			var dbContext = InMemoryDbProvider.GetDbContext();
			var anime = new AnimeBuilder()
				.WithMalId(malId)
				.WithTitle("PreUpdateTitle")
				.WithAbout("PreUpdateAbout")
				.WithEnglishTitle("PreUpdateEnglish")
				.WithJapaneseTitle("PreUpdateJapaneses")
				.WithImageUrl("PreUpdateImage")
				.WithAnimeStatus(x => x.WithName("Airing"))
				.WithAnimeType(x => x.WithId(Domain.Entities.AnimeTypeId.TV))
				.WithPopularity(0)
				.Build();

			var seasons = new List<Domain.Entities.AnimeSeason>
			{
				new SeasonBuilder().WithId(10).WithYear(2000).WithName("Winter").Build(),
				new SeasonBuilder().WithId(11).WithYear(2000).WithName("Spring").Build(),
				new SeasonBuilder().WithId(12).WithYear(2001).WithName("Winter").Build(),
			};

			await dbContext.AddAsync(anime);
			await dbContext.AddRangeAsync(seasons);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build());

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
				updatedAnime.SeasonId.Should().Be(10);

				await tenraiServiceBuilder.TenraiClient.Received(1).GetAnimeAsync(malId);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenBothOnlyAiringDateWithoutMatchingSeasonInDB_ShouldNotUpdateSeasonId()
		{
			// Given
			const int malId = 1;

			var returnedAnime = new Anime
			{
				Season = null,
				Aired = new TimePeriod
				{
					From = new DateTime(2001, 1, 1)
				}
			};

			var tenraiServiceBuilder = new TenraiServiceBuilder().WithAnimeReturned(returnedAnime);

			var dbContext = InMemoryDbProvider.GetDbContext();
			var anime = new AnimeBuilder()
				.WithMalId(malId)
				.WithTitle("PreUpdateTitle")
				.WithAbout("PreUpdateAbout")
				.WithEnglishTitle("PreUpdateEnglish")
				.WithJapaneseTitle("PreUpdateJapaneses")
				.WithImageUrl("PreUpdateImage")
				.WithAnimeStatus(x => x.WithName("Airing"))
				.WithAnimeType(x => x.WithId(Domain.Entities.AnimeTypeId.TV))
				.WithPopularity(0)
				.Build();

			var seasons = new List<Domain.Entities.AnimeSeason>
			{
				new SeasonBuilder().WithId(10).WithYear(2000).WithName("Winter").Build(),
				new SeasonBuilder().WithId(11).WithYear(2000).WithName("Spring").Build(),
				new SeasonBuilder().WithId(12).WithYear(2001).WithName("Winter").Build(),
			};

			await dbContext.AddAsync(anime);
			await dbContext.AddRangeAsync(seasons);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build());

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
				updatedAnime.SeasonId.Should().Be(12);

				await tenraiServiceBuilder.TenraiClient.Received(1).GetAnimeAsync(malId);
			}
		}

		public static IEnumerable<object[]> SeasonData =>
		new List<object[]>
		{
			new object[] { new DateTime(2000, 1, 1), 10 },
			new object[] { new DateTime(2000, 2, 1), 10 },
			new object[] { new DateTime(2000, 3, 14), 10 },
			new object[] { new DateTime(2000, 3, 29), 11 },
			new object[] { new DateTime(2000, 4, 1), 11 },
			new object[] { new DateTime(2000, 6, 10), 11 },
			new object[] { new DateTime(2000, 6, 28), 12 },
			new object[] { new DateTime(2000, 7, 10), 12 },
			new object[] { new DateTime(2000, 9, 1), 12 },
			new object[] { new DateTime(2000, 9, 25), 13 },
			new object[] { new DateTime(2000, 9, 30), 13 },
			new object[] { new DateTime(2000, 10, 1), 13 },
			new object[] { new DateTime(2000, 12, 5), 13 },
			new object[] { new DateTime(2000, 12, 24), 14 },
		};

		[Theory]
		[MemberData(nameof(SeasonData))]
		public async Task HandleAsync_GivenBothOnlyAiringDateWithMatchingSeasonInDB_ShouldUpdateSeasonId(DateTime fromAiringDate, long expectedSeasonId)
		{
			// Given
			const int malId = 1;

			var returnedAnime = new Anime
			{
				Season = null,
				Aired = new TimePeriod
				{
					From = fromAiringDate
				}
			};

			var tenraiServiceBuilder = new TenraiServiceBuilder().WithAnimeReturned(returnedAnime);

			var dbContext = InMemoryDbProvider.GetDbContext();
			var anime = new AnimeBuilder()
				.WithMalId(malId)
				.WithTitle("PreUpdateTitle")
				.WithAbout("PreUpdateAbout")
				.WithEnglishTitle("PreUpdateEnglish")
				.WithJapaneseTitle("PreUpdateJapaneses")
				.WithImageUrl("PreUpdateImage")
				.WithAnimeStatus(x => x.WithName("Airing"))
				.WithAnimeType(x => x.WithId(Domain.Entities.AnimeTypeId.TV))
				.WithPopularity(0)
				.Build();

			var seasons = new List<Domain.Entities.AnimeSeason>
			{
				new SeasonBuilder().WithId(10).WithYear(2000).WithName("Winter").Build(),
				new SeasonBuilder().WithId(11).WithYear(2000).WithName("Spring").Build(),
				new SeasonBuilder().WithId(12).WithYear(2000).WithName("Summer").Build(),
				new SeasonBuilder().WithId(13).WithYear(2000).WithName("Fall").Build(),
				new SeasonBuilder().WithId(14).WithYear(2001).WithName("Winter").Build(),
			};

			await dbContext.AddAsync(anime);
			await dbContext.AddRangeAsync(seasons);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build());

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
				updatedAnime.SeasonId.Should().Be(expectedSeasonId);

				await tenraiServiceBuilder.TenraiClient.Received(1).GetAnimeAsync(malId);
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
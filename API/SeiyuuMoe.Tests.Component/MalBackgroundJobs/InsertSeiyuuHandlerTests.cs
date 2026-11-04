using FluentAssertions;
using FluentAssertions.Execution;
using Tenrai;
using Tenrai.Exceptions;
using NSubstitute;
using SeiyuuMoe.Domain.S3;
using SeiyuuMoe.Infrastructure.Database.Animes;
using SeiyuuMoe.Infrastructure.Database.Characters;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Database.Seasons;
using SeiyuuMoe.Infrastructure.Database.Seiyuus;
using SeiyuuMoe.Infrastructure.Tenrai;
using SeiyuuMoe.MalBackgroundJobs.Application.Handlers;
using SeiyuuMoe.Tests.Common.Builders.Tenrai;
using SeiyuuMoe.Tests.Common.Builders.Model;
using SeiyuuMoe.Tests.Common.Helpers;
using SeiyuuMoe.Tests.Common.Stubs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Component.MalBackgroundJobs
{
	public class InsertSeiyuuHandlerTests
	{
		[Fact]
		public async Task HandleAsync_GivenEmptyDatabaseAndBatchSizeOne_ShouldCallTenraiOnceWithNextId()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var tenraiServiceBuilder = new TenraiServiceBuilder();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build(), new S3ServiceStub(1), 1);

			// When
			await handler.HandleAsync();

			// Then
			await tenraiServiceBuilder.TenraiClient.Received(1).GetPersonFullDataAsync(2);
		}

		[Fact]
		public async Task HandleAsync_GivenEmptyDatabaseAndBatchSizeOneAndBiggerId_ShouldCallTenraiOnceWithNextId()
		{
			// Given
			var lastId = 10000;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var tenraiServiceBuilder = new TenraiServiceBuilder();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build(), new S3ServiceStub(lastId), 1);

			// When
			await handler.HandleAsync();

			// Then
			await tenraiServiceBuilder.TenraiClient.Received(1).GetPersonFullDataAsync(lastId + 1);
		}

		[Fact]
		public async Task HandleAsync_GivenEmptyDatabaseAndBatchSizeFive_ShouldCallTenraiFiveTimesWithNextIds()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var tenraiServiceBuilder = new TenraiServiceBuilder();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build(), new S3ServiceStub(1), 5);

			// When
			await handler.HandleAsync();

			// Then
			using (new AssertionScope())
			{
				await tenraiServiceBuilder.TenraiClient.Received(1).GetPersonFullDataAsync(2);
				await tenraiServiceBuilder.TenraiClient.Received(1).GetPersonFullDataAsync(3);
				await tenraiServiceBuilder.TenraiClient.Received(1).GetPersonFullDataAsync(4);
				await tenraiServiceBuilder.TenraiClient.Received(1).GetPersonFullDataAsync(5);
				await tenraiServiceBuilder.TenraiClient.Received(1).GetPersonFullDataAsync(6);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenEmptyDatabaseAndBatchSizeOne_LastInsertedSeiyuuIdShouldBeBiggerByOne()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var tenraiServiceBuilder = new TenraiServiceBuilder();
			var s3Service = new S3ServiceStub(1);

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build(), s3Service, 1);

			// When
			await handler.HandleAsync();

			// Then
			var state = await s3Service.GetBgJobsStateAsync(null);
			state.LastCheckedSeiyuuMalId.Should().Be(2);
		}

		[Fact]
		public async Task HandleAsync_GivenEmptyDatabaseAndBatchSizeFive_LastInsertedSeiyuuIdShouldBeBiggerByFive()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var tenraiServiceBuilder = new TenraiServiceBuilder();
			var s3Service = new S3ServiceStub(1);

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build(), s3Service, 5);

			// When
			await handler.HandleAsync();

			// Then
			var state = await s3Service.GetBgJobsStateAsync(null);
			state.LastCheckedSeiyuuMalId.Should().Be(6);
		}

		[Fact]
		public async Task HandleAsync_GivenExistingSeiyuu_ShouldSkipAndNotCallTenrai()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var tenraiServiceBuilder = new TenraiServiceBuilder();
			const int startingMalId = 1;
			var s3Service = new S3ServiceStub(startingMalId);

			var anime = new SeiyuuBuilder()
				.WithMalId(startingMalId + 1)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build(), s3Service, 5);

			// When
			await handler.HandleAsync();

			// Then
			await tenraiServiceBuilder.TenraiClient.DidNotReceive().GetPersonFullDataAsync(startingMalId + 1);
			dbContext.Seiyuus.Should().ContainSingle();
		}

		[Fact]
		public async Task HandleAsync_GivenNotExistingSeiyuuExceptionFromTenrai_ShouldSkipAndNotInsert()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var tenraiServiceBuilder = new TenraiServiceBuilder().WithGetPersonThrowing();
			const int startingMalId = 1;
			var s3Service = new S3ServiceStub(startingMalId);

			var anime = new SeiyuuBuilder()
				.WithMalId(startingMalId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build(), s3Service, 1);

			// When
			var action = handler.Awaiting(x => x.HandleAsync());

			// Then
			using (new AssertionScope())
			{
				await action.Should().ThrowExactlyAsync<TenraiRequestException>();
				dbContext.Seiyuus.Should().ContainSingle();
			}
			await tenraiServiceBuilder.TenraiClient.Received(1).GetPersonFullDataAsync(startingMalId + 1);
		}

		[Fact]
		public async Task HandleAsync_GivenNotExistingSeiyuuNullFromTenrai_ShouldSkipAndNotInsert()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var tenraiServiceBuilder = new TenraiServiceBuilder().WithPersonReturned(null, null);
			const int startingMalId = 1;
			var s3Service = new S3ServiceStub(startingMalId);

			var anime = new SeiyuuBuilder()
				.WithMalId(startingMalId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build(), s3Service, 1);

			// When
			await handler.HandleAsync();

			// Then
			await tenraiServiceBuilder.TenraiClient.Received(1).GetPersonFullDataAsync(startingMalId + 1);
			dbContext.Seiyuus.Should().ContainSingle();
		}

		[Fact]
		public async Task HandleAsync_GivenNotExistingJapanesePersonWithoutRoles_ShouldSkipAndNotInsert()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			const int startingMalId = 1;
			var s3Service = new S3ServiceStub(startingMalId);

			const string returnedSeiyuuName = "PostUpdateSeiyuuName";
			const string returnedSeiyuuAbout = "PostUpdateSeiyuuAbout";
			const string returnedSeiyuuGivenName = "花澤";
			const string returnedSeiyuuFamilyNameName = "香菜";
			const string returnedSeiyuuImageUrl = "PostUpdateSeiyuuImageUrl";
			const int returnedSeiyuuPopularity = 1;
			var returnedBirthdate = new DateTime(1990, 1, 1);

			var returnedSeiyuu = new Person
			{
				Name = returnedSeiyuuName,
				About = returnedSeiyuuAbout,
				GivenName = returnedSeiyuuGivenName,
				FamilyName = returnedSeiyuuFamilyNameName,
				Images =new ImagesSet
				{
					JPG = new Image { ImageUrl = returnedSeiyuuImageUrl }
				},
				MemberFavorites = returnedSeiyuuPopularity,
				Birthday = returnedBirthdate
			};

			var tenraiServiceBuilder = new TenraiServiceBuilder().WithPersonReturned(returnedSeiyuu, null);

			var anime = new SeiyuuBuilder()
				.WithMalId(startingMalId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build(), s3Service, 1);

			// When
			await handler.HandleAsync();

			// Then
			await tenraiServiceBuilder.TenraiClient.Received(1).GetPersonFullDataAsync(startingMalId + 1);
			dbContext.Seiyuus.Should().ContainSingle();
		}

		[Fact]
		public async Task HandleAsync_GivenNotExistingNonJapanesePersonWithoutRoles_ShouldSkipAndNotInsert()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			const int startingMalId = 1;
			var s3Service = new S3ServiceStub(startingMalId);

			const string returnedSeiyuuName = "PostUpdateSeiyuuName";
			const string returnedSeiyuuAbout = "PostUpdateSeiyuuAbout";
			const string returnedSeiyuuGivenName = "John";
			const string returnedSeiyuuFamilyNameName = "Doe";
			const string returnedSeiyuuImageUrl = "PostUpdateSeiyuuImageUrl";
			const int returnedSeiyuuPopularity = 1;
			var returnedBirthdate = new DateTime(1990, 1, 1);

			var returnedSeiyuu = new Person
			{
				Name = returnedSeiyuuName,
				About = returnedSeiyuuAbout,
				GivenName = returnedSeiyuuGivenName,
				FamilyName = returnedSeiyuuFamilyNameName,
				Images =new ImagesSet
				{
					JPG = new Image { ImageUrl = returnedSeiyuuImageUrl }
				},
				MemberFavorites = returnedSeiyuuPopularity,
				Birthday = returnedBirthdate
			};

			var tenraiServiceBuilder = new TenraiServiceBuilder().WithPersonReturned(returnedSeiyuu, null);

			var anime = new SeiyuuBuilder()
				.WithMalId(startingMalId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build(), s3Service, 5);

			// When
			await handler.HandleAsync();

			// Then
			await tenraiServiceBuilder.TenraiClient.Received(1).GetPersonFullDataAsync(startingMalId + 1);
			dbContext.Seiyuus.Should().ContainSingle();
		}

		[Fact]
		public async Task HandleAsync_GivenNotExistingNonJapanesePersonWithRoles_ShouldSkipAndNotInsert()
		{
			// Given
			const long animeMalId = 100;
			const long characterMalId = 1000;
			var dbContext = InMemoryDbProvider.GetDbContext();
			const int startingSeiyuuMalId = 1;
			var s3Service = new S3ServiceStub(startingSeiyuuMalId);

			const string returnedSeiyuuName = "PostUpdateSeiyuuName";
			const string returnedSeiyuuAbout = "PostUpdateSeiyuuAbout";
			const string returnedSeiyuuGivenName = "John";
			const string returnedSeiyuuFamilyNameName = "Doe";
			const string returnedSeiyuuImageUrl = "PostUpdateSeiyuuImageUrl";
			const int returnedSeiyuuPopularity = 1;
			var returnedBirthdate = new DateTime(1990, 1, 1);

			var returnedSeiyuu = new Person
			{
				Name = returnedSeiyuuName,
				About = returnedSeiyuuAbout,
				GivenName = returnedSeiyuuGivenName,
				FamilyName = returnedSeiyuuFamilyNameName,
				Images =new ImagesSet
				{
					JPG = new Image { ImageUrl = returnedSeiyuuImageUrl }
				},
				MemberFavorites = returnedSeiyuuPopularity,
				Birthday = returnedBirthdate
			};
			var returnedRoles = new List<VoiceActingRole>
			{
				new VoiceActingRole
				{
					Role = "Main",
					Anime = new MalImageSubItem
					{
						MalId = animeMalId
					},
					Character = new MalImageSubItem()
					{
						MalId = characterMalId
					}
				}
			};

			var tenraiServiceBuilder = new TenraiServiceBuilder().WithPersonReturned(returnedSeiyuu, returnedRoles);

			var anime = new SeiyuuBuilder()
				.WithMalId(startingSeiyuuMalId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build(), s3Service, 5);

			// When
			await handler.HandleAsync();

			// Then
			await tenraiServiceBuilder.TenraiClient.Received(1).GetPersonFullDataAsync(startingSeiyuuMalId + 1);
			dbContext.Seiyuus.Should().ContainSingle();
		}

		[Fact]
		public async Task HandleAsync_GivenNotExistingJapanesePersonWithRolesAndNullResponsesFromCharacterAndAnimeRequests_ShouldInsertNewSeiyuuWithoutroles()
		{
			// Given
			const long animeMalId = 100;
			const long characterMalId = 1000;
			var dbContext = InMemoryDbProvider.GetDbContext();
			const int startingSeiyuuMalId = 1;
			var s3Service = new S3ServiceStub(startingSeiyuuMalId);

			const string returnedSeiyuuName = "PostUpdateSeiyuuName";
			const string returnedSeiyuuAbout = "PostUpdateSeiyuuAbout";
			const string returnedSeiyuuGivenName = "花澤";
			const string returnedSeiyuuFamilyNameName = "香菜";
			const string returnedSeiyuuImageUrl = "PostUpdateSeiyuuImageUrl";
			const int returnedSeiyuuPopularity = 1;
			var returnedBirthdate = new DateTime(1990, 1, 1);

			var returnedSeiyuu = new Person
			{
				Name = returnedSeiyuuName,
				About = returnedSeiyuuAbout,
				GivenName = returnedSeiyuuGivenName,
				FamilyName = returnedSeiyuuFamilyNameName,
				Images =new ImagesSet
				{
					JPG = new Image { ImageUrl = returnedSeiyuuImageUrl }
				},
				MemberFavorites = returnedSeiyuuPopularity,
				Birthday = returnedBirthdate
			};
			var returnedRoles = new List<VoiceActingRole>
			{
				new VoiceActingRole
				{
					Role = "Main",
					Anime = new MalImageSubItem
					{
						MalId = animeMalId
					},
					Character = new MalImageSubItem()
					{
						MalId = characterMalId
					}
				}
			};

			var tenraiServiceBuilder = new TenraiServiceBuilder()
				.WithPersonReturned(returnedSeiyuu, returnedRoles)
				.WithCharacterReturned(null)
				.WithAnimeReturned(null);

			var anime = new SeiyuuBuilder()
				.WithMalId(startingSeiyuuMalId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build(), s3Service, 1);

			// When
			await handler.HandleAsync();

			// Then
			await tenraiServiceBuilder.TenraiClient.Received(1).GetPersonFullDataAsync(startingSeiyuuMalId + 1);
			await tenraiServiceBuilder.TenraiClient.Received(1).GetAnimeAsync(animeMalId);
			await tenraiServiceBuilder.TenraiClient.Received(1).GetCharacterAsync(characterMalId);
			dbContext.Seiyuus.Should().HaveCount(2);
			dbContext.AnimeRoles.Should().BeEmpty();
		}

		[Fact]
		public async Task HandleAsync_GivenNotExistingJapanesePersonWithRolesAndNullResponsesFromCharacterRequest_ShouldInsertNewSeiyuuWithoutroles()
		{
			// Given
			const long animeMalId = 100;
			const long characterMalId = 1000;
			var dbContext = InMemoryDbProvider.GetDbContext();
			const int startingSeiyuuMalId = 1;
			var s3Service = new S3ServiceStub(startingSeiyuuMalId);

			const string returnedSeiyuuName = "PostUpdateSeiyuuName";
			const string returnedSeiyuuAbout = "PostUpdateSeiyuuAbout";
			const string returnedSeiyuuGivenName = "花澤";
			const string returnedSeiyuuFamilyNameName = "香菜";
			const string returnedSeiyuuImageUrl = "PostUpdateSeiyuuImageUrl";
			const int returnedSeiyuuPopularity = 1;
			var returnedBirthdate = new DateTime(1990, 1, 1);

			const string returnedAnimeTitle = "PostUpdateTitle";
			const string returnedAnimeAbout = "PostUpdateAbout";
			const string returnedAnimeEnglishTitle = "PostUpdateEnglish";
			const string returnedAnimeJapaneseTitle = "PostUpdateJapanese";
			const string returnedAnimeImageUrl = "PostUpdateImageUrl";
			const int returnedAnimePopularity = 3;

			var returnedSeiyuu = new Person
			{
				Name = returnedSeiyuuName,
				About = returnedSeiyuuAbout,
				GivenName = returnedSeiyuuGivenName,
				FamilyName = returnedSeiyuuFamilyNameName,
				Images =new ImagesSet
				{
					JPG = new Image { ImageUrl = returnedSeiyuuImageUrl }
				},
				MemberFavorites = returnedSeiyuuPopularity,
				Birthday = returnedBirthdate
			};
			var returnedRoles = new List<VoiceActingRole>
			{
				new VoiceActingRole
				{
					Role = "Main",
					Anime = new MalImageSubItem
					{
						MalId = animeMalId
					},
					Character = new MalImageSubItem()
					{
						MalId = characterMalId
					}
				}
			};

			var returnedAnime = new Anime
			{
				MalId = animeMalId,
				Synopsis = returnedAnimeAbout,
				Titles = AnimeTitlesBuilder.Build(returnedAnimeTitle, returnedAnimeEnglishTitle, returnedAnimeJapaneseTitle),
				Images =new ImagesSet
				{
					JPG = new Image { ImageUrl = returnedAnimeImageUrl }
				},
				Members = returnedAnimePopularity
			};

			var tenraiServiceBuilder = new TenraiServiceBuilder()
				.WithPersonReturned(returnedSeiyuu, returnedRoles)
				.WithCharacterReturned(null)
				.WithAnimeReturned(returnedAnime);

			var anime = new SeiyuuBuilder()
				.WithMalId(startingSeiyuuMalId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build(), s3Service, 1);

			// When
			await handler.HandleAsync();

			// Then
			await tenraiServiceBuilder.TenraiClient.Received(1).GetPersonFullDataAsync(startingSeiyuuMalId + 1);
			await tenraiServiceBuilder.TenraiClient.Received(1).GetAnimeAsync(animeMalId);
			await tenraiServiceBuilder.TenraiClient.Received(1).GetCharacterAsync(characterMalId);
			dbContext.Seiyuus.Should().HaveCount(2);
			dbContext.Animes.Should().ContainSingle();
			dbContext.AnimeCharacters.Should().BeEmpty();
			dbContext.AnimeRoles.Should().BeEmpty();
		}

		[Fact]
		public async Task HandleAsync_GivenNotExistingJapanesePersonWithRolesAndNullResponsesFromAnimeRequest_ShouldInsertNewSeiyuuWithoutroles()
		{
			// Given
			const long animeMalId = 100;
			const long characterMalId = 1000;
			var dbContext = InMemoryDbProvider.GetDbContext();
			const int startingSeiyuuMalId = 1;
			var s3Service = new S3ServiceStub(startingSeiyuuMalId);

			const string returnedSeiyuuName = "PostUpdateSeiyuuName";
			const string returnedSeiyuuAbout = "PostUpdateSeiyuuAbout";
			const string returnedSeiyuuGivenName = "花澤";
			const string returnedSeiyuuFamilyNameName = "香菜";
			const string returnedSeiyuuImageUrl = "PostUpdateSeiyuuImageUrl";
			const int returnedSeiyuuPopularity = 1;
			var returnedBirthdate = new DateTime(1990, 1, 1);

			const string returnedCharacterName = "PostUpdateName";
			const string returnedCharacterAbout = "PostUpdateAbout";
			const string returnedCharacterJapaneseName = "PostUpdateJapanese";
			const string returnedCharacterImageUrl = "PostUpdateImageUrl";
			const int returnedCharacterPopularity = 1;

			var returnedSeiyuu = new Person
			{
				Name = returnedSeiyuuName,
				About = returnedSeiyuuAbout,
				GivenName = returnedSeiyuuGivenName,
				FamilyName = returnedSeiyuuFamilyNameName,
				Images =new ImagesSet
				{
					JPG = new Image { ImageUrl = returnedSeiyuuImageUrl }
				},
				MemberFavorites = returnedSeiyuuPopularity,
				Birthday = returnedBirthdate
			};
			var returnedRoles = new List<VoiceActingRole>
			{
				new VoiceActingRole
				{
					Role = "Main",
					Anime = new MalImageSubItem
					{
						MalId = animeMalId
					},
					Character = new MalImageSubItem()
					{
						MalId = characterMalId
					}
				}
			};

			var returnedCharacter = new Character
			{
				MalId = characterMalId,
				Name = returnedCharacterName,
				About = returnedCharacterAbout,
				NameKanji = returnedCharacterJapaneseName,
				Images =new ImagesSet
				{
					JPG = new Image { ImageUrl = returnedCharacterImageUrl }
				},
				Nicknames = new List<string>(),
				Favorites = returnedCharacterPopularity
			};

			var tenraiServiceBuilder = new TenraiServiceBuilder()
				.WithPersonReturned(returnedSeiyuu, returnedRoles)
				.WithCharacterReturned(returnedCharacter)
				.WithAnimeReturned(null);

			var anime = new SeiyuuBuilder()
				.WithMalId(startingSeiyuuMalId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build(), s3Service, 1);

			// When
			await handler.HandleAsync();

			// Then
			await tenraiServiceBuilder.TenraiClient.Received(1).GetPersonFullDataAsync(startingSeiyuuMalId + 1);
			await tenraiServiceBuilder.TenraiClient.Received(1).GetAnimeAsync(animeMalId);
			await tenraiServiceBuilder.TenraiClient.Received(1).GetCharacterAsync(characterMalId);
			dbContext.Seiyuus.Should().HaveCount(2);
			dbContext.Animes.Should().BeEmpty();
			dbContext.AnimeCharacters.Should().ContainSingle();
			dbContext.AnimeRoles.Should().BeEmpty();
		}

		[Fact]
		public async Task HandleAsync_GivenNotExistingJapanesePersonWithSingleRoles_ShouldInsertNewSeiyuuWithRole()
		{
			// Given
			const long animeMalId = 100;
			const long characterMalId = 1000;
			var dbContext = InMemoryDbProvider.GetDbContext();
			const int startingSeiyuuMalId = 1;
			var s3Service = new S3ServiceStub(startingSeiyuuMalId);

			const string returnedSeiyuuName = "PostUpdateSeiyuuName";
			const string returnedSeiyuuAbout = "PostUpdateSeiyuuAbout";
			const string returnedSeiyuuGivenName = "花澤";
			const string returnedSeiyuuFamilyNameName = "香菜";
			const string returnedSeiyuuImageUrl = "PostUpdateSeiyuuImageUrl";
			const int returnedSeiyuuPopularity = 1;
			var returnedBirthdate = new DateTime(1990, 1, 1);

			const string returnedCharacterName = "PostUpdateName";
			const string returnedCharacterAbout = "PostUpdateAbout";
			const string returnedCharacterJapaneseName = "PostUpdateJapanese";
			const string returnedCharacterImageUrl = "PostUpdateImageUrl";
			const int returnedCharacterPopularity = 1;

			const string returnedAnimeTitle = "PostUpdateTitle";
			const string returnedAnimeAbout = "PostUpdateAbout";
			const string returnedAnimeEnglishTitle = "PostUpdateEnglish";
			const string returnedAnimeJapaneseTitle = "PostUpdateJapanese";
			const string returnedAnimeImageUrl = "PostUpdateImageUrl";
			const int returnedAnimePopularity = 3;

			var returnedSeiyuu = new Person
			{
				Name = returnedSeiyuuName,
				About = returnedSeiyuuAbout,
				GivenName = returnedSeiyuuGivenName,
				FamilyName = returnedSeiyuuFamilyNameName,
				Images =new ImagesSet
				{
					JPG = new Image { ImageUrl = returnedSeiyuuImageUrl }
				},
				MemberFavorites = returnedSeiyuuPopularity,
				Birthday = returnedBirthdate
			};
			var returnedRoles = new List<VoiceActingRole>
			{
				new VoiceActingRole
				{
					Role = "Main",
					Anime = new MalImageSubItem
					{
						MalId = animeMalId
					},
					Character = new MalImageSubItem()
					{
						MalId = characterMalId
					}
				}
			};

			var returnedCharacter = new Character
			{
				MalId = characterMalId,
				Name = returnedCharacterName,
				About = returnedCharacterAbout,
				NameKanji = returnedCharacterJapaneseName,
				Images =new ImagesSet
				{
					JPG = new Image { ImageUrl = returnedCharacterImageUrl }
				},
				Nicknames = new List<string>(),
				Favorites = returnedCharacterPopularity
			};

			var returnedAnime = new Anime
			{
				MalId = animeMalId,
				Synopsis = returnedAnimeAbout,
				Titles = AnimeTitlesBuilder.Build(returnedAnimeTitle, returnedAnimeEnglishTitle, returnedAnimeJapaneseTitle),
				Images =new ImagesSet
				{
					JPG = new Image { ImageUrl = returnedAnimeImageUrl }
				},
				Members = returnedAnimePopularity
			};

			var tenraiServiceBuilder = new TenraiServiceBuilder()
				.WithPersonReturned(returnedSeiyuu, returnedRoles)
				.WithCharacterReturned(returnedCharacter)
				.WithAnimeReturned(returnedAnime);

			var anime = new SeiyuuBuilder()
				.WithMalId(startingSeiyuuMalId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, tenraiServiceBuilder.Build(), s3Service, 1);

			// When
			await handler.HandleAsync();

			// Then
			await tenraiServiceBuilder.TenraiClient.Received(1).GetPersonFullDataAsync(startingSeiyuuMalId + 1);
			await tenraiServiceBuilder.TenraiClient.Received(1).GetAnimeAsync(animeMalId);
			await tenraiServiceBuilder.TenraiClient.Received(1).GetCharacterAsync(characterMalId);
			using (new AssertionScope())
			{
				dbContext.Seiyuus.Should().HaveCount(2);
				dbContext.Animes.Should().ContainSingle();
				dbContext.AnimeCharacters.Should().ContainSingle();
				dbContext.AnimeRoles.Should().ContainSingle();
			}
		}

		private InsertSeiyuuHandler CreateHandler(SeiyuuMoeContext dbContext, TenraiService tenraiService, IS3Service s3Service, int insertSeiyuuBatchSize)
		{
			var animeRepository = new AnimeRepository(dbContext);
			var seiyuuRepository = new SeiyuuRepository(dbContext);
			var characterRepository = new CharacterRepository(dbContext);
			var animeRoleRepository = new AnimeRoleRepository(dbContext);
			var seasonRepository = new SeasonRepository(dbContext);

			return new InsertSeiyuuHandler(
				insertSeiyuuBatchSize,
				0,
				seiyuuRepository,
				seasonRepository,
				characterRepository,
				animeRepository,
				animeRoleRepository,
				tenraiService,
				s3Service
			);
		}
	}
}
using FluentAssertions;
using FluentAssertions.Execution;
using JikanDotNet;
using JikanDotNet.Exceptions;
using Moq;
using SeiyuuMoe.Domain.S3;
using SeiyuuMoe.Infrastructure.Database.Animes;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Database.Seiyuus;
using SeiyuuMoe.Infrastructure.Jikan;
using SeiyuuMoe.MalBackgroundJobs.Application.Handlers;
using SeiyuuMoe.Tests.Common.Builders.Jikan;
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
		public async Task HandleAsync_GivenEmptyDatabaseAndBatchSizeOne_ShouldCallJikanOnceWithNextId()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build(), new S3ServiceStub(1), 1);

			// When
			await handler.HandleAsync();

			// Then
			jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(2), Times.Once);
		}

		[Fact]
		public async Task HandleAsync_GivenEmptyDatabaseAndBatchSizeOneAndBiggerId_ShouldCallJikanOnceWithNextId()
		{
			// Given
			var lastId = 10000;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build(), new S3ServiceStub(lastId), 1);

			// When
			await handler.HandleAsync();

			// Then
			jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(lastId + 1), Times.Once);
		}

		[Fact]
		public async Task HandleAsync_GivenEmptyDatabaseAndBatchSizeFive_ShouldCallJikanFiveTimesWithNextIds()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build(), new S3ServiceStub(1), 5);

			// When
			await handler.HandleAsync();

			// Then
			using (new AssertionScope())
			{
				jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(2), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(3), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(4), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(5), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(6), Times.Once);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenEmptyDatabaseAndBatchSizeOne_LastInsertedSeiyuuIdShouldBeBiggerByOne()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder();
			var s3Service = new S3ServiceStub(1);

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build(), s3Service, 1);

			// When
			await handler.HandleAsync();

			// Then
			var state = await s3Service.GetBgJobsStateAsync(It.IsAny<string>());
			state.LastCheckedSeiyuuMalId.Should().Be(2);
		}

		[Fact]
		public async Task HandleAsync_GivenEmptyDatabaseAndBatchSizeFive_LastInsertedSeiyuuIdShouldBeBiggerByFive()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder();
			var s3Service = new S3ServiceStub(1);

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build(), s3Service, 5);

			// When
			await handler.HandleAsync();

			// Then
			var state = await s3Service.GetBgJobsStateAsync(It.IsAny<string>());
			state.LastCheckedSeiyuuMalId.Should().Be(6);
		}

		[Fact]
		public async Task HandleAsync_GivenExistingSeiyuu_ShouldSkipAndNotCallJikan()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder();
			const int startingMalId = 1;
			var s3Service = new S3ServiceStub(startingMalId);

			var anime = new SeiyuuBuilder()
				.WithMalId(startingMalId + 1)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build(), s3Service, 5);

			// When
			await handler.HandleAsync();

			// Then
			jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(It.IsAny<int>()), Times.Never);
			dbContext.Seiyuus.Should().ContainSingle();
		}

		[Fact]
		public async Task HandleAsync_GivenNotExistingSeiyuuExceptionFromJikan_ShouldSkipAndNotInsert()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder().WithGetPersonThrowing();
			const int startingMalId = 1;
			var s3Service = new S3ServiceStub(startingMalId);

			var anime = new SeiyuuBuilder()
				.WithMalId(startingMalId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build(), s3Service, 1);

			// When
			var action = handler.Awaiting(x => x.HandleAsync());

			// Then
			using (new AssertionScope())
			{
				await action.Should().ThrowExactlyAsync<JikanRequestException>();
				dbContext.Seiyuus.Should().ContainSingle();
			}
			jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(startingMalId + 1), Times.Once);
		}

		[Fact]
		public async Task HandleAsync_GivenNotExistingSeiyuuNullFromJikan_ShouldSkipAndNotInsert()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(null);
			const int startingMalId = 1;
			var s3Service = new S3ServiceStub(startingMalId);

			var anime = new SeiyuuBuilder()
				.WithMalId(startingMalId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build(), s3Service, 1);

			// When
			await handler.HandleAsync();

			// Then
			jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(startingMalId + 1), Times.Once);
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
				More = returnedSeiyuuAbout,
				GivenName = returnedSeiyuuGivenName,
				FamilyName = returnedSeiyuuFamilyNameName,
				ImageURL = returnedSeiyuuImageUrl,
				MemberFavorites = returnedSeiyuuPopularity,
				Birthday = returnedBirthdate
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu);

			var anime = new SeiyuuBuilder()
				.WithMalId(startingMalId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build(), s3Service, 1);

			// When
			await handler.HandleAsync();

			// Then
			jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(startingMalId + 1), Times.Once);
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
				More = returnedSeiyuuAbout,
				GivenName = returnedSeiyuuGivenName,
				FamilyName = returnedSeiyuuFamilyNameName,
				ImageURL = returnedSeiyuuImageUrl,
				MemberFavorites = returnedSeiyuuPopularity,
				Birthday = returnedBirthdate
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu);

			var anime = new SeiyuuBuilder()
				.WithMalId(startingMalId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build(), s3Service, 5);

			// When
			await handler.HandleAsync();

			// Then
			jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(startingMalId + 1), Times.Once);
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
				More = returnedSeiyuuAbout,
				GivenName = returnedSeiyuuGivenName,
				FamilyName = returnedSeiyuuFamilyNameName,
				ImageURL = returnedSeiyuuImageUrl,
				MemberFavorites = returnedSeiyuuPopularity,
				Birthday = returnedBirthdate,
				VoiceActingRoles = new List<VoiceActingRole>
				{
					new VoiceActingRole
					{
						Role = "Main",
						Anime = new MALImageSubItem
						{
							MalId = animeMalId
						},
						Character = new MALImageSubItem
						{
							MalId = characterMalId
						}
					}
				}
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu);

			var anime = new SeiyuuBuilder()
				.WithMalId(startingSeiyuuMalId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build(), s3Service, 5);

			// When
			await handler.HandleAsync();

			// Then
			jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(startingSeiyuuMalId + 1), Times.Once);
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
				More = returnedSeiyuuAbout,
				GivenName = returnedSeiyuuGivenName,
				FamilyName = returnedSeiyuuFamilyNameName,
				ImageURL = returnedSeiyuuImageUrl,
				MemberFavorites = returnedSeiyuuPopularity,
				Birthday = returnedBirthdate,
				VoiceActingRoles = new List<VoiceActingRole>
				{
					new VoiceActingRole
					{
						Role = "Main",
						Anime = new MALImageSubItem
						{
							MalId = animeMalId
						},
						Character = new MALImageSubItem
						{
							MalId = characterMalId
						}
					}
				}
			};

			var jikanServiceBuilder = new JikanServiceBuilder()
				.WithPersonReturned(returnedSeiyuu)
				.WithCharacterReturned(null)
				.WithAnimeReturned(null);

			var anime = new SeiyuuBuilder()
				.WithMalId(startingSeiyuuMalId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build(), s3Service, 1);

			// When
			await handler.HandleAsync();

			// Then
			jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(startingSeiyuuMalId + 1), Times.Once);
			jikanServiceBuilder.JikanClient.Verify(x => x.GetAnime(animeMalId), Times.Once);
			jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacter(characterMalId), Times.Once);
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
				More = returnedSeiyuuAbout,
				GivenName = returnedSeiyuuGivenName,
				FamilyName = returnedSeiyuuFamilyNameName,
				ImageURL = returnedSeiyuuImageUrl,
				MemberFavorites = returnedSeiyuuPopularity,
				Birthday = returnedBirthdate,
				VoiceActingRoles = new List<VoiceActingRole>
				{
					new VoiceActingRole
					{
						Role = "Main",
						Anime = new MALImageSubItem
						{
							MalId = animeMalId
						},
						Character = new MALImageSubItem
						{
							MalId = characterMalId
						}
					}
				}
			};

			var returnedAnime = new Anime
			{
				MalId = animeMalId,
				Title = returnedAnimeTitle,
				Synopsis = returnedAnimeAbout,
				TitleEnglish = returnedAnimeEnglishTitle,
				TitleJapanese = returnedAnimeJapaneseTitle,
				ImageURL = returnedAnimeImageUrl,
				TitleSynonyms = new List<string>(),
				Members = returnedAnimePopularity
			};

			var jikanServiceBuilder = new JikanServiceBuilder()
				.WithPersonReturned(returnedSeiyuu)
				.WithCharacterReturned(null)
				.WithAnimeReturned(returnedAnime);

			var anime = new SeiyuuBuilder()
				.WithMalId(startingSeiyuuMalId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build(), s3Service, 1);

			// When
			await handler.HandleAsync();

			// Then
			jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(startingSeiyuuMalId + 1), Times.Once);
			jikanServiceBuilder.JikanClient.Verify(x => x.GetAnime(animeMalId), Times.Once);
			jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacter(characterMalId), Times.Once);
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
				More = returnedSeiyuuAbout,
				GivenName = returnedSeiyuuGivenName,
				FamilyName = returnedSeiyuuFamilyNameName,
				ImageURL = returnedSeiyuuImageUrl,
				MemberFavorites = returnedSeiyuuPopularity,
				Birthday = returnedBirthdate,
				VoiceActingRoles = new List<VoiceActingRole>
				{
					new VoiceActingRole
					{
						Role = "Main",
						Anime = new MALImageSubItem
						{
							MalId = animeMalId
						},
						Character = new MALImageSubItem
						{
							MalId = characterMalId
						}
					}
				}
			};

			var returnedCharacter = new Character
			{
				MalId = characterMalId,
				Name = returnedCharacterName,
				About = returnedCharacterAbout,
				NameKanji = returnedCharacterJapaneseName,
				ImageURL = returnedCharacterImageUrl,
				Nicknames = new List<string>(),
				MemberFavorites = returnedCharacterPopularity
			};

			var jikanServiceBuilder = new JikanServiceBuilder()
				.WithPersonReturned(returnedSeiyuu)
				.WithCharacterReturned(returnedCharacter)
				.WithAnimeReturned(null);

			var anime = new SeiyuuBuilder()
				.WithMalId(startingSeiyuuMalId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build(), s3Service, 1);

			// When
			await handler.HandleAsync();

			// Then
			jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(startingSeiyuuMalId + 1), Times.Once);
			jikanServiceBuilder.JikanClient.Verify(x => x.GetAnime(animeMalId), Times.Once);
			jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacter(characterMalId), Times.Once);
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
				More = returnedSeiyuuAbout,
				GivenName = returnedSeiyuuGivenName,
				FamilyName = returnedSeiyuuFamilyNameName,
				ImageURL = returnedSeiyuuImageUrl,
				MemberFavorites = returnedSeiyuuPopularity,
				Birthday = returnedBirthdate,
				VoiceActingRoles = new List<VoiceActingRole>
				{
					new VoiceActingRole
					{
						Role = "Main",
						Anime = new MALImageSubItem
						{
							MalId = animeMalId
						},
						Character = new MALImageSubItem
						{
							MalId = characterMalId
						}
					}
				}
			};

			var returnedCharacter = new Character
			{
				MalId = characterMalId,
				Name = returnedCharacterName,
				About = returnedCharacterAbout,
				NameKanji = returnedCharacterJapaneseName,
				ImageURL = returnedCharacterImageUrl,
				Nicknames = new List<string>(),
				MemberFavorites = returnedCharacterPopularity
			};

			var returnedAnime = new Anime
			{
				MalId = animeMalId,
				Title = returnedAnimeTitle,
				Synopsis = returnedAnimeAbout,
				TitleEnglish = returnedAnimeEnglishTitle,
				TitleJapanese = returnedAnimeJapaneseTitle,
				ImageURL = returnedAnimeImageUrl,
				TitleSynonyms = new List<string>(),
				Members = returnedAnimePopularity
			};

			var jikanServiceBuilder = new JikanServiceBuilder()
				.WithPersonReturned(returnedSeiyuu)
				.WithCharacterReturned(returnedCharacter)
				.WithAnimeReturned(returnedAnime);

			var anime = new SeiyuuBuilder()
				.WithMalId(startingSeiyuuMalId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build(), s3Service, 1);

			// When
			await handler.HandleAsync();

			// Then
			jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(startingSeiyuuMalId + 1), Times.Once);
			jikanServiceBuilder.JikanClient.Verify(x => x.GetAnime(animeMalId), Times.Once);
			jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacter(characterMalId), Times.Once);
			using (new AssertionScope())
			{
				dbContext.Seiyuus.Should().HaveCount(2);
				dbContext.Animes.Should().ContainSingle();
				dbContext.AnimeCharacters.Should().ContainSingle();
				dbContext.AnimeRoles.Should().ContainSingle();
			}
		}

		private InsertSeiyuuHandler CreateHandler(SeiyuuMoeContext dbContext, JikanService jikanService, IS3Service s3Service, int insertSeiyuuBatchSize)
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
				jikanService,
				s3Service
			);
		}
	}
}
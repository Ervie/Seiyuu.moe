using FluentAssertions;
using FluentAssertions.Execution;
using JikanDotNet;
using Microsoft.EntityFrameworkCore;
using Moq;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.SqsMessages;
using SeiyuuMoe.Infrastructure.Animes;
using SeiyuuMoe.Infrastructure.Characters;
using SeiyuuMoe.Infrastructure.Context;
using SeiyuuMoe.Infrastructure.Jikan;
using SeiyuuMoe.Infrastructure.Seasons;
using SeiyuuMoe.Infrastructure.Seiyuus;
using SeiyuuMoe.MalBackgroundJobs.Application.Handlers;
using SeiyuuMoe.Tests.Common.Builders.Jikan;
using SeiyuuMoe.Tests.Common.Builders.Model;
using SeiyuuMoe.Tests.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Component.MalBackgroundJobs
{
	public class UpdateSeiyuuHandlerTests
	{
		[Fact]
		public async Task HandleAsync_GivenEmptyDatabase_ShouldNotThrowAndNotCallJikan()
		{
			// Given
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			var command = new UpdateSeiyuuMessage
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
				jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnime(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacter(It.IsAny<long>()), Times.Never);
			}
		}

		[Fact]
		public async Task HandleAsync_EmptyPersonResponse_ShouldNotThrowAndCallJikanOnce()
		{
			// Given
			const int malId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(null); ;

			var anime = new SeiyuuBuilder()
				.WithMalId(malId)
				.Build();

			await dbContext.AddAsync(anime);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			var command = new UpdateSeiyuuMessage
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
				jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(malId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnime(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacter(It.IsAny<long>()), Times.Never);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenCorrectPersonResponseWithNoRoles_ShouldUpdateBasicData()
		{
			// Given
			const int malId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();

			const string returnedSeiyuuName = "PostUpdateSeiyuuName";
			const string returnedSeiyuuAbout = "PostUpdateSeiyuuAbout";
			const string returnedSeiyuuGivenName = "PostUpdateSeiyuuJapaneseGivenName";
			const string returnedSeiyuuFamilyNameName = "PostUpdateSeiyuuJapaneseFamilyName";
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

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu); ;

			var seiyuu = new SeiyuuBuilder()
				.WithMalId(malId)
				.WithName("PreUpdateSeiyuuName")
				.WithAbout("PreUpdateSeiyuuAbout")
				.WithJapaneseName("PreUpdateSeiyuuName")
				.WithImageUrl("PreUpdateSeiyuuImage")
				.WithPopularity(0)
				.Build();

			await dbContext.AddAsync(seiyuu);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			var command = new UpdateSeiyuuMessage
			{
				Id = Guid.NewGuid(),
				MalId = 1
			};

			// When
			await handler.HandleAsync(command);

			// Then
			var updatedSeiyuu = await dbContext.Seiyuus.FirstAsync(x => x.MalId == malId);
			using (new AssertionScope())
			{
				updatedSeiyuu.About.Should().Be(returnedSeiyuuAbout);
				updatedSeiyuu.Name.Should().Be(returnedSeiyuuName);
				updatedSeiyuu.Popularity.Should().Be(returnedSeiyuuPopularity);
				updatedSeiyuu.ImageUrl.Should().Be(returnedSeiyuuImageUrl);
				updatedSeiyuu.KanjiName.Should().Be($"{returnedSeiyuuFamilyNameName} {returnedSeiyuuGivenName}");
				updatedSeiyuu.Birthday.Should().Be(returnedBirthdate);

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(malId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnime(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacter(It.IsAny<long>()), Times.Never);
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
			var dbContext = InMemoryDbProvider.GetDbContext();

			var returnedSeiyuu = new Person
			{
				ImageURL = imageUrl
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu); ;

			var seiyuu = new SeiyuuBuilder()
				.WithMalId(malId)
				.WithName("PreUpdateSeiyuuName")
				.WithAbout("PreUpdateSeiyuuAbout")
				.WithJapaneseName("PreUpdateSeiyuuName")
				.WithImageUrl("PreUpdateSeiyuuImage")
				.WithPopularity(0)
				.Build();

			await dbContext.AddAsync(seiyuu);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			var command = new UpdateSeiyuuMessage
			{
				Id = Guid.NewGuid(),
				MalId = 1
			};

			// When
			await handler.HandleAsync(command);

			// Then
			var updatedSeiyuu = await dbContext.Seiyuus.FirstAsync(x => x.MalId == malId);
			using (new AssertionScope())
			{
				updatedSeiyuu.ImageUrl.Should().BeEmpty();

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(malId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnime(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacter(It.IsAny<long>()), Times.Never);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSeiyuuWithSingleAlreadyInsertedRole_ShouldUpdateBasicDataAndCallJikanOnce()
		{
			// Given
			var seiyuuId = Guid.NewGuid();
			const long seiyuuMalId = 1;
			const long animeMalId = 100;
			const long characterMalId = 1000;
			var dbContext = InMemoryDbProvider.GetDbContext();

			var returnedSeiyuu = new Person
			{
				MalId = seiyuuMalId,
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

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu); ;

			var role = new AnimeRoleBuilder()
				.WithSeiyuu(x => x
					.WithId(seiyuuId)
					.WithMalId(seiyuuMalId)
					.WithName("PreUpdateSeiyuuName")
					.WithAbout("PreUpdateSeiyuuAbout")
					.WithJapaneseName("PreUpdateSeiyuuName")
					.WithImageUrl("PreUpdateSeiyuuImage")
					.WithPopularity(0)
				)
				.WithCharacter(x => x
					.WithMalId(characterMalId)
				)
				.WithAnime(x => x
					.WithMalId(animeMalId)
				)
				.WithLanguage(x => x
					.WithLanguageId(LanguageId.Japanese)
				)
				.WithRoleType(x => x
					.WithId(AnimeRoleTypeId.Main)
				)
				.Build();

			await dbContext.AddAsync(role);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			var command = new UpdateSeiyuuMessage
			{
				Id = seiyuuId,
				MalId = seiyuuMalId
			};

			// When
			await handler.HandleAsync(command);

			// Then
			var updatedSeiyuu = await dbContext.Seiyuus.FirstAsync(x => x.MalId == seiyuuMalId);
			using (new AssertionScope())
			{
				dbContext.AnimeRoles.Should().ContainSingle();

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnime(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacter(It.IsAny<long>()), Times.Never);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSeiyuuWithMultipleAlreadyInsertedRole_ShouldUpdateBasicDataAndCallJikanOnce()
		{
			// Given
			var seiyuuId = Guid.NewGuid();
			const long seiyuuMalId = 1;
			const long anime1MalId = 100;
			const long character1MalId = 1000;
			const long anime2MalId = 101;
			const long character2MalId = 1001;
			var dbContext = InMemoryDbProvider.GetDbContext();

			var returnedSeiyuu = new Person
			{
				MalId = seiyuuMalId,
				VoiceActingRoles = new List<VoiceActingRole>
				{
					new VoiceActingRole
					{
						Role = "Main",
						Anime = new MALImageSubItem
						{
							MalId = anime1MalId
						},
						Character = new MALImageSubItem
						{
							MalId = character1MalId
						}
					},
					new VoiceActingRole
					{
						Role = "Supporting",
						Anime = new MALImageSubItem
						{
							MalId = anime2MalId
						},
						Character = new MALImageSubItem
						{
							MalId = character2MalId
						}
					}
				}
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu);
			var japanese = new LanguageBuilder().WithLanguageId(LanguageId.Japanese).Build();
			var seiyuu = new SeiyuuBuilder()
				.WithId(seiyuuId)
				.WithMalId(seiyuuMalId)
				.WithName("PreUpdateSeiyuuName")
				.WithAbout("PreUpdateSeiyuuAbout")
				.WithJapaneseName("PreUpdateSeiyuuName")
				.WithImageUrl("PreUpdateSeiyuuImage")
				.WithPopularity(0)
				.Build();
			var roles = new List<AnimeRole>
			{
				new AnimeRoleBuilder()
				.WithSeiyuu(seiyuu)
				.WithCharacter(x => x
					.WithMalId(character1MalId)
				)
				.WithAnime(x => x
					.WithMalId(anime1MalId)
				)
				.WithLanguage(japanese)
				.WithRoleType(x => x
					.WithId(AnimeRoleTypeId.Main)
				)
				.Build(),
		
				new AnimeRoleBuilder()
				.WithSeiyuu(seiyuu)
				.WithCharacter(x => x
					.WithMalId(character2MalId)
				)
				.WithAnime(x => x
					.WithMalId(anime2MalId)
				)
				.WithLanguage(japanese)
				.WithRoleType(x => x
					.WithId(AnimeRoleTypeId.Supporting)
				)
				.Build()
			};

			await dbContext.AddRangeAsync(roles);
			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			var command = new UpdateSeiyuuMessage
			{
				Id = seiyuuId,
				MalId = seiyuuMalId
			};

			// When
			await handler.HandleAsync(command);

			// Then
			var updatedSeiyuu = await dbContext.Seiyuus.FirstAsync(x => x.MalId == seiyuuMalId);
			using (new AssertionScope())
			{
				dbContext.AnimeRoles.Should().HaveCount(2);

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnime(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacter(It.IsAny<long>()), Times.Never);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSeiyuuWithSingleNotInsertedRoleWithInsertedAnimeAndCharacter_ShouldInsertNewRole()
		{
			// Given
			var seiyuuId = Guid.NewGuid();
			const long seiyuuMalId = 1;
			const long animeMalId = 100;
			const long characterMalId = 1000;
			var dbContext = InMemoryDbProvider.GetDbContext();

			var returnedSeiyuu = new Person
			{
				MalId = seiyuuMalId,
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

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu); ;

			var seiyuu = new SeiyuuBuilder()
				.WithId(seiyuuId)
				.WithMalId(seiyuuMalId)
				.WithName("PreUpdateSeiyuuName")
				.WithAbout("PreUpdateSeiyuuAbout")
				.WithJapaneseName("PreUpdateSeiyuuName")
				.WithImageUrl("PreUpdateSeiyuuImage")
				.WithPopularity(0)
				.Build();

			var anime = new AnimeBuilder()
				.WithMalId(animeMalId)
				.WithTitle("PreUpdateTitle")
				.WithAbout("PreUpdateAbout")
				.WithEnglishTitle("PreUpdateEnglish")
				.WithJapaneseTitle("PreUpdateJapaneses")
				.WithImageUrl("PreUpdateImage")
				.WithAnimeStatus(x => x.WithName("Airing"))
				.WithAnimeType(x => x.WithId(AnimeTypeId.TV))
				.WithPopularity(0)
				.Build();

			var character = new CharacterBuilder()
				.WithMalId(characterMalId)
				.WithName("PreUpdateName")
				.WithAbout("PreUpdateAbout")
				.WithKanjiName("PreUpdateJapanese")
				.WithImageUrl("PreUpdateImageUrl")
				.WithPopularity(0)
				.Build();

			await dbContext.AddAsync(seiyuu);
			await dbContext.AddAsync(anime);
			await dbContext.AddAsync(character);
			await dbContext.SaveChangesAsync();

			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			var command = new UpdateSeiyuuMessage
			{
				Id = seiyuuId,
				MalId = seiyuuMalId
			};

			// When
			await handler.HandleAsync(command);

			// Then
			var updatedSeiyuu = await dbContext.Seiyuus.Include(x => x.Role).FirstAsync(x => x.MalId == seiyuuMalId);
			using (new AssertionScope())
			{
				dbContext.AnimeRoles.Should().ContainSingle();

				updatedSeiyuu.Role.First().AnimeId.Should().Be(anime.Id);
				updatedSeiyuu.Role.First().CharacterId.Should().Be(character.Id);
				updatedSeiyuu.Role.First().SeiyuuId.Should().Be(seiyuu.Id);

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnime(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacter(It.IsAny<long>()), Times.Never);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSeiyuuWithMultipleNotInsertedRoleWithInsertedAnimeAndCharacter_ShouldInsertMultipleNewRole()
		{
			// Given
			var seiyuuId = Guid.NewGuid();
			var anime1Id = Guid.NewGuid();
			var anime2Id = Guid.NewGuid();
			var character1Id = Guid.NewGuid();
			var character2Id = Guid.NewGuid();
			const long seiyuuMalId = 1;
			const long anime1MalId = 100;
			const long anime2MalId = 101;
			const long character1MalId = 1000;
			const long character2MalId = 1001;
			var dbContext = InMemoryDbProvider.GetDbContext();

			var returnedSeiyuu = new Person
			{
				MalId = seiyuuMalId,
				VoiceActingRoles = new List<VoiceActingRole>
				{
					new VoiceActingRole
					{
						Role = "Main",
						Anime = new MALImageSubItem
						{
							MalId = anime1MalId
						},
						Character = new MALImageSubItem
						{
							MalId = character1MalId
						}
					},
					new VoiceActingRole
					{
						Role = "Supporting",
						Anime = new MALImageSubItem
						{
							MalId = anime2MalId
						},
						Character = new MALImageSubItem
						{
							MalId = character2MalId
						}
					}
				}
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu); ;
			var animeStatus = new AnimeStatusBuilder()
				.WithId(AnimeStatusId.CurrentlyAiring)
				.WithName("Airing")
				.Build();
			var animeType = new AnimeTypeBuilder()
				.WithId(AnimeTypeId.TV)
				.Build();

			var seiyuu = new SeiyuuBuilder()
				.WithId(seiyuuId)
				.WithMalId(seiyuuMalId)
				.WithName("PreUpdateSeiyuuName")
				.WithAbout("PreUpdateSeiyuuAbout")
				.WithJapaneseName("PreUpdateSeiyuuName")
				.WithImageUrl("PreUpdateSeiyuuImage")
				.WithPopularity(0)
				.Build();

			var animes = new List<Domain.Entities.Anime>
			{
				new AnimeBuilder()
					.WithId(anime1Id)
					.WithMalId(anime1MalId)
					.WithTitle("PreUpdateTitle")
					.WithAbout("PreUpdateAbout")
					.WithEnglishTitle("PreUpdateEnglish")
					.WithJapaneseTitle("PreUpdateJapaneses")
					.WithImageUrl("PreUpdateImage")
					.WithAnimeStatus(animeStatus)
					.WithAnimeType(animeType)
					.WithPopularity(0)
					.Build(),
				new AnimeBuilder()
					.WithId(anime2Id)
					.WithMalId(anime2MalId)
					.WithTitle("PreUpdateTitle")
					.WithAbout("PreUpdateAbout")
					.WithEnglishTitle("PreUpdateEnglish")
					.WithJapaneseTitle("PreUpdateJapaneses")
					.WithImageUrl("PreUpdateImage")
					.WithAnimeStatus(animeStatus)
					.WithAnimeType(animeType)
					.WithPopularity(0)
					.Build()
			};

			var characters = new List<AnimeCharacter>
			{ 
				new CharacterBuilder()
					.WithId(character1Id)
					.WithMalId(character1MalId)
					.WithName("PreUpdateName")
					.WithAbout("PreUpdateAbout")
					.WithKanjiName("PreUpdateJapanese")
					.WithImageUrl("PreUpdateImageUrl")
					.WithPopularity(0)
					.Build(),
				new CharacterBuilder()
					.WithId(character2Id)
					.WithMalId(character2MalId)
					.WithName("PreUpdateName")
					.WithAbout("PreUpdateAbout")
					.WithKanjiName("PreUpdateJapanese")
					.WithImageUrl("PreUpdateImageUrl")
					.WithPopularity(0)
					.Build()
			};

			await dbContext.AddAsync(seiyuu);
			await dbContext.AddRangeAsync(animes);
			await dbContext.AddRangeAsync(characters);
			await dbContext.SaveChangesAsync();

			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			var command = new UpdateSeiyuuMessage
			{
				Id = seiyuuId,
				MalId = seiyuuMalId
			};

			// When
			await handler.HandleAsync(command);

			// Then
			var updatedSeiyuu = await dbContext.Seiyuus.Include(x => x.Role).FirstAsync(x => x.MalId == seiyuuMalId);
			using (new AssertionScope())
			{
				dbContext.AnimeRoles.Should().HaveCount(2);

				updatedSeiyuu.Role.First().AnimeId.Should().Be(anime1Id);
				updatedSeiyuu.Role.Skip(1).First().AnimeId.Should().Be(anime2Id);
				updatedSeiyuu.Role.First().CharacterId.Should().Be(character1Id);
				updatedSeiyuu.Role.Skip(1).First().CharacterId.Should().Be(character2Id);
				updatedSeiyuu.Role.First().SeiyuuId.Should().Be(seiyuu.Id);
				updatedSeiyuu.Role.Skip(1).First().SeiyuuId.Should().Be(seiyuu.Id);

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnime(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacter(It.IsAny<long>()), Times.Never);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSeiyuuWithSingleInsertedAndSingleNotInsertedRoleWithInsertedAnimeAndCharacter_ShouldInsertSingleNewRole()
		{
			// Given
			var seiyuuId = Guid.NewGuid();
			var anime1Id = Guid.NewGuid();
			var anime2Id = Guid.NewGuid();
			var character1Id = Guid.NewGuid();
			var character2Id = Guid.NewGuid();
			const long seiyuuMalId = 1;
			const long anime1MalId = 100;
			const long anime2MalId = 101;
			const long character1MalId = 1000;
			const long character2MalId = 1001;
			var dbContext = InMemoryDbProvider.GetDbContext();

			var returnedSeiyuu = new Person
			{
				MalId = seiyuuMalId,
				VoiceActingRoles = new List<VoiceActingRole>
				{
					new VoiceActingRole
					{
						Role = "Main",
						Anime = new MALImageSubItem
						{
							MalId = anime1MalId
						},
						Character = new MALImageSubItem
						{
							MalId = character1MalId
						}
					},
					new VoiceActingRole
					{
						Role = "Supporting",
						Anime = new MALImageSubItem
						{
							MalId = anime2MalId
						},
						Character = new MALImageSubItem
						{
							MalId = character2MalId
						}
					}
				}
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu); ;
			var animeStatus = new AnimeStatusBuilder()
				.WithId(AnimeStatusId.CurrentlyAiring)
				.WithName("Airing")
				.Build();
			var animeType = new AnimeTypeBuilder()
				.WithId(AnimeTypeId.TV)
				.Build();

			var seiyuu = new SeiyuuBuilder()
				.WithId(seiyuuId)
				.WithMalId(seiyuuMalId)
				.WithName("PreUpdateSeiyuuName")
				.WithAbout("PreUpdateSeiyuuAbout")
				.WithJapaneseName("PreUpdateSeiyuuName")
				.WithImageUrl("PreUpdateSeiyuuImage")
				.WithPopularity(0)
				.Build();

			var animes = new List<Domain.Entities.Anime>
			{
				new AnimeBuilder()
					.WithId(anime2Id)
					.WithMalId(anime2MalId)
					.WithTitle("PreUpdateTitle")
					.WithAbout("PreUpdateAbout")
					.WithEnglishTitle("PreUpdateEnglish")
					.WithJapaneseTitle("PreUpdateJapaneses")
					.WithImageUrl("PreUpdateImage")
					.WithAnimeStatus(animeStatus)
					.WithAnimeType(animeType)
					.WithPopularity(0)
					.Build()
			};

			var characters = new List<AnimeCharacter>
			{
				new CharacterBuilder()
					.WithId(character2Id)
					.WithMalId(character2MalId)
					.WithName("PreUpdateName")
					.WithAbout("PreUpdateAbout")
					.WithKanjiName("PreUpdateJapanese")
					.WithImageUrl("PreUpdateImageUrl")
					.WithPopularity(0)
					.Build()
			};

			var role = new AnimeRoleBuilder()
				.WithSeiyuu(seiyuu)
				.WithCharacter(x => x
					.WithId(character1Id)
					.WithMalId(character1MalId)
				)
				.WithAnime(x => x
					.WithId(anime1Id)
					.WithMalId(anime1MalId)
				)
				.WithLanguage(x => x
					.WithLanguageId(LanguageId.Japanese)
				)
				.WithRoleType(x => x
					.WithId(AnimeRoleTypeId.Main)
				)
				.Build();

			await dbContext.AddAsync(role);
			await dbContext.AddRangeAsync(animes);
			await dbContext.AddRangeAsync(characters);
			await dbContext.SaveChangesAsync();

			await dbContext.SaveChangesAsync();

			var handler = CreateHandler(dbContext, jikanServiceBuilder.Build());

			var command = new UpdateSeiyuuMessage
			{
				Id = seiyuuId,
				MalId = seiyuuMalId
			};

			// When
			await handler.HandleAsync(command);

			// Then
			var updatedSeiyuu = await dbContext.Seiyuus.Include(x => x.Role).FirstAsync(x => x.MalId == seiyuuMalId);
			using (new AssertionScope())
			{
				dbContext.AnimeRoles.Should().HaveCount(2);

				updatedSeiyuu.Role.First().AnimeId.Should().Be(anime1Id);
				updatedSeiyuu.Role.Skip(1).First().AnimeId.Should().Be(anime2Id);
				updatedSeiyuu.Role.First().CharacterId.Should().Be(character1Id);
				updatedSeiyuu.Role.Skip(1).First().CharacterId.Should().Be(character2Id);
				updatedSeiyuu.Role.First().SeiyuuId.Should().Be(seiyuu.Id);
				updatedSeiyuu.Role.Skip(1).First().SeiyuuId.Should().Be(seiyuu.Id);

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPerson(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnime(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacter(It.IsAny<long>()), Times.Never);
			}
		}

		private UpdateSeiyuuHandler CreateHandler(SeiyuuMoeContext dbContext, JikanService jikanService)
		{
			var animeRepository = new AnimeRepository(dbContext);
			var seiyuuRepository = new SeiyuuRepository(dbContext);
			var characterRepository = new CharacterRepository(dbContext);
			var seiyuuRoleRepository = new SeiyuuRoleRepository(dbContext);
			var animeRoleRepository = new AnimeRoleRepository(dbContext);
			var seasonRepository = new SeasonRepository(dbContext);

			return new UpdateSeiyuuHandler(seiyuuRepository, animeRepository, characterRepository, seiyuuRoleRepository, animeRoleRepository, seasonRepository, jikanService);
		}
	}
}
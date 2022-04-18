using FluentAssertions;
using FluentAssertions.Execution;
using JikanDotNet;
using Microsoft.EntityFrameworkCore;
using Moq;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.SqsMessages;
using SeiyuuMoe.Infrastructure.Database.Animes;
using SeiyuuMoe.Infrastructure.Database.Characters;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Database.Seasons;
using SeiyuuMoe.Infrastructure.Database.Seiyuus;
using SeiyuuMoe.Infrastructure.Jikan;
using SeiyuuMoe.MalBackgroundJobs.Application.Handlers;
using SeiyuuMoe.Tests.Common.Builders.Jikan;
using SeiyuuMoe.Tests.Common.Builders.Model;
using SeiyuuMoe.Tests.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using AnimeCharacter = SeiyuuMoe.Domain.Entities.AnimeCharacter;

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
				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(It.IsAny<long>()), Times.Never);
			}
		}

		[Fact]
		public async Task HandleAsync_EmptyPersonResponse_ShouldNotThrowAndCallJikanOnce()
		{
			// Given
			const int malId = 1;
			var dbContext = InMemoryDbProvider.GetDbContext();
			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(null, null); ;

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
				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(malId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(It.IsAny<long>()), Times.Never);
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
				About = returnedSeiyuuAbout,
				GivenName = returnedSeiyuuGivenName,
				FamilyName = returnedSeiyuuFamilyNameName,
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = returnedSeiyuuImageUrl }
				},
				MemberFavorites = returnedSeiyuuPopularity,
				Birthday = returnedBirthdate
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu, null); ;

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

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(malId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(It.IsAny<long>()), Times.Never);
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
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = imageUrl }
				},
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu, null); ;

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

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(malId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(It.IsAny<long>()), Times.Never);
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
				MalId = seiyuuMalId
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

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu, returnedRoles); ;

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

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(It.IsAny<long>()), Times.Never);
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
			};

			var returnedRoles = new List<VoiceActingRole>
			{
				new VoiceActingRole
				{
					Role = "Main",
					Anime = new MalImageSubItem
					{
						MalId = anime1MalId
					},
					Character = new MalImageSubItem()
					{
						MalId = character1MalId
					}
				},
				new VoiceActingRole
				{
					Role = "Supporting",
					Anime = new MalImageSubItem
					{
						MalId = anime2MalId
					},
					Character = new MalImageSubItem()
					{
						MalId = character2MalId
					}
				}
			};
			
			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu, returnedRoles);
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

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(It.IsAny<long>()), Times.Never);
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
				MalId = seiyuuMalId
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

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu, returnedRoles); ;

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

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(It.IsAny<long>()), Times.Never);
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
				MalId = seiyuuMalId
			};

			var returnedRoles = new List<VoiceActingRole>
			{
				new VoiceActingRole
				{
					Role = "Main",
					Anime = new MalImageSubItem
					{
						MalId = anime1MalId
					},
					Character = new MalImageSubItem()
					{
						MalId = character1MalId
					}
				},
				new VoiceActingRole
				{
					Role = "Supporting",
					Anime = new MalImageSubItem
					{
						MalId = anime2MalId
					},
					Character = new MalImageSubItem()
					{
						MalId = character2MalId
					}
				}
			};
			
			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu, returnedRoles); ;
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

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(It.IsAny<long>()), Times.Never);
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
				MalId = seiyuuMalId
			};
			
			var returnedRoles = new List<VoiceActingRole>
			{
				new VoiceActingRole
				{
					Role = "Main",
					Anime = new MalImageSubItem
					{
						MalId = anime1MalId
					},
					Character = new MalImageSubItem()
					{
						MalId = character1MalId
					}
				},
				new VoiceActingRole
				{
					Role = "Supporting",
					Anime = new MalImageSubItem
					{
						MalId = anime2MalId
					},
					Character = new MalImageSubItem()
					{
						MalId = character2MalId
					}
				}
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu, returnedRoles); ;
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

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(It.IsAny<long>()), Times.Never);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSeiyuuWithSingleNotInsertedRoleWithInsertedAnimeAndNotInsertedCharacter_ShouldInsertNewRoleAndNewCharacter()
		{
			// Given
			var seiyuuId = Guid.NewGuid();
			const long seiyuuMalId = 1;
			const long animeMalId = 100;
			const long characterMalId = 1000;
			var dbContext = InMemoryDbProvider.GetDbContext();

			const string returnedCharacterName = "PostUpdateName";
			const string returnedCharacterAbout = "PostUpdateAbout";
			const string returnedCharacterJapaneseName = "PostUpdateJapanese";
			const string returnedCharacterImageUrl = "PostUpdateImageUrl";
			const int returnedCharacterPopularity = 1;

			var returnedSeiyuu = new Person
			{
				MalId = seiyuuMalId
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
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = returnedCharacterImageUrl }
				},
				Nicknames = new List<string>(),
				Favorites = returnedCharacterPopularity
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu, returnedRoles).WithCharacterReturned(returnedCharacter);

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

			await dbContext.AddAsync(seiyuu);
			await dbContext.AddAsync(anime);
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
			var insertedCharacter = await dbContext.AnimeCharacters.FirstAsync(x => x.MalId == characterMalId);
			using (new AssertionScope())
			{
				dbContext.AnimeRoles.Should().ContainSingle();
				dbContext.AnimeCharacters.Should().ContainSingle();

				updatedSeiyuu.Role.First().AnimeId.Should().Be(anime.Id);
				updatedSeiyuu.Role.First().SeiyuuId.Should().Be(seiyuu.Id);
				updatedSeiyuu.Role.First().CharacterId.Should().Be(insertedCharacter.Id);

				insertedCharacter.MalId.Should().Be(characterMalId);
				insertedCharacter.Name.Should().Be(returnedCharacterName);
				insertedCharacter.About.Should().Be(returnedCharacterAbout);
				insertedCharacter.KanjiName.Should().Be(returnedCharacterJapaneseName);
				insertedCharacter.Popularity.Should().Be(returnedCharacterPopularity);
				insertedCharacter.ImageUrl.Should().Be(returnedCharacterImageUrl);

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(characterMalId), Times.Once);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSeiyuuWithSingleNotInsertedRoleWithInsertedAnimeAndNotInsertedCharacterAndNullFromCharacterRequest_ShouldNotInsertAnything()
		{
			// Given
			var seiyuuId = Guid.NewGuid();
			const long seiyuuMalId = 1;
			const long animeMalId = 100;
			const long characterMalId = 1000;
			var dbContext = InMemoryDbProvider.GetDbContext();

			var returnedSeiyuu = new Person
			{
				MalId = seiyuuMalId
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

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu, returnedRoles).WithCharacterReturned(null);

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

			await dbContext.AddAsync(seiyuu);
			await dbContext.AddAsync(anime);
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
			using (new AssertionScope())
			{
				dbContext.AnimeRoles.Should().BeEmpty();
				dbContext.AnimeCharacters.Should().BeEmpty();

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(characterMalId), Times.Once);
			}
		}

		[Theory]
		[InlineData("")]
		[InlineData(null)]
		[InlineData("\t\t  \n \t   ")]
		[InlineData("https://cdn.myanimelist.net/images/questionmark_23.gif")]
		[InlineData("https://cdn.myanimelist.net/img/sp/icon/apple-touch-icon-256.png")]
		public async Task HandleAsync_GivenSeiyuuWithSingleNotInsertedRoleWithInsertedAnimeAndNotInsertedCharacterWithIncorrectImage_ShouldInsertNewRoleAndNewCharacter(string characterImageUrl)
		{
			// Given
			var seiyuuId = Guid.NewGuid();
			const long seiyuuMalId = 1;
			const long animeMalId = 100;
			const long characterMalId = 1000;
			var dbContext = InMemoryDbProvider.GetDbContext();

			var returnedSeiyuu = new Person
			{
				MalId = seiyuuMalId
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
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = characterImageUrl }
				},
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu, returnedRoles).WithCharacterReturned(returnedCharacter);

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

			await dbContext.AddAsync(seiyuu);
			await dbContext.AddAsync(anime);
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
			var insertedCharacter = await dbContext.AnimeCharacters.FirstAsync(x => x.MalId == characterMalId);
			using (new AssertionScope())
			{
				dbContext.AnimeRoles.Should().ContainSingle();
				dbContext.AnimeCharacters.Should().ContainSingle();

				updatedSeiyuu.Role.First().AnimeId.Should().Be(anime.Id);
				updatedSeiyuu.Role.First().SeiyuuId.Should().Be(seiyuu.Id);
				updatedSeiyuu.Role.First().CharacterId.Should().Be(insertedCharacter.Id);

				insertedCharacter.MalId.Should().Be(characterMalId);
				insertedCharacter.ImageUrl.Should().BeEmpty();

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(characterMalId), Times.Once);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSeiyuuWithSingleNotInsertedRoleWithInsertedAnimeAndNotInsertedCharacterWithMultipleNicknames_ShouldInsertNewRoleAndNewCharacter()
		{
			// Given
			var seiyuuId = Guid.NewGuid();
			const long seiyuuMalId = 1;
			const long animeMalId = 100;
			const long characterMalId = 1000;
			var dbContext = InMemoryDbProvider.GetDbContext();

			const string returnedCharacterName = "PostUpdateName";
			const string returnedCharacterAbout = "PostUpdateAbout";
			const string returnedCharacterJapaneseName = "PostUpdateJapanese";
			const string returnedCharacterImageUrl = "PostUpdateImageUrl";
			const int returnedCharacterPopularity = 2;

			var returnedSeiyuu = new Person
			{
				MalId = seiyuuMalId
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
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = returnedCharacterImageUrl }
				},
				Nicknames = new List<string> { "Nickname 1", "Nickname 2", "Nickname 3" },
				Favorites = returnedCharacterPopularity
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu, returnedRoles).WithCharacterReturned(returnedCharacter);

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

			await dbContext.AddAsync(seiyuu);
			await dbContext.AddAsync(anime);
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
			var insertedCharacter = await dbContext.AnimeCharacters.FirstAsync(x => x.MalId == characterMalId);
			using (new AssertionScope())
			{
				dbContext.AnimeRoles.Should().ContainSingle();
				dbContext.AnimeCharacters.Should().ContainSingle();

				updatedSeiyuu.Role.First().AnimeId.Should().Be(anime.Id);
				updatedSeiyuu.Role.First().SeiyuuId.Should().Be(seiyuu.Id);
				updatedSeiyuu.Role.First().CharacterId.Should().Be(insertedCharacter.Id);

				insertedCharacter.MalId.Should().Be(characterMalId);
				insertedCharacter.Name.Should().Be(returnedCharacterName);
				insertedCharacter.KanjiName.Should().Be(returnedCharacterJapaneseName);
				insertedCharacter.Popularity.Should().Be(returnedCharacterPopularity);
				insertedCharacter.ImageUrl.Should().Be(returnedCharacterImageUrl);
				insertedCharacter.Nicknames.Should().Be("Nickname 1;Nickname 2;Nickname 3");

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(characterMalId), Times.Once);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSeiyuuWithSingleNotInsertedRoleWithNotInsertedAnimeAndInsertedCharacter_ShouldInsertNewRoleAndNewAnime()
		{
			// Given
			var seiyuuId = Guid.NewGuid();
			const long seiyuuMalId = 1;
			const long animeMalId = 100;
			const long characterMalId = 1000;
			var dbContext = InMemoryDbProvider.GetDbContext();

			const string returnedAnimeTitle = "PostUpdateTitle";
			const string returnedAnimeAbout = "PostUpdateAbout";
			const string returnedAnimeEnglishTitle = "PostUpdateEnglish";
			const string returnedAnimeJapaneseTitle = "PostUpdateJapanese";
			const string returnedAnimeImageUrl = "PostUpdateImageUrl";
			const int returnedAnimePopularity = 3;

			var returnedSeiyuu = new Person
			{
				MalId = seiyuuMalId
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
			
			var returnedAnime = new JikanDotNet.Anime
			{
				Title = returnedAnimeTitle,
				Synopsis = returnedAnimeAbout,
				TitleEnglish = returnedAnimeEnglishTitle,
				TitleJapanese = returnedAnimeJapaneseTitle,
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = returnedAnimeImageUrl }
				},
				TitleSynonyms = new List<string>(),
				Members = returnedAnimePopularity
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu, returnedRoles).WithAnimeReturned(returnedAnime);

			var seiyuu = new SeiyuuBuilder()
				.WithId(seiyuuId)
				.WithMalId(seiyuuMalId)
				.WithName("PreUpdateSeiyuuName")
				.WithAbout("PreUpdateSeiyuuAbout")
				.WithJapaneseName("PreUpdateSeiyuuName")
				.WithImageUrl("PreUpdateSeiyuuImage")
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
			await dbContext.AddAsync(character);
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
			var insertedAnime = await dbContext.Animes.FirstAsync(x => x.MalId == animeMalId);
			using (new AssertionScope())
			{
				dbContext.AnimeRoles.Should().ContainSingle();
				dbContext.AnimeCharacters.Should().ContainSingle();

				updatedSeiyuu.Role.First().AnimeId.Should().Be(insertedAnime.Id);
				updatedSeiyuu.Role.First().SeiyuuId.Should().Be(seiyuu.Id);
				updatedSeiyuu.Role.First().CharacterId.Should().Be(character.Id);

				insertedAnime.MalId.Should().Be(animeMalId);
				insertedAnime.About.Should().Be(returnedAnimeAbout);
				insertedAnime.Title.Should().Be(returnedAnimeTitle);
				insertedAnime.EnglishTitle.Should().Be(returnedAnimeEnglishTitle);
				insertedAnime.KanjiTitle.Should().Be(returnedAnimeJapaneseTitle);
				insertedAnime.Popularity.Should().Be(returnedAnimePopularity);
				insertedAnime.ImageUrl.Should().Be(returnedAnimeImageUrl);

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(animeMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(It.IsAny<long>()), Times.Never);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSeiyuuWithSingleNotInsertedRoleWithNotInsertedAnimeAndInsertedCharacterAndNullFromAnimeRequest_ShouldNotInsertAnything()
		{
			// Given
			var seiyuuId = Guid.NewGuid();
			const long seiyuuMalId = 1;
			const long animeMalId = 100;
			const long characterMalId = 1000;
			var dbContext = InMemoryDbProvider.GetDbContext();

			var returnedSeiyuu = new Person
			{
				MalId = seiyuuMalId
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
			
			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu, returnedRoles).WithAnimeReturned(null);

			var seiyuu = new SeiyuuBuilder()
				.WithId(seiyuuId)
				.WithMalId(seiyuuMalId)
				.WithName("PreUpdateSeiyuuName")
				.WithAbout("PreUpdateSeiyuuAbout")
				.WithJapaneseName("PreUpdateSeiyuuName")
				.WithImageUrl("PreUpdateSeiyuuImage")
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
			await dbContext.AddAsync(character);
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
			using (new AssertionScope())
			{
				dbContext.AnimeRoles.Should().BeEmpty();
				dbContext.Animes.Should().BeEmpty();

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(animeMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(It.IsAny<long>()), Times.Never);
			}
		}

		[Theory]
		[InlineData("")]
		[InlineData(null)]
		[InlineData("\t\t  \n \t   ")]
		[InlineData("https://cdn.myanimelist.net/images/questionmark_23.gif")]
		[InlineData("https://cdn.myanimelist.net/img/sp/icon/apple-touch-icon-256.png")]
		public async Task HandleAsync_GivenSeiyuuWithSingleNotInsertedRoleWithNotInsertedAnimeWithIncorrectImageAndInsertedCharacter_ShouldInsertNewRoleAndNewAnimeWithEmptyImage(string imageUrl)
		{
			// Given
			var seiyuuId = Guid.NewGuid();
			const long seiyuuMalId = 1;
			const long animeMalId = 100;
			const long characterMalId = 1000;
			var dbContext = InMemoryDbProvider.GetDbContext();

			var returnedSeiyuu = new Person
			{
				MalId = seiyuuMalId
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
			
			var returnedAnime = new JikanDotNet.Anime
			{
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = imageUrl }
				}
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu, returnedRoles).WithAnimeReturned(returnedAnime);

			var seiyuu = new SeiyuuBuilder()
				.WithId(seiyuuId)
				.WithMalId(seiyuuMalId)
				.WithName("PreUpdateSeiyuuName")
				.WithAbout("PreUpdateSeiyuuAbout")
				.WithJapaneseName("PreUpdateSeiyuuName")
				.WithImageUrl("PreUpdateSeiyuuImage")
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
			await dbContext.AddAsync(character);
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
			var insertedAnime = await dbContext.Animes.FirstAsync(x => x.MalId == animeMalId);
			using (new AssertionScope())
			{
				dbContext.AnimeRoles.Should().ContainSingle();
				dbContext.AnimeCharacters.Should().ContainSingle();

				updatedSeiyuu.Role.First().AnimeId.Should().Be(insertedAnime.Id);
				updatedSeiyuu.Role.First().SeiyuuId.Should().Be(seiyuu.Id);
				updatedSeiyuu.Role.First().CharacterId.Should().Be(character.Id);

				insertedAnime.MalId.Should().Be(animeMalId);
				insertedAnime.ImageUrl.Should().BeEmpty();

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(animeMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(It.IsAny<long>()), Times.Never);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSeiyuuWithSingleNotInsertedRoleWithNotInsertedAnimeWithManyTitleSynonymsAndInsertedCharacter_ShouldInsertNewRoleAndNewAnime()
		{
			// Given
			var seiyuuId = Guid.NewGuid();
			const long seiyuuMalId = 1;
			const long animeMalId = 100;
			const long characterMalId = 1000;
			var dbContext = InMemoryDbProvider.GetDbContext();

			var returnedSeiyuu = new Person
			{
				MalId = seiyuuMalId
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
			
			var returnedAnime = new JikanDotNet.Anime
			{
				TitleSynonyms = new List<string> { "Synonym 1", "Synonym 2", "Synonym 3" }
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu, returnedRoles).WithAnimeReturned(returnedAnime);

			var seiyuu = new SeiyuuBuilder()
				.WithId(seiyuuId)
				.WithMalId(seiyuuMalId)
				.WithName("PreUpdateSeiyuuName")
				.WithAbout("PreUpdateSeiyuuAbout")
				.WithJapaneseName("PreUpdateSeiyuuName")
				.WithImageUrl("PreUpdateSeiyuuImage")
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
			await dbContext.AddAsync(character);
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
			var insertedAnime = await dbContext.Animes.FirstAsync(x => x.MalId == animeMalId);
			using (new AssertionScope())
			{
				dbContext.AnimeRoles.Should().ContainSingle();
				dbContext.AnimeCharacters.Should().ContainSingle();

				updatedSeiyuu.Role.First().AnimeId.Should().Be(insertedAnime.Id);
				updatedSeiyuu.Role.First().SeiyuuId.Should().Be(seiyuu.Id);
				updatedSeiyuu.Role.First().CharacterId.Should().Be(character.Id);

				insertedAnime.MalId.Should().Be(animeMalId);
				insertedAnime.TitleSynonyms.Should().Be("Synonym 1;Synonym 2;Synonym 3");

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(animeMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(It.IsAny<long>()), Times.Never);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSeiyuuWithSingleNotInsertedRoleWithNotInsertedAnimeWithUnknownTypeAndStatusAndInsertedCharacter_ShouldInsertNewRoleAndNewAnimeWithoutStatusAndType()
		{
			// Given
			var seiyuuId = Guid.NewGuid();
			const long seiyuuMalId = 1;
			const long animeMalId = 100;
			const long characterMalId = 1000;
			var dbContext = InMemoryDbProvider.GetDbContext();

			var returnedSeiyuu = new Person
			{
				MalId = seiyuuMalId
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
			
			var returnedAnime = new JikanDotNet.Anime
			{
				Type = "test type",
				Status = "test status"
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu, returnedRoles).WithAnimeReturned(returnedAnime);

			var seiyuu = new SeiyuuBuilder()
				.WithId(seiyuuId)
				.WithMalId(seiyuuMalId)
				.WithName("PreUpdateSeiyuuName")
				.WithAbout("PreUpdateSeiyuuAbout")
				.WithJapaneseName("PreUpdateSeiyuuName")
				.WithImageUrl("PreUpdateSeiyuuImage")
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
			await dbContext.AddAsync(character);
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
			var insertedAnime = await dbContext.Animes.FirstAsync(x => x.MalId == animeMalId);
			using (new AssertionScope())
			{
				dbContext.AnimeRoles.Should().ContainSingle();
				dbContext.AnimeCharacters.Should().ContainSingle();

				updatedSeiyuu.Role.First().AnimeId.Should().Be(insertedAnime.Id);
				updatedSeiyuu.Role.First().SeiyuuId.Should().Be(seiyuu.Id);
				updatedSeiyuu.Role.First().CharacterId.Should().Be(character.Id);

				insertedAnime.MalId.Should().Be(animeMalId);
				insertedAnime.StatusId.Should().BeNull();
				insertedAnime.TypeId.Should().BeNull();

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(animeMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(It.IsAny<long>()), Times.Never);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSeiyuuWithSingleNotInsertedRoleWithNotInsertedAnimeWithSeasonNotInDbAndInsertedCharacter_ShouldInsertNewRoleAndNewAnimeWithoutSeasonId()
		{
			// Given
			var seiyuuId = Guid.NewGuid();
			const long seiyuuMalId = 1;
			const long animeMalId = 100;
			const long characterMalId = 1000;
			var dbContext = InMemoryDbProvider.GetDbContext();

			var returnedSeiyuu = new Person
			{
				MalId = seiyuuMalId
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
			
			var returnedAnime = new JikanDotNet.Anime
			{
				Season = Season.Winter,
				Year = 2001
			};

			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(returnedSeiyuu, returnedRoles).WithAnimeReturned(returnedAnime);

			var seiyuu = new SeiyuuBuilder()
				.WithId(seiyuuId)
				.WithMalId(seiyuuMalId)
				.WithName("PreUpdateSeiyuuName")
				.WithAbout("PreUpdateSeiyuuAbout")
				.WithJapaneseName("PreUpdateSeiyuuName")
				.WithImageUrl("PreUpdateSeiyuuImage")
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

			var season = new SeasonBuilder()
				.WithId(10)
				.WithYear(2000)
				.WithName("Winter")
				.Build();

			await dbContext.AddAsync(seiyuu);
			await dbContext.AddAsync(character);
			await dbContext.AddAsync(season);
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
			var insertedAnime = await dbContext.Animes.FirstAsync(x => x.MalId == animeMalId);
			using (new AssertionScope())
			{
				dbContext.AnimeRoles.Should().ContainSingle();
				dbContext.AnimeCharacters.Should().ContainSingle();

				updatedSeiyuu.Role.First().AnimeId.Should().Be(insertedAnime.Id);
				updatedSeiyuu.Role.First().SeiyuuId.Should().Be(seiyuu.Id);
				updatedSeiyuu.Role.First().CharacterId.Should().Be(character.Id);

				insertedAnime.MalId.Should().Be(animeMalId);
				insertedAnime.SeasonId.Should().BeNull();

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(animeMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(It.IsAny<long>()), Times.Never);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSeiyuuWithSingleNotInsertedRoleWithNotInsertedAnimeWAndNotInsertedCharacter_ShouldInsertNewRoleAndNewCharacterAndNewAnime()
		{
			// Given
			var seiyuuId = Guid.NewGuid();
			const long seiyuuMalId = 1;
			const long animeMalId = 100;
			const long characterMalId = 1000;
			var dbContext = InMemoryDbProvider.GetDbContext();

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
				MalId = seiyuuMalId
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
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = returnedCharacterImageUrl }
				},
				Nicknames = new List<string>(),
				Favorites = returnedCharacterPopularity
			};

			var returnedAnime = new JikanDotNet.Anime
			{
				Title = returnedAnimeTitle,
				Synopsis = returnedAnimeAbout,
				TitleEnglish = returnedAnimeEnglishTitle,
				TitleJapanese = returnedAnimeJapaneseTitle,
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = returnedAnimeImageUrl }
				},
				TitleSynonyms = new List<string>(),
				Members = returnedAnimePopularity
			};

			var jikanServiceBuilder = new JikanServiceBuilder()
				.WithPersonReturned(returnedSeiyuu, returnedRoles)
				.WithCharacterReturned(returnedCharacter)
				.WithAnimeReturned(returnedAnime);

			var seiyuu = new SeiyuuBuilder()
				.WithId(seiyuuId)
				.WithMalId(seiyuuMalId)
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
				Id = seiyuuId,
				MalId = seiyuuMalId
			};

			// When
			await handler.HandleAsync(command);

			// Then
			var updatedSeiyuu = await dbContext.Seiyuus.Include(x => x.Role).FirstAsync(x => x.MalId == seiyuuMalId);
			var insertedCharacter = await dbContext.AnimeCharacters.FirstAsync(x => x.MalId == characterMalId);
			var insertedAnime = await dbContext.Animes.FirstAsync(x => x.MalId == animeMalId);
			using (new AssertionScope())
			{
				dbContext.AnimeRoles.Should().ContainSingle();
				dbContext.AnimeCharacters.Should().ContainSingle();
				dbContext.Animes.Should().ContainSingle();

				updatedSeiyuu.Role.First().AnimeId.Should().Be(insertedAnime.Id);
				updatedSeiyuu.Role.First().SeiyuuId.Should().Be(seiyuu.Id);
				updatedSeiyuu.Role.First().CharacterId.Should().Be(insertedCharacter.Id);

				insertedCharacter.MalId.Should().Be(characterMalId);
				insertedCharacter.Name.Should().Be(returnedCharacterName);
				insertedCharacter.About.Should().Be(returnedCharacterAbout);
				insertedCharacter.KanjiName.Should().Be(returnedCharacterJapaneseName);
				insertedCharacter.Popularity.Should().Be(returnedCharacterPopularity);
				insertedCharacter.ImageUrl.Should().Be(returnedCharacterImageUrl);

				insertedAnime.MalId.Should().Be(animeMalId);
				insertedAnime.About.Should().Be(returnedAnimeAbout);
				insertedAnime.Title.Should().Be(returnedAnimeTitle);
				insertedAnime.EnglishTitle.Should().Be(returnedAnimeEnglishTitle);
				insertedAnime.KanjiTitle.Should().Be(returnedAnimeJapaneseTitle);
				insertedAnime.Popularity.Should().Be(returnedAnimePopularity);
				insertedAnime.ImageUrl.Should().Be(returnedAnimeImageUrl);

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(animeMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(characterMalId), Times.Once);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSeiyuuWithSingleNotInsertedRoleWithNotInsertedAnimeWAndNotInsertedCharacterWithNullFromAnimeRequest_ShouldInsertOnlyNewCharacter()
		{
			// Given
			var seiyuuId = Guid.NewGuid();
			const long seiyuuMalId = 1;
			const long animeMalId = 100;
			const long characterMalId = 1000;
			var dbContext = InMemoryDbProvider.GetDbContext();

			const string returnedCharacterName = "PostUpdateName";
			const string returnedCharacterAbout = "PostUpdateAbout";
			const string returnedCharacterJapaneseName = "PostUpdateJapanese";
			const string returnedCharacterImageUrl = "PostUpdateImageUrl";
			const int returnedCharacterPopularity = 1;
			var returnedSeiyuu = new Person
			{
				MalId = seiyuuMalId
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
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = returnedCharacterImageUrl }
				},
				Nicknames = new List<string>(),
				Favorites = returnedCharacterPopularity
			};

			var jikanServiceBuilder = new JikanServiceBuilder()
				.WithPersonReturned(returnedSeiyuu, returnedRoles)
				.WithCharacterReturned(returnedCharacter)
				.WithAnimeReturned(null);

			var seiyuu = new SeiyuuBuilder()
				.WithId(seiyuuId)
				.WithMalId(seiyuuMalId)
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
				Id = seiyuuId,
				MalId = seiyuuMalId
			};

			// When
			await handler.HandleAsync(command);

			// Then
			var updatedSeiyuu = await dbContext.Seiyuus.Include(x => x.Role).FirstAsync(x => x.MalId == seiyuuMalId);
			var insertedCharacter = await dbContext.AnimeCharacters.FirstAsync(x => x.MalId == characterMalId);
			using (new AssertionScope())
			{
				dbContext.AnimeRoles.Should().BeEmpty();
				dbContext.AnimeCharacters.Should().ContainSingle();
				dbContext.Animes.Should().BeEmpty();

				updatedSeiyuu.Role.Should().BeEmpty();

				insertedCharacter.MalId.Should().Be(characterMalId);
				insertedCharacter.Name.Should().Be(returnedCharacterName);
				insertedCharacter.About.Should().Be(returnedCharacterAbout);
				insertedCharacter.KanjiName.Should().Be(returnedCharacterJapaneseName);
				insertedCharacter.Popularity.Should().Be(returnedCharacterPopularity);
				insertedCharacter.ImageUrl.Should().Be(returnedCharacterImageUrl);

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(animeMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(characterMalId), Times.Once);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSeiyuuWithSingleNotInsertedRoleWithNotInsertedAnimeWAndNotInsertedCharacterWithNullFromCharacterRequest_ShouldInsertOnlyNewAnime()
		{
			// Given
			var seiyuuId = Guid.NewGuid();
			const long seiyuuMalId = 1;
			const long animeMalId = 100;
			const long characterMalId = 1000;
			var dbContext = InMemoryDbProvider.GetDbContext();

			const string returnedAnimeTitle = "PostUpdateTitle";
			const string returnedAnimeAbout = "PostUpdateAbout";
			const string returnedAnimeEnglishTitle = "PostUpdateEnglish";
			const string returnedAnimeJapaneseTitle = "PostUpdateJapanese";
			const string returnedAnimeImageUrl = "PostUpdateImageUrl";
			const int returnedAnimePopularity = 3;

			var returnedSeiyuu = new Person
			{
				MalId = seiyuuMalId
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
			
			var returnedAnime = new JikanDotNet.Anime
			{
				Title = returnedAnimeTitle,
				Synopsis = returnedAnimeAbout,
				TitleEnglish = returnedAnimeEnglishTitle,
				TitleJapanese = returnedAnimeJapaneseTitle,
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = returnedAnimeImageUrl }
				},
				TitleSynonyms = new List<string>(),
				Members = returnedAnimePopularity
			};

			var jikanServiceBuilder = new JikanServiceBuilder()
				.WithPersonReturned(returnedSeiyuu, returnedRoles)
				.WithCharacterReturned(null)
				.WithAnimeReturned(returnedAnime);

			var seiyuu = new SeiyuuBuilder()
				.WithId(seiyuuId)
				.WithMalId(seiyuuMalId)
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
				Id = seiyuuId,
				MalId = seiyuuMalId
			};

			// When
			await handler.HandleAsync(command);

			// Then
			var updatedSeiyuu = await dbContext.Seiyuus.Include(x => x.Role).FirstAsync(x => x.MalId == seiyuuMalId);
			var insertedAnime = await dbContext.Animes.FirstAsync(x => x.MalId == animeMalId);
			using (new AssertionScope())
			{
				dbContext.AnimeRoles.Should().BeEmpty();
				dbContext.AnimeCharacters.Should().BeEmpty();
				dbContext.Animes.Should().ContainSingle();

				updatedSeiyuu.Role.Should().BeEmpty();

				insertedAnime.MalId.Should().Be(animeMalId);
				insertedAnime.About.Should().Be(returnedAnimeAbout);
				insertedAnime.Title.Should().Be(returnedAnimeTitle);
				insertedAnime.EnglishTitle.Should().Be(returnedAnimeEnglishTitle);
				insertedAnime.KanjiTitle.Should().Be(returnedAnimeJapaneseTitle);
				insertedAnime.Popularity.Should().Be(returnedAnimePopularity);
				insertedAnime.ImageUrl.Should().Be(returnedAnimeImageUrl);

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(animeMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(characterMalId), Times.Once);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSeiyuuWithMultipleNotInsertedRoleWithInsertedAnimeAndNotInsertedCharacter_ShouldInsertMultipleNewRolesAndMultipleNewCharacters()
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
				MalId = seiyuuMalId
			};
			var returnedRoles = new List<VoiceActingRole>
			{
				new VoiceActingRole
				{
					Role = "Main",
					Anime = new MalImageSubItem
					{
						MalId = anime1MalId
					},
					Character = new MalImageSubItem()
					{
						MalId = character1MalId
					}
				},
				new VoiceActingRole
				{
					Role = "Supporting",
					Anime = new MalImageSubItem
					{
						MalId = anime2MalId
					},
					Character = new MalImageSubItem()
					{
						MalId = character2MalId
					}
				}
			};
			const string firstReturnedCharacterName = "PostUpdateNameCharacter1";
			const string firstReturnedCharacterAbout = "PostUpdateAboutCharacter1";
			const string firstReturnedCharacterJapaneseName = "PostUpdateJapaneseCharacter1";
			const string firstReturnedCharacterImageUrl = "PostUpdateImageUrlCharacter1";
			const int firstReturnedCharacterPopularity = 30;
			const string secondReturnedCharacterName = "PostUpdateNameCharacter2";
			const string secondReturnedCharacterAbout = "PostUpdateAboutCharacter2";
			const string secondReturnedCharacterJapaneseName = "PostUpdateJapaneseCharacter2";
			const string secondReturnedCharacterImageUrl = "PostUpdateImageUrlCharacter2";
			const int secondReturnedCharacterPopularity = 42;

			var firstReturnedCharacter = new Character
			{
				MalId = character1MalId,
				Name = firstReturnedCharacterName,
				About = firstReturnedCharacterAbout,
				NameKanji = firstReturnedCharacterJapaneseName,
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = firstReturnedCharacterImageUrl }
				},
				Nicknames = new List<string>(),
				Favorites = firstReturnedCharacterPopularity
			};
			var secondReturnedCharacter = new Character
			{
				MalId = character1MalId,
				Name = secondReturnedCharacterName,
				About = secondReturnedCharacterAbout,
				NameKanji = secondReturnedCharacterJapaneseName,
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = secondReturnedCharacterImageUrl }
				},
				Nicknames = new List<string>(),
				Favorites = secondReturnedCharacterPopularity
			};

			var jikanServiceBuilder = new JikanServiceBuilder()
				.WithPersonReturned(returnedSeiyuu, returnedRoles)
				.WithTwoCharactersReturned(firstReturnedCharacter, secondReturnedCharacter);

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

			await dbContext.AddAsync(seiyuu);
			await dbContext.AddRangeAsync(animes);
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
				dbContext.AnimeCharacters.Should().HaveCount(2);

				updatedSeiyuu.Role.First().AnimeId.Should().Be(anime1Id);
				updatedSeiyuu.Role.Skip(1).First().AnimeId.Should().Be(anime2Id);
				updatedSeiyuu.Role.First().CharacterId.Should().NotBeEmpty();
				updatedSeiyuu.Role.Skip(1).First().CharacterId.Should().NotBeEmpty();
				updatedSeiyuu.Role.First().SeiyuuId.Should().Be(seiyuu.Id);
				updatedSeiyuu.Role.Skip(1).First().SeiyuuId.Should().Be(seiyuu.Id);

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(It.IsAny<long>()), Times.Never);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(It.IsAny<long>()), Times.Exactly(2));
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSeiyuuWithMultipleNotInsertedRoleWithNotInsertedAnimeAndInsertedCharacter_ShouldInsertMultipleNewRolesAndMultipleNewAnime()
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
				MalId = seiyuuMalId
			};
			var returnedRoles = new List<VoiceActingRole>
			{
				new VoiceActingRole
				{
					Role = "Main",
					Anime = new MalImageSubItem
					{
						MalId = anime1MalId
					},
					Character = new MalImageSubItem()
					{
						MalId = character1MalId
					}
				},
				new VoiceActingRole
				{
					Role = "Supporting",
					Anime = new MalImageSubItem
					{
						MalId = anime2MalId
					},
					Character = new MalImageSubItem()
					{
						MalId = character2MalId
					}
				}
			};

			const string firstReturnedAnimeName = "PostUpdateNameAnime1";
			const string firstReturnedAnimeAbout = "PostUpdateAboutAnime1";
			const string firstReturnedAnimeJapaneseName = "PostUpdateJapaneseAnime1";
			const string firstReturnedAnimeImageUrl = "PostUpdateImageUrlAnime1";
			const int firstReturnedAnimePopularity = 30;
			const string secondReturnedAnimeName = "PostUpdateNameAnime2";
			const string secondReturnedAnimeAbout = "PostUpdateAboutAnime2";
			const string secondReturnedAnimeJapaneseName = "PostUpdateJapaneseAnime2";
			const string secondReturnedAnimeImageUrl = "PostUpdateImageUrlAnime2";
			const int secondReturnedAnimePopularity = 42;

			var firstReturnedAnime = new JikanDotNet.Anime
			{
				MalId = anime1MalId,
				Title = firstReturnedAnimeName,
				Synopsis = firstReturnedAnimeAbout,
				TitleJapanese = firstReturnedAnimeJapaneseName,
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = firstReturnedAnimeImageUrl }
				},
				TitleSynonyms = new List<string>(),
				Popularity = firstReturnedAnimePopularity
			};
			var secondReturnedAnime = new JikanDotNet.Anime
			{
				MalId = anime2MalId,
				Title = secondReturnedAnimeName,
				Synopsis = secondReturnedAnimeAbout,
				TitleJapanese = secondReturnedAnimeJapaneseName,
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = secondReturnedAnimeImageUrl }
				},
				TitleSynonyms = new List<string>(),
				Popularity = secondReturnedAnimePopularity
			};

			var jikanServiceBuilder = new JikanServiceBuilder()
				.WithPersonReturned(returnedSeiyuu, returnedRoles)
				.WithTwoAnimeReturned(firstReturnedAnime, secondReturnedAnime);

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
			await dbContext.AddRangeAsync(characters);
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
				dbContext.AnimeCharacters.Should().HaveCount(2);
				dbContext.Animes.Should().HaveCount(2);

				updatedSeiyuu.Role.First().AnimeId.Should().NotBeEmpty();
				updatedSeiyuu.Role.Skip(1).First().AnimeId.Should().NotBeEmpty();
				updatedSeiyuu.Role.First().CharacterId.Should().Be(character1Id);
				updatedSeiyuu.Role.Skip(1).First().CharacterId.Should().Be(character2Id);
				updatedSeiyuu.Role.First().SeiyuuId.Should().Be(seiyuu.Id);
				updatedSeiyuu.Role.Skip(1).First().SeiyuuId.Should().Be(seiyuu.Id);

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(It.IsAny<long>()), Times.Exactly(2));
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(It.IsAny<long>()), Times.Never);
			}
		}

		[Fact]
		public async Task HandleAsync_GivenSeiyuuWithMultipleNotInsertedRoleWithNotInsertedAnimeAndNotInsertedCharacter_ShouldInsertMultipleNewRolesAndMultipleNewAnimeAndMultpleNewCharacters()
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
				MalId = seiyuuMalId
			};
			var returnedRoles = new List<VoiceActingRole>
			{
				new VoiceActingRole
				{
					Role = "Main",
					Anime = new MalImageSubItem
					{
						MalId = anime1MalId
					},
					Character = new MalImageSubItem()
					{
						MalId = character1MalId
					}
				},
				new VoiceActingRole
				{
					Role = "Supporting",
					Anime = new MalImageSubItem
					{
						MalId = anime2MalId
					},
					Character = new MalImageSubItem()
					{
						MalId = character2MalId
					}
				}
			};

			const string firstReturnedAnimeName = "PostUpdateNameAnime1";
			const string firstReturnedAnimeAbout = "PostUpdateAboutAnime1";
			const string firstReturnedAnimeJapaneseName = "PostUpdateJapaneseAnime1";
			const string firstReturnedAnimeImageUrl = "PostUpdateImageUrlAnime1";
			const int firstReturnedAnimePopularity = 30;
			const string secondReturnedAnimeName = "PostUpdateNameAnime2";
			const string secondReturnedAnimeAbout = "PostUpdateAboutAnime2";
			const string secondReturnedAnimeJapaneseName = "PostUpdateJapaneseAnime2";
			const string secondReturnedAnimeImageUrl = "PostUpdateImageUrlAnime2";
			const int secondReturnedAnimePopularity = 42;

			var firstReturnedAnime = new JikanDotNet.Anime
			{
				MalId = anime1MalId,
				Title = firstReturnedAnimeName,
				Synopsis = firstReturnedAnimeAbout,
				TitleJapanese = firstReturnedAnimeJapaneseName,
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = firstReturnedAnimeImageUrl }
				},
				TitleSynonyms = new List<string>(),
				Popularity = firstReturnedAnimePopularity
			};
			var secondReturnedAnime = new JikanDotNet.Anime
			{
				MalId = anime2MalId,
				Title = secondReturnedAnimeName,
				Synopsis = secondReturnedAnimeAbout,
				TitleJapanese = secondReturnedAnimeJapaneseName,
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = secondReturnedAnimeImageUrl }
				},
				TitleSynonyms = new List<string>(),
				Popularity = secondReturnedAnimePopularity
			};

			const string firstReturnedCharacterName = "PostUpdateNameCharacter1";
			const string firstReturnedCharacterAbout = "PostUpdateAboutCharacter1";
			const string firstReturnedCharacterJapaneseName = "PostUpdateJapaneseCharacter1";
			const string firstReturnedCharacterImageUrl = "PostUpdateImageUrlCharacter1";
			const int firstReturnedCharacterPopularity = 30;
			const string secondReturnedCharacterName = "PostUpdateNameCharacter2";
			const string secondReturnedCharacterAbout = "PostUpdateAboutCharacter2";
			const string secondReturnedCharacterJapaneseName = "PostUpdateJapaneseCharacter2";
			const string secondReturnedCharacterImageUrl = "PostUpdateImageUrlCharacter2";
			const int secondReturnedCharacterPopularity = 42;

			var firstReturnedCharacter = new Character
			{
				MalId = character1MalId,
				Name = firstReturnedCharacterName,
				About = firstReturnedCharacterAbout,
				NameKanji = firstReturnedCharacterJapaneseName,
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = firstReturnedCharacterImageUrl }
				},
				Nicknames = new List<string>(),
				Favorites = firstReturnedCharacterPopularity
			};
			var secondReturnedCharacter = new Character
			{
				MalId = character1MalId,
				Name = secondReturnedCharacterName,
				About = secondReturnedCharacterAbout,
				NameKanji = secondReturnedCharacterJapaneseName,
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = secondReturnedCharacterImageUrl }
				},
				Nicknames = new List<string>(),
				Favorites = secondReturnedCharacterPopularity
			};

			var jikanServiceBuilder = new JikanServiceBuilder()
				.WithPersonReturned(returnedSeiyuu, returnedRoles)
				.WithTwoAnimeReturned(firstReturnedAnime, secondReturnedAnime)
				.WithTwoCharactersReturned(firstReturnedCharacter, secondReturnedCharacter);

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
			await dbContext.AddRangeAsync(characters);
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
				dbContext.AnimeCharacters.Should().HaveCount(2);
				dbContext.Animes.Should().HaveCount(2);

				updatedSeiyuu.Role.First().AnimeId.Should().NotBeEmpty();
				updatedSeiyuu.Role.Skip(1).First().AnimeId.Should().NotBeEmpty();
				updatedSeiyuu.Role.First().CharacterId.Should().NotBeEmpty();
				updatedSeiyuu.Role.Skip(1).First().CharacterId.Should().NotBeEmpty();
				updatedSeiyuu.Role.First().SeiyuuId.Should().Be(seiyuu.Id);
				updatedSeiyuu.Role.Skip(1).First().SeiyuuId.Should().Be(seiyuu.Id);

				jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(seiyuuMalId), Times.Once);
				jikanServiceBuilder.JikanClient.Verify(x => x.GetAnimeAsync(It.IsAny<long>()), Times.Exactly(2));
				jikanServiceBuilder.JikanClient.Verify(x => x.GetCharacterAsync(It.IsAny<long>()), Times.Never);
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
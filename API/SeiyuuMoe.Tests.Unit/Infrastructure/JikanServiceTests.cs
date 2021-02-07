using FluentAssertions;
using FluentAssertions.Execution;
using JikanDotNet;
using Moq;
using SeiyuuMoe.Infrastructure.Jikan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Infrastructure
{
	public class JikanServiceTests
	{
		[Fact]
		public async Task GetAnimeDataAsync_GivenNullResponse_ShouldReturnNull()
		{
			// Given
			var mockJikanClient = new Mock<IJikan>();
			var service = new JikanService(mockJikanClient.Object);

			// When
			var result = await service.GetAnimeDataAsync(1);

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetAnimeDataAsync_GivenResponse_ShouldReturnMappedMalAnimeData()
		{
			// Given
			const string returnedAnimeTitle = "PostUpdateTitle";
			const string returnedAnimeAbout = "PostUpdateAbout";
			const string returnedAnimeEnglishTitle = "PostUpdateEnglish";
			const string returnedAnimeJapaneseTitle = "PostUpdateJapanese";
			const string returnedAnimeImageUrl = "PostUpdateImageUrl";
			const string returnedAnimeType = "PostUpdateType";
			const string returnedAnimeStatus = "PostUpdateStatus";
			const int returnedAnimePopularity = 3;
			const int returnedAnimeMalId = 100;
			var returnedPremiereDate = new DateTime(1990, 1, 1);

			var mockJikanClient = new Mock<IJikan>();
			mockJikanClient.Setup(x => x.GetAnime(returnedAnimeMalId)).ReturnsAsync(
				new Anime
				{
					Title = returnedAnimeTitle,
					Synopsis = returnedAnimeAbout,
					TitleEnglish = returnedAnimeEnglishTitle,
					TitleJapanese = returnedAnimeJapaneseTitle,
					ImageURL = returnedAnimeImageUrl,
					Members = returnedAnimePopularity,
					Type = returnedAnimeType,
					Status = returnedAnimeStatus,
					Aired = new TimePeriod { From = returnedPremiereDate }
				}
			);
			var service = new JikanService(mockJikanClient.Object);

			// When
			var result = await service.GetAnimeDataAsync(returnedAnimeMalId);

			// Then
			using (new AssertionScope())
			{
				result.Title.Should().Be(returnedAnimeTitle);
				result.EnglishTitle.Should().Be(returnedAnimeEnglishTitle);
				result.JapaneseTitle.Should().Be(returnedAnimeJapaneseTitle);
				result.ImageUrl.Should().Be(returnedAnimeImageUrl);
				result.Popularity.Should().Be(returnedAnimePopularity);
				result.About.Should().Be(returnedAnimeAbout);
				result.Type.Should().Be(returnedAnimeType);
				result.Status.Should().Be(returnedAnimeStatus);
				result.TitleSynonyms.Should().BeEmpty();
				result.AiringDate.Should().Be(returnedPremiereDate);
			}
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("https://cdn.myanimelist.net/images/questionmark_23.gif")]
		[InlineData("https://cdn.myanimelist.net/img/sp/icon/apple-touch-icon-256.png")]
		public async Task GetAnimeDataAsync_GivenEmptyOrPlaceholderImage_ShouldReturnMalAnimeDataWithEmptyImageUrl(string returnedImageUrl)
		{
			// Given
			const int returnedAnimeMalId = 100;

			var mockJikanClient = new Mock<IJikan>();
			mockJikanClient.Setup(x => x.GetAnime(returnedAnimeMalId)).ReturnsAsync(
				new Anime
				{
					ImageURL = returnedImageUrl
				}
			);
			var service = new JikanService(mockJikanClient.Object);

			// When
			var result = await service.GetAnimeDataAsync(returnedAnimeMalId);

			// Then
			result.ImageUrl.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAnimeDataAsync_GivenResponseWithTitleSynonyms_ShouldReturnMalAnimeDataWithJoinedSynonyms()
		{
			// Given
			var returnedAnimeTitleSynonyms = new List<string> { "Synonym 1", "Synonym 2", "something else" };
			const int returnedAnimeMalId = 100;

			var mockJikanClient = new Mock<IJikan>();
			mockJikanClient.Setup(x => x.GetAnime(returnedAnimeMalId)).ReturnsAsync(
				new Anime
				{
					TitleSynonyms = returnedAnimeTitleSynonyms
				}
			);
			var service = new JikanService(mockJikanClient.Object);

			// When
			var result = await service.GetAnimeDataAsync(returnedAnimeMalId);

			// Then
			result.TitleSynonyms.Should().Be("Synonym 1;Synonym 2;something else");
		}

		[Fact]
		public async Task GetCharacterDataAsync_GivenNullResponse_ShouldReturnNull()
		{
			// Given
			var mockJikanClient = new Mock<IJikan>();
			var service = new JikanService(mockJikanClient.Object);

			// When
			var result = await service.GetCharacterDataAsync(1);

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetCharacterDataAsync_GivenResponse_ShouldReturnMappedMalCharacterData()
		{
			// Given
			const string returnedCharacterName = "CharacterName";
			const string returnedCharacterAbout = "CharacterAbout";
			const string returnedCharacterNameKanji = "CharacterNameKanji";
			const string returnedCharacterImageUrl = "CharacterImageUrl";
			const int returnedCharacterPopularity = 10;
			const int returnedCharacterMalId = 100;

			var mockJikanClient = new Mock<IJikan>();
			mockJikanClient.Setup(x => x.GetCharacter(returnedCharacterMalId)).ReturnsAsync(
				new Character
				{
					Name = returnedCharacterName,
					About = returnedCharacterAbout,
					NameKanji = returnedCharacterNameKanji,
					MemberFavorites = returnedCharacterPopularity,
					ImageURL = returnedCharacterImageUrl
				}
			);
			var service = new JikanService(mockJikanClient.Object);

			// When
			var result = await service.GetCharacterDataAsync(returnedCharacterMalId);

			// Then
			using (new AssertionScope())
			{
				result.Name.Should().Be(returnedCharacterName);
				result.JapaneseName.Should().Be(returnedCharacterNameKanji);
				result.ImageUrl.Should().Be(returnedCharacterImageUrl);
				result.Popularity.Should().Be(returnedCharacterPopularity);
				result.About.Should().Be(returnedCharacterAbout);
			}
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("https://cdn.myanimelist.net/images/questionmark_23.gif")]
		[InlineData("https://cdn.myanimelist.net/img/sp/icon/apple-touch-icon-256.png")]
		public async Task GetCharacterDataAsync_GivenEmptyOrPlaceholderImage_ShouldReturnMalCharacterDataWithEmptyImageUrl(string returnedImageUrl)
		{
			// Given
			const int returnedCharacterMalId = 100;

			var mockJikanClient = new Mock<IJikan>();
			mockJikanClient.Setup(x => x.GetCharacter(returnedCharacterMalId)).ReturnsAsync(
				new Character
				{
					ImageURL = returnedImageUrl
				}
			);
			var service = new JikanService(mockJikanClient.Object);

			// When
			var result = await service.GetCharacterDataAsync(returnedCharacterMalId);

			// Then
			result.ImageUrl.Should().BeEmpty();
		}

		[Fact]
		public async Task GetCharacterDataAsync_GivenResponseWithNicknames_ShouldReturnMappedMalCharacterDataWithJoinedNcknames()
		{
			// Given
			var returnedNicknames = new List<string> { "Nickname 1", "Nick 2", "something else" };
			const int returnedCharacterMalId = 100;

			var mockJikanClient = new Mock<IJikan>();
			mockJikanClient.Setup(x => x.GetCharacter(returnedCharacterMalId)).ReturnsAsync(
				new Character
				{
					Nicknames = returnedNicknames
				}
			);
			var service = new JikanService(mockJikanClient.Object);

			// When
			var result = await service.GetCharacterDataAsync(returnedCharacterMalId);

			// Then
			result.Nicknames.Should().Be("Nickname 1;Nick 2;something else");
		}

		[Fact]
		public async Task GetSeasonDataAsync_GivenNullResponse_ShouldReturnNull()
		{
			// Given
			var mockJikanClient = new Mock<IJikan>();
			var service = new JikanService(mockJikanClient.Object);

			// When
			var result = await service.GetSeasonDataAsync();

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetSeasonDataAsync_GivenSingleSeason_ShouldReturnIt()
		{
			// Given
			const int year = 2020;
			const Seasons season = Seasons.Winter;
			var response = new SeasonArchives
			{
				Archives = new List<SeasonArchive>
				{
					new SeasonArchive
					{
						Year = year,
						Season = new List<Seasons>
						{
							season
						}
					}
				}
			};

			var mockJikanClient = new Mock<IJikan>();
			mockJikanClient.Setup(x => x.GetSeasonArchive()).ReturnsAsync(response);
			var service = new JikanService(mockJikanClient.Object);

			// When
			var result = await service.GetSeasonDataAsync();

			// Then
			using (new AssertionScope())
			{
				result.NewestSeasonName.Should().Be(season.ToString());
				result.NewestSeasonYear.Should().Be(year);
			}
		}

		[Fact]
		public async Task GetSeasonDataAsync_GivenMultipleSeasons_ShouldReturnLatest()
		{
			// Given
			const int year = 2020;
			var response = new SeasonArchives
			{
				Archives = new List<SeasonArchive>
				{
					new SeasonArchive
					{
						Year = year,
						Season = new List<Seasons>
						{
							Seasons.Winter,
							Seasons.Spring,
							Seasons.Summer
						}
					}
				}
			};

			var mockJikanClient = new Mock<IJikan>();
			mockJikanClient.Setup(x => x.GetSeasonArchive()).ReturnsAsync(response);
			var service = new JikanService(mockJikanClient.Object);

			// When
			var result = await service.GetSeasonDataAsync();

			// Then
			using (new AssertionScope())
			{
				result.NewestSeasonName.Should().Be(Seasons.Summer.ToString());
				result.NewestSeasonYear.Should().Be(year);
			}
		}

		[Fact]
		public async Task GetSeasonDataAsync_GivenMultipleYears_ShouldReturnNewest()
		{
			// Given
			var response = new SeasonArchives
			{
				Archives = new List<SeasonArchive>
				{
					new SeasonArchive
					{
						Year = 2020,
						Season = new List<Seasons>
						{
							Seasons.Winter
						}
					},
					new SeasonArchive
					{
						Year = 2019,
						Season = new List<Seasons>
						{
							Seasons.Winter,
							Seasons.Spring,
							Seasons.Summer,
							Seasons.Fall
						}
					}
				}
			};

			var mockJikanClient = new Mock<IJikan>();
			mockJikanClient.Setup(x => x.GetSeasonArchive()).ReturnsAsync(response);
			var service = new JikanService(mockJikanClient.Object);

			// When
			var result = await service.GetSeasonDataAsync();

			// Then
			using (new AssertionScope())
			{
				result.NewestSeasonName.Should().Be(Seasons.Winter.ToString());
				result.NewestSeasonYear.Should().Be(2020);
			}
		}

		[Fact]
		public async Task GetSeiyuuDataAsync_GivenNullResponse_ShouldReturnNull()
		{
			// Given
			var mockJikanClient = new Mock<IJikan>();
			var service = new JikanService(mockJikanClient.Object);

			// When
			var result = await service.GetSeiyuuDataAsync(1);

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetSeiyuuDataAsync_GivenResponse_ShouldReturnMappedMalAnimeData()
		{
			// Given
			const string returnedSeiyuuName = "SeiyuuName";
			const string returnedSeiyuuAbout = "SeiyuuAbout";
			const string returnedSeiyuuImageUrl = "SeiyuuImageUrl";
			const int returnedSeiyuuPopularity = 3;
			const int returnedSeiyuuMalId = 100;

			var mockJikanClient = new Mock<IJikan>();
			mockJikanClient.Setup(x => x.GetPerson(returnedSeiyuuMalId)).ReturnsAsync(
				new Person
				{
					Name = returnedSeiyuuName,
					More = returnedSeiyuuAbout,
					ImageURL = returnedSeiyuuImageUrl,
					MemberFavorites = returnedSeiyuuPopularity
				}
			);
			var service = new JikanService(mockJikanClient.Object);

			// When
			var result = await service.GetSeiyuuDataAsync(returnedSeiyuuMalId);

			// Then
			using (new AssertionScope())
			{
				result.Name.Should().Be(returnedSeiyuuName);
				result.About.Should().Be(returnedSeiyuuAbout);
				result.ImageUrl.Should().Be(returnedSeiyuuImageUrl);
				result.Popularity.Should().Be(returnedSeiyuuPopularity);
			}
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("https://cdn.myanimelist.net/images/questionmark_23.gif")]
		[InlineData("https://cdn.myanimelist.net/img/sp/icon/apple-touch-icon-256.png")]
		public async Task GetSeiyuuDataAsync_GivenEmptyOrPlaceholderImage_ShouldReturnMalSeiyuuDataWithEmptyImageUrl(string returnedImageUrl)
		{
			// Given
			const int returnedCharacterMalId = 100;

			var mockJikanClient = new Mock<IJikan>();
			mockJikanClient.Setup(x => x.GetPerson(returnedCharacterMalId)).ReturnsAsync(
				new Person
				{
					ImageURL = returnedImageUrl
				}
			);
			var service = new JikanService(mockJikanClient.Object);

			// When
			var result = await service.GetSeiyuuDataAsync(returnedCharacterMalId);

			// Then
			result.ImageUrl.Should().BeEmpty();
		}

		[Theory]
		[InlineData("", "", "")]
		[InlineData(null, null, "")]
		[InlineData("", null, "")]
		[InlineData(null, "", "")]
		[InlineData("family", "given", "family given")]
		[InlineData("family", "", "family")]
		[InlineData("", "given", "given")]
		[InlineData(" family", "given ", "family given")]
		[InlineData("family ", " given", "family   given")]
		public async Task GetSeiyuuDataAsync_GivenResponseWithGivenAndFamilyName_ShouldReturnMappedMalAnimeData(string familyName, string givenName, string expectedMappedName)
		{
			// Given
			const int returnedSeiyuuMalId = 100;

			var mockJikanClient = new Mock<IJikan>();
			mockJikanClient.Setup(x => x.GetPerson(returnedSeiyuuMalId)).ReturnsAsync(
				new Person
				{
					FamilyName = familyName,
					GivenName = givenName
				}
			);
			var service = new JikanService(mockJikanClient.Object);

			// When
			var result = await service.GetSeiyuuDataAsync(returnedSeiyuuMalId);

			// Then
			using (new AssertionScope())
			{
				result.JapaneseName.Should().Be(expectedMappedName);
			}
		}



		[Fact]
		public async Task GetSeiyuuDataAsync_GivenResponseWithVoiceRoles_ShouldReturnMappedMalAnimeDataWithVoiceRoles()
		{
			// Given
			const string returnedRoleType = "Main";
			const int returnedRoleAnimeMalId = 200;
			const int returnedRoleCharacterMalId = 300;
			const int returnedSeiyuuMalId = 100;

			var mockJikanClient = new Mock<IJikan>();
			mockJikanClient.Setup(x => x.GetPerson(returnedSeiyuuMalId)).ReturnsAsync(
				new Person
				{
					VoiceActingRoles = new List<VoiceActingRole>
					{
						new VoiceActingRole
						{
							Anime = new MALImageSubItem
							{
								MalId = returnedRoleAnimeMalId
							},
							Character = new MALImageSubItem
							{
								MalId = returnedRoleCharacterMalId
							},
							Role = returnedRoleType
						}
					}
				}
			);
			var service = new JikanService(mockJikanClient.Object);

			// When
			var result = await service.GetSeiyuuDataAsync(returnedSeiyuuMalId);

			// Then
			using (new AssertionScope())
			{
				result.VoiceActingRoles.Should().ContainSingle();

				result.VoiceActingRoles.First().RoleType.Should().Be(returnedRoleType);
				result.VoiceActingRoles.First().AnimeMalId.Should().Be(returnedRoleAnimeMalId);
				result.VoiceActingRoles.First().CharacterMaId.Should().Be(returnedRoleCharacterMalId);
			}
		}
	}
}
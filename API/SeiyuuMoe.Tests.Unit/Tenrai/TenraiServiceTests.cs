using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using SeiyuuMoe.Tests.Common.Builders.Tenrai;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tenrai;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tenrai
{
	public class TenraiServiceTests
	{
		[Fact]
		public async Task GetSeiyuuFullDataAsync_GivenNullResponse_ShouldReturnNull()
		{
			// Given
			var tenraiService = new TenraiServiceBuilder()
				.WithPersonReturned(null, null)
				.Build();

			// When
			var result = await tenraiService.GetSeiyuuFullDataAsync(1);

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetSeiyuuFullDataAsync_GivenPersonData_ShouldMapSeiyuuFields()
		{
			// Given
			const string name = "Tomokazu Seki";
			const string about = "Hometown: Tokyo, Japan";
			const string givenName = "智一";
			const string familyName = "関";
			const string imageUrl = "https://cdn.myanimelist.net/images/person.jpg";
			const int popularity = 5000;
			var birthday = new DateTime(1972, 9, 8);

			var person = new Person
			{
				Name = name,
				About = about,
				GivenName = givenName,
				FamilyName = familyName,
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = imageUrl }
				},
				MemberFavorites = popularity,
				Birthday = birthday
			};

			var tenraiService = new TenraiServiceBuilder()
				.WithPersonReturned(person, null)
				.Build();

			// When
			var result = await tenraiService.GetSeiyuuFullDataAsync(1);

			// Then
			using (new AssertionScope())
			{
				result.Should().NotBeNull();
				result.SeiyuuData.Name.Should().Be(name);
				result.SeiyuuData.About.Should().Be(about);
				result.SeiyuuData.JapaneseName.Should().Be($"{familyName} {givenName}");
				result.SeiyuuData.ImageUrl.Should().Be(imageUrl);
				result.SeiyuuData.Popularity.Should().Be(popularity);
				result.SeiyuuData.Birthday.Should().Be(birthday);
				result.VoiceActingRoles.Should().BeEmpty();
			}
		}

		[Fact]
		public async Task GetSeiyuuFullDataAsync_GivenVoiceActingRoles_ShouldMapRoles()
		{
			// Given
			const long animeMalId = 100;
			const long characterMalId = 1000;

			var person = new Person
			{
				Name = "Test Seiyuu",
				GivenName = "花澤",
				FamilyName = "香菜"
			};

			var voiceActingRoles = new List<VoiceActingRole>
			{
				new VoiceActingRole
				{
					Role = "Main",
					Anime = new MalImageSubItem { MalId = animeMalId },
					Character = new MalImageSubItem { MalId = characterMalId }
				}
			};

			var tenraiService = new TenraiServiceBuilder()
				.WithPersonReturned(person, voiceActingRoles)
				.Build();

			// When
			var result = await tenraiService.GetSeiyuuFullDataAsync(1);

			// Then
			using (new AssertionScope())
			{
				result.Should().NotBeNull();
				result.VoiceActingRoles.Should().ContainSingle();
				var role = result.VoiceActingRoles.Should().ContainSingle().Subject;
				role.AnimeMalId.Should().Be(animeMalId);
				role.CharacterMaId.Should().Be(characterMalId);
				role.RoleType.Should().Be("Main");
			}
		}

		[Fact]
		public async Task GetSeiyuuFullDataAsync_ShouldCallGetPersonFullDataAsyncOnce()
		{
			// Given
			const long malId = 42;
			var tenraiServiceBuilder = new TenraiServiceBuilder().WithPersonReturned(new Person { Name = "Test" }, null);
			var tenraiService = tenraiServiceBuilder.Build();

			// When
			await tenraiService.GetSeiyuuFullDataAsync(malId);

			// Then
			await tenraiServiceBuilder.TenraiClient.Received(1).GetPersonFullDataAsync(malId);
			await tenraiServiceBuilder.TenraiClient.DidNotReceive().GetPersonAsync(Arg.Any<long>());
			await tenraiServiceBuilder.TenraiClient.DidNotReceive().GetPersonVoiceActingRolesAsync(Arg.Any<long>());
		}
	}
}

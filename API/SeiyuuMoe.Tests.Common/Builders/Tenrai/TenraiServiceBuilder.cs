using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SeiyuuMoe.Infrastructure.Tenrai;
using System.Collections.Generic;
using Tenrai;
using Tenrai.Exceptions;

namespace SeiyuuMoe.Tests.Common.Builders.Tenrai
{
	public class TenraiServiceBuilder
	{
		public readonly ITenrai TenraiClient = Substitute.For<ITenrai>();

		public TenraiService Build() => new TenraiService(TenraiClient);

		public TenraiServiceBuilder WithAnimeReturned(Anime anime)
		{
			TenraiClient.GetAnimeAsync(Arg.Any<long>())
				.Returns(new BaseTenraiResponse<Anime>() { Data = anime });
			return this;
		}

		public TenraiServiceBuilder WithTwoAnimeReturned(Anime firstAnime, Anime secondAnime)
		{
			TenraiClient.GetAnimeAsync(Arg.Any<long>())
				.Returns(
					new BaseTenraiResponse<Anime>() { Data = firstAnime },
					new BaseTenraiResponse<Anime>() { Data = secondAnime });
			return this;
		}

		public TenraiServiceBuilder WithGetAnimeThrowing()
		{
			TenraiClient.GetAnimeAsync(Arg.Any<long>())
				.ThrowsAsync(new TenraiRequestException());
			return this;
		}

		public TenraiServiceBuilder WithCharacterReturned(Character character)
		{
			TenraiClient.GetCharacterAsync(Arg.Any<long>())
				.Returns(new BaseTenraiResponse<Character>() { Data = character });
			return this;
		}

		public TenraiServiceBuilder WithTwoCharactersReturned(Character firstCharacter, Character secondCharacter)
		{
			TenraiClient.GetCharacterAsync(Arg.Any<long>())
				.Returns(
					new BaseTenraiResponse<Character>() { Data = firstCharacter },
					new BaseTenraiResponse<Character>() { Data = secondCharacter });
			return this;
		}

		public TenraiServiceBuilder WithGetCharacterThrowing()
		{
			TenraiClient.GetCharacterAsync(Arg.Any<long>())
				.ThrowsAsync(new TenraiRequestException());
			return this;
		}

		public TenraiServiceBuilder WithPersonReturned(Person person, ICollection<VoiceActingRole> voiceActingRoles)
		{
			TenraiClient.GetPersonFullDataAsync(Arg.Any<long>())
				.Returns(new BaseTenraiResponse<PersonFull>() { Data = CreatePersonFull(person, voiceActingRoles) });
			return this;
		}

		public TenraiServiceBuilder WithGetPersonThrowing()
		{
			TenraiClient.GetPersonFullDataAsync(Arg.Any<long>())
				.ThrowsAsync(new TenraiRequestException());
			return this;
		}

		public TenraiServiceBuilder WithTwoPersonsReturned(Person firstPerson, Person secondPerson)
		{
			TenraiClient.GetPersonFullDataAsync(Arg.Any<long>())
				.Returns(
					new BaseTenraiResponse<PersonFull>() { Data = CreatePersonFull(firstPerson, null) },
					new BaseTenraiResponse<PersonFull>() { Data = CreatePersonFull(secondPerson, null) });
			return this;
		}

		public TenraiServiceBuilder WithLastSeasonArchiveReturned(SeasonArchive seasonArchive)
		{
			TenraiClient.GetSeasonArchiveAsync()
				.Returns(new PaginatedTenraiResponse<ICollection<SeasonArchive>>() { Data = new List<SeasonArchive> { seasonArchive } });
			return this;
		}

		public TenraiServiceBuilder WithGetSeasonArchiveThrowing()
		{
			TenraiClient.GetSeasonArchiveAsync()
				.ThrowsAsync(new TenraiRequestException());
			return this;
		}

		private static PersonFull CreatePersonFull(Person person, ICollection<VoiceActingRole> voiceActingRoles)
		{
			if (person is null)
			{
				return null;
			}

			return new PersonFull
			{
				MalId = person.MalId,
				Url = person.Url,
				Name = person.Name,
				GivenName = person.GivenName,
				FamilyName = person.FamilyName,
				AlternativeNames = person.AlternativeNames,
				About = person.About,
				Images = person.Images,
				MemberFavorites = person.MemberFavorites,
				Birthday = person.Birthday,
				VoiceActingRoles = voiceActingRoles
			};
		}
	}
}

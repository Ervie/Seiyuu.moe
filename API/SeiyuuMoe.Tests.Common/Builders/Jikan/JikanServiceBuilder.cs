using JikanDotNet;
using JikanDotNet.Exceptions;
using Moq;
using SeiyuuMoe.Infrastructure.Jikan;
using System.Collections.Generic;

namespace SeiyuuMoe.Tests.Common.Builders.Jikan
{
	public class JikanServiceBuilder
	{
		public readonly Mock<IJikan> JikanClient = new Mock<IJikan>();

		public JikanService Build() => new JikanService(JikanClient.Object);

		public JikanServiceBuilder WithAnimeReturned(Anime anime)
		{
			JikanClient.Setup(x => x.GetAnime(It.IsAny<long>()))
				.ReturnsAsync(anime);
			return this;
		}

		public JikanServiceBuilder WithGetAnimeThrowing()
		{
			JikanClient.Setup(x => x.GetAnime(It.IsAny<long>()))
				.ThrowsAsync(new JikanRequestException());
			return this;
		}

		public JikanServiceBuilder WithCharacterReturned(Character character)
		{
			JikanClient.Setup(x => x.GetCharacter(It.IsAny<long>()))
				.ReturnsAsync(character);
			return this;
		}

		public JikanServiceBuilder WithGetCharacterThrowing()
		{
			JikanClient.Setup(x => x.GetCharacter(It.IsAny<long>()))
				.ThrowsAsync(new JikanRequestException());
			return this;
		}

		public JikanServiceBuilder WithPersonReturned(Person person)
		{
			JikanClient.Setup(x => x.GetPerson(It.IsAny<long>()))
				.ReturnsAsync(person);
			return this;
		}

		public JikanServiceBuilder WithLastSeasonArchiveReturned(SeasonArchive seasonArchive)
		{
			JikanClient.Setup(x => x.GetSeasonArchive())
				.ReturnsAsync(new SeasonArchives { Archives = new List<SeasonArchive> { seasonArchive } });
			return this;
		}

		public JikanServiceBuilder WithGetSeasonArchiveThrowing()
		{
			JikanClient.Setup(x => x.GetSeasonArchive())
				.ThrowsAsync(new JikanRequestException());
			return this;
		}
	}
}
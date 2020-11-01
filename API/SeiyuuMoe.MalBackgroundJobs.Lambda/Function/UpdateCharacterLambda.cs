using JikanDotNet;
using SeiyuuMoe.Domain.SqsMessages;
using SeiyuuMoe.Infrastructure.Characters;
using SeiyuuMoe.Infrastructure.Configuration;
using SeiyuuMoe.Infrastructure.Context;
using SeiyuuMoe.Infrastructure.Jikan;
using SeiyuuMoe.MalBackgroundJobs.Application.Handlers;
using SeiyuuMoe.MalBackgroundJobs.Lambda.Base;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Lambda.Function
{
	public class UpdateCharacterLambda : BaseSqsLambda<UpdateCharacterMessage>
	{
		protected async override Task HandleAsync(UpdateCharacterMessage message)
		{
			Console.WriteLine($"UpdateCharacterLambda was invoked for character {message.Id}");

			var dbConfig = ConfigurationReader.DatabaseConfiguration;
			using var dbContext = new SeiyuuMoeContext(dbConfig);

			var handler = CreateHandler(dbContext);
			await handler.HandleAsync(message);
		}

		private static UpdateCharacterHandler CreateHandler(SeiyuuMoeContext dbContext)
		{
			var characterRepository = new CharacterRepository(dbContext);

			var jikanUrl = ConfigurationReader.JikanUrl;
			var jikanClient = new Jikan(jikanUrl);
			var jikanService = new JikanService(jikanClient);

			return new UpdateCharacterHandler(characterRepository, jikanService);
		}
	}
}
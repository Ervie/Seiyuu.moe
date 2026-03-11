using JikanDotNet;
using SeiyuuMoe.Domain.SqsMessages;
using SeiyuuMoe.Infrastructure.Configuration;
using SeiyuuMoe.Infrastructure.Database.Characters;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Jikan;
using SeiyuuMoe.MalBackgroundJobs.Application.Handlers;
using SeiyuuMoe.MalBackgroundJobs.Lambda.Base;
using System;
using System.Threading.Tasks;
using JikanDotNet.Config;

namespace SeiyuuMoe.MalBackgroundJobs.Lambda.Function
{
	public class UpdateCharacterLambda : BaseSqsLambda<UpdateCharacterMessage>
	{
		private static readonly IJikan JikanClient;
		private static readonly JikanService JikanService;

		static UpdateCharacterLambda()
		{
			var jikanConfiguration = new JikanClientConfiguration { SuppressException = true };
			JikanClient = new Jikan(jikanConfiguration);
			JikanService = new JikanService(JikanClient);
		}

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

			return new UpdateCharacterHandler(characterRepository, JikanService);
		}
	}
}
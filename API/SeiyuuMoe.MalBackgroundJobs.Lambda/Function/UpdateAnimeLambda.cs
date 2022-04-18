using JikanDotNet;
using SeiyuuMoe.Domain.SqsMessages;
using SeiyuuMoe.Infrastructure.Configuration;
using SeiyuuMoe.Infrastructure.Database.Animes;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Database.Seasons;
using SeiyuuMoe.Infrastructure.Jikan;
using SeiyuuMoe.MalBackgroundJobs.Application.Handlers;
using SeiyuuMoe.MalBackgroundJobs.Lambda.Base;
using System;
using System.Threading.Tasks;
using JikanDotNet.Config;

namespace SeiyuuMoe.MalBackgroundJobs.Lambda.Function
{
	public class UpdateAnimeLambda : BaseSqsLambda<UpdateAnimeMessage>
	{
		protected async override Task HandleAsync(UpdateAnimeMessage message)
		{
			Console.WriteLine($"UpdateAnimeLambda was invoked for anime {message.Id}");

			var dbConfig = ConfigurationReader.DatabaseConfiguration;
			using var dbContext = new SeiyuuMoeContext(dbConfig);

			var handler = CreateHandler(dbContext);
			await handler.HandleAsync(message);
		}

		private static UpdateAnimeHandler CreateHandler(SeiyuuMoeContext dbContext)
		{
			var animeRepository = new AnimeRepository(dbContext);
			var seasonRepository = new SeasonRepository(dbContext);

			var jikanUrl = ConfigurationReader.JikanUrl;;
			var jikanConfiguration = new JikanClientConfiguration { Endpoint = jikanUrl, SuppressException = true };
			var jikanClient = new Jikan(jikanConfiguration);
			var jikanService = new JikanService(jikanClient);

			return new UpdateAnimeHandler(animeRepository, seasonRepository, jikanService);
		}
	}
}
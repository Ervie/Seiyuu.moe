using JikanDotNet;
using SeiyuuMoe.Infrastructure.Configuration;
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
	public class UpdateSeasonsLambda : BaseLambda
	{
		private static readonly IJikan JikanClient;
		private static readonly JikanService JikanService;

		static UpdateSeasonsLambda()
		{
			var jikanConfiguration = new JikanClientConfiguration { SuppressException = true };
			JikanClient = new Jikan(jikanConfiguration);
			JikanService = new JikanService(JikanClient);
		}

		protected override async Task HandleAsync()
		{
			Console.WriteLine("ScheduleAnimesLambda was invoked");

			var dbConfig = ConfigurationReader.DatabaseConfiguration;
			using var dbContext = new SeiyuuMoeContext(dbConfig);

			var handler = CreateHandler(dbContext);
			await handler.HandleAsync();
		}

		private UpdateSeasonsHandler CreateHandler(SeiyuuMoeContext dbContext)
		{
			var seasonRepository = new SeasonRepository(dbContext);

			return new UpdateSeasonsHandler(seasonRepository, JikanService);
		}
	}
}
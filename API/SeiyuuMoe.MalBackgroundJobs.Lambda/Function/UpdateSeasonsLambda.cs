using JikanDotNet;
using SeiyuuMoe.Infrastructure.Configuration;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Database.Seasons;
using SeiyuuMoe.Infrastructure.Jikan;
using SeiyuuMoe.MalBackgroundJobs.Application.Handlers;
using SeiyuuMoe.MalBackgroundJobs.Lambda.Base;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Lambda.Function
{
	public class UpdateSeasonsLambda : BaseLambda
	{
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

			var jikanUrl = ConfigurationReader.JikanUrl;
			var jikanClient = new Jikan(jikanUrl);
			var jikanService = new JikanService(jikanClient);

			return new UpdateSeasonsHandler(seasonRepository, jikanService);
		}
	}
}
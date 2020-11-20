using Amazon.SQS;
using SeiyuuMoe.Infrastructure.Animes;
using SeiyuuMoe.Infrastructure.Configuration;
using SeiyuuMoe.Infrastructure.Context;
using SeiyuuMoe.Infrastructure.Sns;
using SeiyuuMoe.MalBackgroundJobs.Application.Handlers;
using SeiyuuMoe.MalBackgroundJobs.Lambda.Base;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Lambda.Function
{
	public class ScheduleAnimesLambda : BaseLambda
	{
		protected async override Task HandleAsync()
		{
			Console.WriteLine("ScheduleAnimesLambda was invoked");

			var dbConfig = ConfigurationReader.DatabaseConfiguration;
			using var dbContext = new SeiyuuMoeContext(dbConfig);

			var handler = CreateHandler(dbContext);
			await handler.HandleAsync();
		}

		private static ScheduleAnimesHandler CreateHandler(SeiyuuMoeContext dbContext)
		{
			var animeRepository = new AnimeRepository(dbContext);
			var queueUrl = Environment.GetEnvironmentVariable("AnimeToUpdateQueueUrl");
			var animeUpdatePublisher = new SqsAnimeUpdatePublisher(new AmazonSQSClient(), queueUrl);
			var scheduleConfiguration = ConfigurationReader.MalBgJobsScheduleConfiguration;

			return new ScheduleAnimesHandler(
				scheduleConfiguration.UpdateBatchSize,
				scheduleConfiguration.DelayBetweenMessagesInSeconds,
				animeRepository,
				animeUpdatePublisher
			);
		}
	}
}
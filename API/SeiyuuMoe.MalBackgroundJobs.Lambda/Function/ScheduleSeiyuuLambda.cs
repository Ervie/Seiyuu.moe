using Amazon.SQS;
using SeiyuuMoe.Infrastructure.Configuration;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Database.Seiyuus;
using SeiyuuMoe.Infrastructure.Sqs;
using SeiyuuMoe.MalBackgroundJobs.Application.Handlers;
using SeiyuuMoe.MalBackgroundJobs.Lambda.Base;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Lambda.Function
{
	public class ScheduleSeiyuuLambda : BaseLambda
	{
		protected async override Task HandleAsync()
		{
			Console.WriteLine("ScheduleSeiyuuLambda was invoked");

			var dbConfig = ConfigurationReader.DatabaseConfiguration;
			using var dbContext = new SeiyuuMoeContext(dbConfig);

			var handler = CreateHandler(dbContext);
			await handler.HandleAsync();
		}

		private static ScheduleSeiyuuHandler CreateHandler(SeiyuuMoeContext dbContext)
		{
			var seiyuuRepository = new SeiyuuRepository(dbContext);
			var queueUrl = Environment.GetEnvironmentVariable("SeiyuuToUpdateQueueUrl");
			var characterUpdatePublisher = new SqsSeiyuuUpdatePublisher(new AmazonSQSClient(), queueUrl);
			var scheduleConfiguration = ConfigurationReader.MalBgJobsScheduleConfiguration;

			return new ScheduleSeiyuuHandler(
				scheduleConfiguration.UpdateBatchSize,
				scheduleConfiguration.DelayBetweenMessagesInSeconds,
				seiyuuRepository,
				characterUpdatePublisher
			);
		}
	}
}
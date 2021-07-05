using Amazon.SQS;
using SeiyuuMoe.Infrastructure.Configuration;
using SeiyuuMoe.Infrastructure.Database.Characters;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Sqs;
using SeiyuuMoe.MalBackgroundJobs.Application.Handlers;
using SeiyuuMoe.MalBackgroundJobs.Lambda.Base;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Lambda.Function
{
	public class ScheduleCharactersLambda : BaseLambda
	{
		protected async override Task HandleAsync()
		{
			Console.WriteLine("ScheduleCharactersLambda was invoked");

			var dbConfig = ConfigurationReader.DatabaseConfiguration;
			using var dbContext = new SeiyuuMoeContext(dbConfig);

			var handler = CreateHandler(dbContext);
			await handler.HandleAsync();
		}

		private static ScheduleCharactersHandler CreateHandler(SeiyuuMoeContext dbContext)
		{
			var characterRepository = new CharacterRepository(dbContext);
			var queueUrl = Environment.GetEnvironmentVariable("CharactersToUpdateQueueUrl");
			var characterUpdatePublisher = new SqsCharacterUpdatePublisher(new AmazonSQSClient(), queueUrl);
			var scheduleConfiguration = ConfigurationReader.MalBgJobsScheduleConfiguration;

			return new ScheduleCharactersHandler(
				scheduleConfiguration.UpdateBatchSize,
				scheduleConfiguration.DelayBetweenMessagesInSeconds,
				characterRepository,
				characterUpdatePublisher
			);
		}
	}
}
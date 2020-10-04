using Amazon.Lambda.SQSEvents;
using SeiyuuMoe.Domain.SqsMessages;
using SeiyuuMoe.MalBackgroundJobs.Lambda.Function;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.LocalLambdaRunner.Runners
{
	public class UpdateAnimeLambdaRunner
	{
		public async Task RunAsync()
		{
			var lambda = new UpdateAnimeLambda();

			var message = new UpdateAnimeMessage
			{
				Id = Guid.Parse("5ab374d5-10cf-435e-9ae9-4c4caa3175a5"),
				MalId = 1
			};

			var sqsMessage = new SQSEvent
			{
				Records = new List<SQSEvent.SQSMessage>
				{
					new SQSEvent.SQSMessage
					{
						Body = JsonSerializer.Serialize(message)
					}
				}
			};

			await lambda.InvokeAsync(sqsMessage);
		}
	}
}
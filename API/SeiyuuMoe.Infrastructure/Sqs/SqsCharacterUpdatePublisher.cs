using Amazon.SQS;
using Amazon.SQS.Model;
using SeiyuuMoe.Domain.Publishers;
using SeiyuuMoe.Domain.SqsMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Sqs
{
	public class SqsCharacterUpdatePublisher : ICharactersUpdatePublisher
	{
		private const int MaxQueueBatchSize = 10;

		private readonly IAmazonSQS _sqsService;
		private readonly string _queueUrl;

		public SqsCharacterUpdatePublisher(IAmazonSQS sqsService, string queueArn)
		{
			_sqsService = sqsService;
			_queueUrl = queueArn;
		}

		public async Task PublishCharacterUpdatesAsync(IReadOnlyList<UpdateCharacterMessage> messages)
		{
			if (messages.Count == 0)
			{
				return;
			}

			for (var i = 0; i < messages.Count; i += MaxQueueBatchSize)
			{
				var chunk = messages.Skip(i).Take(MaxQueueBatchSize).ToList();
				var entries = chunk.Select((m, idx) => new SendMessageBatchRequestEntry
				{
					Id = (i + idx).ToString(),
					MessageBody = JsonSerializer.Serialize(m)
				}).ToList();

				var request = new SendMessageBatchRequest
				{
					QueueUrl = _queueUrl,
					Entries = entries
				};

				await _sqsService.SendMessageBatchAsync(request);
			}
		}
	}
}
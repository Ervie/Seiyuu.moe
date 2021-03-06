﻿using Amazon.SQS;
using Amazon.SQS.Model;
using SeiyuuMoe.Domain.Publishers;
using SeiyuuMoe.Domain.SqsMessages;
using System.Text.Json;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Sns
{
	public class SqsAnimeUpdatePublisher : IAnimeUpdatePublisher
	{
		private readonly IAmazonSQS _sqsService;
		private readonly string _queueUrl;

		public SqsAnimeUpdatePublisher(IAmazonSQS sqsService, string queueArn)
		{
			_sqsService = sqsService;
			_queueUrl = queueArn;
		}

		public async Task PublishAnimeUpdateAsync(UpdateAnimeMessage updateAnimeMessage, int delayInSeconds = 0)
		{
			var sendMessageRequest = new SendMessageRequest
			{
				MessageBody = JsonSerializer.Serialize(updateAnimeMessage),
				QueueUrl = _queueUrl,
				DelaySeconds = delayInSeconds
			};

			await _sqsService.SendMessageAsync(sendMessageRequest);
		}
	}
}
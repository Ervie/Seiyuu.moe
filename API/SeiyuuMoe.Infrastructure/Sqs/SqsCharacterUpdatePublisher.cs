using Amazon.SQS;
using Amazon.SQS.Model;
using SeiyuuMoe.Domain.Publishers;
using SeiyuuMoe.Domain.SqsMessages;
using System.Text.Json;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Sqs
{
	public class SqsCharacterUpdatePublisher : ICharactersUpdatePublisher
	{
		private readonly IAmazonSQS _sqsService;
		private readonly string _queueUrl;

		public SqsCharacterUpdatePublisher(IAmazonSQS sqsService, string queueArn)
		{
			_sqsService = sqsService;
			_queueUrl = queueArn;
		}

		public async Task PublishCharacterUpdateAsync(UpdateCharacterMessage updateCharacterMessage, int delayInSeconds = 0)
		{
			var sendMessageRequest = new SendMessageRequest
			{
				MessageBody = JsonSerializer.Serialize(updateCharacterMessage),
				QueueUrl = _queueUrl,
				DelaySeconds = delayInSeconds
			};

			await _sqsService.SendMessageAsync(sendMessageRequest);
		}
	}
}
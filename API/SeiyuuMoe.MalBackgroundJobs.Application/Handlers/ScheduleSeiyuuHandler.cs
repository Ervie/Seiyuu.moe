using SeiyuuMoe.Domain.Publishers;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.SqsMessages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Application.Handlers
{
	public class ScheduleSeiyuuHandler
	{
		private readonly int _batchSize;
		private readonly int _delayBetweenMessages;
		private readonly ISeiyuuRepository _seiyuuRepository;
		private readonly ISeiyuuUpdatePublisher _seiyuuUpdatePublisher;

		public ScheduleSeiyuuHandler(int batchSize, int delayBetweenMessages, ISeiyuuRepository seiyuuRepository, ISeiyuuUpdatePublisher seiyuuUpdatePublisher)
		{
			_batchSize = batchSize;
			_delayBetweenMessages = delayBetweenMessages;
			_seiyuuRepository = seiyuuRepository;
			_seiyuuUpdatePublisher = seiyuuUpdatePublisher;
		}

		public async Task HandleAsync()
		{
			var thresholdDate = DateTime.UtcNow.AddDays(-14);

			var seiyuuToUpdate = await _seiyuuRepository.GetOlderThanModifiedDate(thresholdDate, _batchSize);

			var publishTasks = seiyuuToUpdate.Select(
				(a, i) => _seiyuuUpdatePublisher.PublishSeiyuuUpdateAsync(
					new UpdateSeiyuuMessage { Id = a.Id, MalId = a.MalId },
					i * _delayBetweenMessages
				)
			);

			await Task.WhenAll(publishTasks);
		}
	}
}
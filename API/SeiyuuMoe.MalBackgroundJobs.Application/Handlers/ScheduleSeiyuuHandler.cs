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
		private readonly ISeiyuuRepository _seiyuuRepository;
		private readonly ISeiyuuUpdatePublisher _seiyuuUpdatePublisher;

		public ScheduleSeiyuuHandler(int batchSize, ISeiyuuRepository seiyuuRepository, ISeiyuuUpdatePublisher seiyuuUpdatePublisher)
		{
			_batchSize = batchSize;
			_seiyuuRepository = seiyuuRepository;
			_seiyuuUpdatePublisher = seiyuuUpdatePublisher;
		}

		public async Task HandleAsync()
		{
			var thresholdDate = DateTime.UtcNow.AddDays(-14);
			DateTime? afterModificationDate = null;
			Guid? afterId = null;

			while (true)
			{
				var batch = await _seiyuuRepository.GetOlderThanModifiedDate(thresholdDate, _batchSize, afterModificationDate, afterId);
				if (batch.Count == 0)
				{
					break;
				}

				var messages = batch.Select(a => new UpdateSeiyuuMessage { Id = a.Id, MalId = a.MalId }).ToList();
				await _seiyuuUpdatePublisher.PublishSeiyuuUpdatesAsync(messages);

				var last = batch[batch.Count - 1];
				afterModificationDate = last.ModificationDate;
				afterId = last.Id;

				if (batch.Count < _batchSize)
				{
					break;
				}
			}
		}
	}
}
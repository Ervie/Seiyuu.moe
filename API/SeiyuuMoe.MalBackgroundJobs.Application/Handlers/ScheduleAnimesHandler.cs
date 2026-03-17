using SeiyuuMoe.Domain.Publishers;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.SqsMessages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Application.Handlers
{
	public class ScheduleAnimesHandler
	{
		private readonly int _batchSize;
		private readonly IAnimeRepository _animeRepository;
		private readonly IAnimeUpdatePublisher _animeUpdatePublisher;

		public ScheduleAnimesHandler(int batchSize, IAnimeRepository animeRepository, IAnimeUpdatePublisher animeUpdatePublisher)
		{
			_batchSize = batchSize;
			_animeRepository = animeRepository;
			_animeUpdatePublisher = animeUpdatePublisher;
		}

		public async Task HandleAsync()
		{
			var thresholdDate = DateTime.UtcNow.AddDays(-31);
			DateTime? afterModificationDate = null;
			Guid? afterId = null;

			while (true)
			{
				var batch = await _animeRepository.GetOlderThanModifiedDate(thresholdDate, _batchSize, afterModificationDate, afterId);
				if (batch.Count == 0)
				{
					break;
				}

				var messages = batch.Select(a => new UpdateAnimeMessage { Id = a.Id, MalId = a.MalId }).ToList();
				await _animeUpdatePublisher.PublishAnimeUpdatesAsync(messages);

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
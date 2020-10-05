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
		private readonly int _delayBetweenMessages;
		private readonly IAnimeRepository _animeRepository;
		private readonly IAnimeUpdatePublisher _animeUpdatePublisher;

		public ScheduleAnimesHandler(int batchSize, int delayBetweenMessages, IAnimeRepository animeRepository, IAnimeUpdatePublisher animeUpdatePublisher)
		{
			_batchSize = batchSize;
			_delayBetweenMessages = delayBetweenMessages;
			_animeRepository = animeRepository;
			_animeUpdatePublisher = animeUpdatePublisher;
		}

		public async Task HandleAsync()
		{
			var animesToUpdate = await _animeRepository.GetOlderThanModifiedDate(DateTime.UtcNow, _batchSize);

			var publishTasks = Enumerable.Range(0, animesToUpdate.Count).Select(
					a => _animeUpdatePublisher.PublishAnimeUpdateAsync(
						new UpdateAnimeMessage { Id = animesToUpdate.ElementAt(a).Id, MalId = animesToUpdate.ElementAt(a).MalId },
						a * _delayBetweenMessages
					)
			);
			await Task.WhenAll(publishTasks);
		}
	}
}
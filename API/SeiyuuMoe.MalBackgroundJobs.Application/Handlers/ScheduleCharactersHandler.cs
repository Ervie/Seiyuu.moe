using SeiyuuMoe.Domain.Publishers;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.SqsMessages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Application.Handlers
{
	public class ScheduleCharactersHandler
	{
		private readonly int _batchSize;
		private readonly int _delayBetweenMessages;
		private readonly ICharacterRepository _characterRepository;
		private readonly ICharactersUpdatePublisher _charactersUpdatePublisher;

		public ScheduleCharactersHandler(int batchSize, int delayBetweenMessages, ICharacterRepository characterRepository, ICharactersUpdatePublisher charactersUpdatePublisher)
		{
			_batchSize = batchSize;
			_delayBetweenMessages = delayBetweenMessages;
			_characterRepository = characterRepository;
			_charactersUpdatePublisher = charactersUpdatePublisher;
		}

		public async Task HandleAsync()
		{
			var thresholdDate = DateTime.UtcNow.AddDays(-31);

			var charactersToUpdate = await _characterRepository.GetOlderThanModifiedDate(thresholdDate, _batchSize);

			var publishTasks = charactersToUpdate.Select(
				(a, i) => _charactersUpdatePublisher.PublishCharacterUpdateAsync(
					new UpdateCharacterMessage { Id = a.Id, MalId = a.MalId },
					i * _delayBetweenMessages
				)
			);

			await Task.WhenAll(publishTasks);
		}
	}
}
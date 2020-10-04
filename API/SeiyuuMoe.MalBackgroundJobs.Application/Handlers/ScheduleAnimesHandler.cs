using SeiyuuMoe.Domain.Publishers;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.SqsMessages;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Application.Handlers
{
	public class ScheduleAnimesHandler
	{
		private readonly IAnimeRepository _animeRepository;
		private readonly IAnimeUpdatePublisher _animeUpdatePublisher;

		public ScheduleAnimesHandler(IAnimeRepository animeRepository, IAnimeUpdatePublisher animeUpdatePublisher)
		{
			_animeRepository = animeRepository;
			_animeUpdatePublisher = animeUpdatePublisher;
		}

		public async Task HandleAsync()
		{
			// var animesToUpdate = await _animeRepository.GetAllAsync(x => string.IsNullOrWhiteSpace(x.ImageUrl) || x.AiringDate == null || string.IsNullOrWhiteSpace(x.About));
			var animesToUpdate = await _animeRepository.GetAllAsync(x => x.MalId < 2);

			var publishTasks = animesToUpdate.Select(
				a => _animeUpdatePublisher.PublishAnimeUpdateAsync(
					new UpdateAnimeMessage { Id = a.Id, MalId = a.MalId }
				)
			);
			await Task.WhenAll(publishTasks);
		}
	}
}
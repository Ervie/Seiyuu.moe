using SeiyuuMoe.Application.Animes.Extensions;
using SeiyuuMoe.Domain.Repositories;
using System.Threading.Tasks;

namespace SeiyuuMoe.Application.Animes.GetAnimeCardInfo
{
	public class GetAnimeCardInfoQueryHandler
	{
		private readonly IAnimeRepository _animeRepository;

		public GetAnimeCardInfoQueryHandler(IAnimeRepository animeRepository)
		{
			_animeRepository = animeRepository;
		}

		public async Task<AnimeCardDto> HandleAsync(GetAnimeCardInfoQuery query)
		{
			var entity = await _animeRepository.GetAsync(query.MalId);

			return entity.ToAnimeCardDto();
		}
	}
}
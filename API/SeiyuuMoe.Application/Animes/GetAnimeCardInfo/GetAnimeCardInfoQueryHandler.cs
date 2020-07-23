using SeiyuuMoe.Application.Animes.Extensions;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.WebEssentials;
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

		public async Task<QueryResponse<AnimeCardDto>> HandleAsync(GetAnimeCardInfoQuery query)
		{
			var entity = await _animeRepository.GetAsync(query.MalId);

			return new QueryResponse<AnimeCardDto>(entity.ToAnimeCardDto());
		}
	}
}
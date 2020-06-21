using SeiyuuMoe.Application.Anime.Extensions;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.WebEssentials;
using System.Threading.Tasks;

namespace SeiyuuMoe.Application.Anime.GetAnimeCardInfo
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
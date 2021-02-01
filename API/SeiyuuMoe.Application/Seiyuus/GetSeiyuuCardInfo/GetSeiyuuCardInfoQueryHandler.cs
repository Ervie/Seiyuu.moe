using SeiyuuMoe.Application.Seiyuus.Extensions;
using SeiyuuMoe.Domain.Repositories;
using System.Threading.Tasks;

namespace SeiyuuMoe.Application.Seiyuus.GetSeiyuuCardInfo
{
	public class GetSeiyuuCardInfoQueryHandler
	{
		private readonly ISeiyuuRepository _seiyuuRepository;

		public GetSeiyuuCardInfoQueryHandler(ISeiyuuRepository seiyuuRepository)
		{
			_seiyuuRepository = seiyuuRepository;
		}

		public async Task<SeiyuuCardDto> HandleAsync(GetSeiyuuCardInfoQuery query)
		{
			var entity = await _seiyuuRepository.GetByMalIdAsync(query.MalId);

			return entity.ToSeiyuuCardDto();
		}
	}
}
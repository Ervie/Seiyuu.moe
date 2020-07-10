using SeiyuuMoe.Application.Seiyuus.Extensions;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.WebEssentials;
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

		public async Task<QueryResponse<SeiyuuCardDto>> HandleAsync(GetSeiyuuCardInfoQuery query)

		{
			var entity = await _seiyuuRepository.GetAsync(query.MalId);

			return new QueryResponse<SeiyuuCardDto>(entity.ToSeiyuuCardDto());
		}
	}
}
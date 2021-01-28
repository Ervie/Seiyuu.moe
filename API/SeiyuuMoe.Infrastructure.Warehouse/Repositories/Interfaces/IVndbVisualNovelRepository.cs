using SeiyuuMoe.Domain.WebEssentials;
using SeiyuuMoe.Infrastructure.Warehouse.VndbEntities;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Warehouse.Repositories.Interfaces
{
	public interface IVndbVisualNovelRepository
	{
		Task<int> GetCountAsync();

		Task<PagedResult<VndbVisualNovel>> GetOrderedPageByAsync(int page = 0, int pageSize = 100);
	}
}
using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.WebEssentials;
using SeiyuuMoe.Infrastructure.Warehouse.Repositories.Interfaces;
using SeiyuuMoe.Infrastructure.Warehouse.VndbEntities;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Warehouse.Repositories
{
	public class VndbVisualNovelRepository : IVndbVisualNovelRepository
	{
		private readonly WarehouseDbContext _warehouseDbContext;

		public VndbVisualNovelRepository(WarehouseDbContext warehouseDbContext)
		{
			_warehouseDbContext = warehouseDbContext;
		}

		public Task<int> GetCountAsync() => _warehouseDbContext.VisualNovels.CountAsync();

		public async Task<PagedResult<VndbVisualNovel>> GetOrderedPageByAsync(int page = 0, int pageSize = 100)
		{
			var totalCount = await _warehouseDbContext.VisualNovels.CountAsync();
			var results = await _warehouseDbContext.VisualNovels
				.Skip(page * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return new PagedResult<VndbVisualNovel>
			{
				Results = results,
				Page = page,
				PageSize = pageSize,
				TotalCount = totalCount
			};
		}
	}
}
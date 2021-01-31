using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.WebEssentials;
using SeiyuuMoe.Infrastructure.Warehouse.Repositories.Interfaces;
using SeiyuuMoe.Infrastructure.Warehouse.VndbEntities;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Warehouse.Repositories
{
	public class VndbCharacterRepository : IVndbCharacterRepository
	{
		private readonly WarehouseDbContext _warehouseDbContext;

		public VndbCharacterRepository(WarehouseDbContext warehouseDbContext)
		{
			_warehouseDbContext = warehouseDbContext;
		}

		public Task<int> GetCountAsync() => _warehouseDbContext.Characters.CountAsync();

		public async Task<PagedResult<VndbCharacter>> GetOrderedPageByAsync(int page = 0, int pageSize = 100)
		{
			var totalCount = await _warehouseDbContext.Characters.CountAsync();
			var results = await _warehouseDbContext.Characters
				.Skip(page * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return new PagedResult<VndbCharacter>
			{
				Results = results,
				Page = page,
				PageSize = pageSize,
				TotalCount = totalCount
			};
		}
	}
}
using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Infrastructure.Warehouse.Repositories.Interfaces;
using SeiyuuMoe.Infrastructure.Warehouse.VndbEntities;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Warehouse.Repositories
{
	public class VndbStaffRepository : IVndbStaffRepository
	{
		private readonly WarehouseDbContext _warehouseDbContext;

		public VndbStaffRepository(WarehouseDbContext warehouseDbContext)
		{
			_warehouseDbContext = warehouseDbContext;
		}

		public Task<VndbStaff> GetSeiyuuAsync(int vndbId)
			=> _warehouseDbContext.Staffs
			.Include(x => x.MainAlias)
			.FirstAsync(x => x.Id == vndbId);
	}
}
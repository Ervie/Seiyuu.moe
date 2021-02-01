using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Infrastructure.Warehouse.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Warehouse.Repositories
{
	public class VndbStaffAliasRepository : IVndbStaffAliasRepository
	{
		private readonly WarehouseDbContext _warehouseDbContext;

		public VndbStaffAliasRepository(WarehouseDbContext warehouseDbContext)
		{
			_warehouseDbContext = warehouseDbContext;
		}

		public Task<List<int>> GetDistinctSeiyuuIdsAsync()
			=> _warehouseDbContext.StaffAliases
			.Where(x => x.Roles.Any() && x.Staff.Language.Equals("ja"))
			.Select(x => x.Id)
			.Distinct()
			.ToListAsync();
	}
}
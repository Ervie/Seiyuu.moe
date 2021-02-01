using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Warehouse.Repositories.Interfaces
{
	public interface IVndbStaffAliasRepository
	{
		Task<List<int>> GetDistinctSeiyuuIdsAsync();
	}
}
using SeiyuuMoe.Infrastructure.Warehouse.VndbEntities;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Warehouse.Repositories.Interfaces
{
	public interface IVndbStaffRepository
	{
		Task<VndbStaff> GetSeiyuuAsync(int vndbId);
	}
}
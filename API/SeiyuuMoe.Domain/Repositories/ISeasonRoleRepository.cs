using SeiyuuMoe.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Repositories
{
	public interface ISeasonRoleRepository
	{
		public Task<IReadOnlyList<Role>> GetAllRolesInSeason(ICollection<long> animeIds, bool mainRolesOnly);
	}
}
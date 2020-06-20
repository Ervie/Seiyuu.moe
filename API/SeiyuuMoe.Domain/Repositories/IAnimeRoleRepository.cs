using SeiyuuMoe.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.Repositories.Repositories
{
	public interface IAnimeRoleRepository
	{
		public Task<IReadOnlyList<Role>> GetAllRolesInAnimeAsync(long animeMalId);

		public Task AddAsync(Role role);
	}
}
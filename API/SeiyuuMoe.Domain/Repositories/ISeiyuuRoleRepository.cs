using SeiyuuMoe.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Repositories
{
	public interface ISeiyuuRoleRepository
	{
		public Task<IReadOnlyList<Role>> GetAllSeiyuuRolesAsync(long seiyuuMalId, bool mainRolesOnly);
	}
}
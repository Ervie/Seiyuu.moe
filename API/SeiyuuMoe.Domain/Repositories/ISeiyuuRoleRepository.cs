using SeiyuuMoe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Repositories
{
	public interface ISeiyuuRoleRepository
	{
		public Task<IReadOnlyList<AnimeRole>> GetAllSeiyuuRolesAsync(Guid seiyuuId, bool mainRolesOnly);
		public Task<IReadOnlyList<AnimeRole>> GetAllSeiyuuRolesByMalIdAsync(long seiyuuMalId, bool mainRolesOnly);
	}
}
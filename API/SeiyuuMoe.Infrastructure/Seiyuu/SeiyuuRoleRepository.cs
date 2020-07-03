using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Infrastructure.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Seiyuu
{
	public class SeiyuuRoleRepository : ISeiyuuRoleRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public SeiyuuRoleRepository(SeiyuuMoeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<IReadOnlyList<Role>> GetAllSeiyuuRolesAsync(long seiyuuMalId, bool mainRolesOnly)
			=> await _dbContext.Role
			.Include(a => a.Anime)
			.Include(a => a.Character)
			.Include(a => a.Seiyuu)
			.Where(x => x.LanguageId == 1
				&& x.SeiyuuId == seiyuuMalId
				&& (!mainRolesOnly || x.RoleTypeId == 1))
			.ToListAsync();
	}
}
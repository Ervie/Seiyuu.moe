using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Infrastructure.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Database.Seiyuus
{
	public class SeiyuuRoleRepository : ISeiyuuRoleRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public SeiyuuRoleRepository(SeiyuuMoeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<IReadOnlyList<AnimeRole>> GetAllSeiyuuRolesAsync(Guid seiyuuId, bool mainRolesOnly)
			=> await _dbContext.AnimeRoles
			.Include(a => a.Anime)
			.Include(a => a.Character)
			.Include(a => a.Seiyuu)
			.Where(x => x.LanguageId == LanguageId.Japanese
				&& x.SeiyuuId == seiyuuId
				&& (!mainRolesOnly || x.RoleTypeId == AnimeRoleTypeId.Main))
			.ToListAsync();

		public async Task<IReadOnlyList<AnimeRole>> GetAllSeiyuuRolesByMalIdAsync(long seiyuuMalId, bool mainRolesOnly)
		=> await _dbContext.AnimeRoles
			.Include(a => a.Anime)
			.Include(a => a.Character)
			.Include(a => a.Seiyuu)
			.Where(x => x.LanguageId == LanguageId.Japanese
				&& x.Seiyuu.MalId == seiyuuMalId
				&& (!mainRolesOnly || x.RoleTypeId == AnimeRoleTypeId.Main))
			.ToListAsync();
	}
}
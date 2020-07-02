using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Infrastructure.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Season
{
	public class SeasonRoleRepository : ISeasonRoleRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public SeasonRoleRepository(SeiyuuMoeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<IReadOnlyList<Role>> GetAllRolesInSeason(ICollection<long> animeIds, bool mainRolesOnly)
			=> await _dbContext.Role
			.Include(a => a.Anime)
			.Include(a => a.Character)
			.Include(a => a.Seiyuu)
			.Where(x => x.LanguageId == 1
				&& animeIds.Contains(x.AnimeId.Value)
				&& (!mainRolesOnly || x.RoleTypeId == 1))
			.ToListAsync();
	}
}
using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Infrastructure.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Database.Animes
{
	public class SeasonRoleRepository : ISeasonRoleRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public SeasonRoleRepository(SeiyuuMoeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<IReadOnlyList<AnimeRole>> GetAllRolesInSeason(ICollection<Guid> animeIds, bool mainRolesOnly)
			=> await _dbContext.AnimeRoles
			.Include(a => a.Anime)
			.Include(a => a.Character)
			.Include(a => a.Seiyuu)
			.Where(x => x.LanguageId == LanguageId.Japanese
				&& animeIds.Contains(x.AnimeId.Value)
				&& (!mainRolesOnly || x.RoleTypeId == AnimeRoleTypeId.Main))
			.ToListAsync();
	}
}
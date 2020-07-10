using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Infrastructure.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Animes
{
	public class AnimeRoleRepository: IAnimeRoleRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public AnimeRoleRepository(SeiyuuMoeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddAsync(Role role)
		{
			await _dbContext.Role.AddAsync(role);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<IReadOnlyList<Role>> GetAllRolesInAnimeAsync(long animeMalId)
			=> await _dbContext.Role
			.Include(a => a.Anime)
			.Include(a => a.Character)
			.Include(a => a.Seiyuu)
			.Where(x => x.LanguageId == 1
				&& x.AnimeId == animeMalId)
			.ToListAsync();
	}
}
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
	public class AnimeRoleRepository : IAnimeRoleRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public AnimeRoleRepository(SeiyuuMoeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddAsync(AnimeRole role)
		{
			await _dbContext.AnimeRoles.AddAsync(role);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<IReadOnlyList<AnimeRole>> GetAllRolesInAnimeAsync(Guid animeId)
			=> await _dbContext.AnimeRoles
			.Include(a => a.Anime)
			.Include(a => a.Character)
			.Include(a => a.Seiyuu)
			.Where(x => x.LanguageId == LanguageId.Japanese
				&& x.AnimeId == animeId)
			.ToListAsync();

		public async Task<IReadOnlyList<AnimeRole>> GetAllRolesInAnimeByMalIdAsync(long animeMalId)
			=> await _dbContext.AnimeRoles
			.Include(a => a.Anime)
			.Include(a => a.Character)
			.Include(a => a.Seiyuu)
			.Where(x => x.LanguageId == LanguageId.Japanese
				&& x.Anime.MalId == animeMalId)
			.ToListAsync();
	}
}
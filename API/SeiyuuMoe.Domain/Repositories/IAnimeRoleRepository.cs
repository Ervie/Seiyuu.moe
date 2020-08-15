using SeiyuuMoe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Repositories
{
	public interface IAnimeRoleRepository
	{
		public Task<IReadOnlyList<AnimeRole>> GetAllRolesInAnimeAsync(Guid animeId);
		public Task<IReadOnlyList<AnimeRole>> GetAllRolesInAnimeByMalIdAsync(long animeMalId);

		public Task AddAsync(AnimeRole role);
	}
}
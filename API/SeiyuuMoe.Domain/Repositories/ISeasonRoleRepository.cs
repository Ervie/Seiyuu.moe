using SeiyuuMoe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Repositories
{
	public interface ISeasonRoleRepository
	{
		public Task<IReadOnlyList<AnimeRole>> GetAllRolesInSeason(ICollection<Guid> animeIds, bool mainRolesOnly);
	}
}
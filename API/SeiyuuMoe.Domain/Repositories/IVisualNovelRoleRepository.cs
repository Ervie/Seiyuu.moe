using SeiyuuMoe.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Repositories
{
	public interface IVisualNovelRoleRepository
	{
		Task<bool> RoleExistsAsync(Guid vnId, Guid vnCharacterId, Guid seiyuuId);

		Task InsertRoleAsync(VisualNovelRole visualNovelRole);
	}
}
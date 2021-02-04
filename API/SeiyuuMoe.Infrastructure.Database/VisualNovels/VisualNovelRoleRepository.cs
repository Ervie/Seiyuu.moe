using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Infrastructure.Database.Context;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Database.VisualNovels
{
	public class VisualNovelRoleRepository : IVisualNovelRoleRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public VisualNovelRoleRepository(SeiyuuMoeContext seiyuuMoeContext)
		{
			_dbContext = seiyuuMoeContext;
		}

		public async Task InsertRoleAsync(VisualNovelRole visualNovelRole)
		{
			await _dbContext.VisualNovelRoles.AddAsync(visualNovelRole);
			await _dbContext.SaveChangesAsync();
		}

		public Task<bool> RoleExistsAsync(Guid vnId, Guid vnCharacterId, Guid seiyuuId)
			=> _dbContext.VisualNovelRoles.AnyAsync(x => x.CharacterId == vnCharacterId && x.VisualNovelId == vnId && x.SeiyuuId == seiyuuId);
	}
}
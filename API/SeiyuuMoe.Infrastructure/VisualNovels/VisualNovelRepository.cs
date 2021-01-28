using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Infrastructure.Context;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.VisualNovels
{
	public class VisualNovelRepository : IVisualNovelRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public VisualNovelRepository(SeiyuuMoeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddAsync(VisualNovel visualNovel)
		{
			await _dbContext.AddAsync(visualNovel);
			await _dbContext.SaveChangesAsync();
		}

		public Task<VisualNovel> GetAsync(long vndbId)
			=> _dbContext.VisualNovels
			.FirstOrDefaultAsync(x => x.VndbId == vndbId);

		public async Task UpdateAsync(VisualNovel visualNovel)
		{
			visualNovel.ModificationDate = DateTime.UtcNow;
			_dbContext.VisualNovels.Update(visualNovel);
			await _dbContext.SaveChangesAsync();
		}
	}
}
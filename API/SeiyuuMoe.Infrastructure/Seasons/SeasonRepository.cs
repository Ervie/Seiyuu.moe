using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Infrastructure.Context;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Seasons
{
	public class SeasonRepository : ISeasonRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public SeasonRepository(SeiyuuMoeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddAsync(Domain.Entities.Season season)
		{
			await _dbContext.Season.AddAsync(season);
			await _dbContext.SaveChangesAsync();
		}

		public Task<Season> GetAsync(Expression<Func<Season, bool>> predicate)
			=> _dbContext.Season.FirstOrDefaultAsync(predicate);
	}
}
using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Infrastructure.Context;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Repositories
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

		public Task<Domain.Entities.Season> GetAsync(Expression<Func<Domain.Entities.Season, bool>> predicate)
			=> _dbContext.Season.FirstOrDefaultAsync(predicate);
	}
}
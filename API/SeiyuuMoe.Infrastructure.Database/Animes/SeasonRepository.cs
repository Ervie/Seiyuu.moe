using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Infrastructure.Database.Context;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Database.Animes
{
	public class SeasonRepository : ISeasonRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public SeasonRepository(SeiyuuMoeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddAsync(AnimeSeason season)
		{
			await _dbContext.AnimeSeasons.AddAsync(season);
			await _dbContext.SaveChangesAsync();
		}

		public Task<AnimeSeason> GetAsync(Expression<Func<AnimeSeason, bool>> predicate)
			=> _dbContext.AnimeSeasons.FirstOrDefaultAsync(predicate);
	}
}
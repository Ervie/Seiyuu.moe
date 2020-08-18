using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.WebEssentials;
using SeiyuuMoe.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Animes
{
	public class AnimeRepository : IAnimeRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public AnimeRepository(SeiyuuMoeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddAsync(Anime anime)
		{
			await _dbContext.AddAsync(anime);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<IReadOnlyList<Anime>> GetAllAsync()
			=> await _dbContext.Animes
			.Include(a => a.Type)
			.Include(a => a.Status)
			.Include(a => a.Season)
			.ToListAsync();

		public async Task<IReadOnlyList<Anime>> GetAllAsync(Expression<Func<Anime, bool>> predicate)
			=> await _dbContext.Animes
			.Include(a => a.Type)
			.Include(a => a.Status)
			.Include(a => a.Season)
			.Where(predicate)
			.ToListAsync();

		public Task<int> GetAnimeCountAsync() => _dbContext.Animes.CountAsync();

		public Task<Anime> GetAsync(long animeMalId)
			=> _dbContext.Animes
			.Include(x => x.Type)
			.Include(x => x.Status)
			.Include(x => x.Season)
			.FirstOrDefaultAsync(x => x.MalId == animeMalId);

		public async Task<PagedResult<Anime>> GetOrderedPageByAsync(Expression<Func<Anime, bool>> predicate, int page = 0, int pageSize = 10)
		{
			var entities = _dbContext.Animes.Where(predicate);
			var totalCount = await entities.CountAsync();
			var results = await entities
				.Skip(page * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return new PagedResult<Anime>
			{
				Results = results,
				Page = page,
				PageSize = pageSize,
				TotalCount = totalCount
			};
		}

		public async Task<PagedResult<Anime>> GetOrderedPageByPopularityAsync(Expression<Func<Anime, bool>> predicate, int page = 0, int pageSize = 10)
		{
			var entities = _dbContext.Animes.Where(predicate);
			var totalCount = await entities.CountAsync();
			var results = await entities
				.OrderByDescending(x => x.Popularity)
				.Skip(page * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return new PagedResult<Anime>
			{
				Results = results,
				Page = page,
				PageSize = pageSize,
				TotalCount = totalCount
			};
		}

		public async Task UpdateAsync(Anime anime)
		{
			_dbContext.Animes.Update(anime);
			await _dbContext.SaveChangesAsync();
		}
	}
}
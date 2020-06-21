using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.WebEssentials;
using SeiyuuMoe.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Repositories
{
	public class SeiyuuRepository : ISeiyuuRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public SeiyuuRepository(SeiyuuMoeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddAsync(Domain.Entities.Seiyuu seiyuu)
		{
			await _dbContext.Seiyuu.AddAsync(seiyuu);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<ICollection<long>> GetAllIdsAsync()
			=> await _dbContext.Seiyuu.Select(x => x.MalId).ToListAsync();

		public Task<int> GetAnimeCountAsync() => _dbContext.Seiyuu.CountAsync();

		public Task<Domain.Entities.Seiyuu> GetAsync(long seiyuuMalId)
			=> _dbContext.Seiyuu.FirstOrDefaultAsync(x => x.MalId == seiyuuMalId);

		public async Task<PagedResult<Domain.Entities.Seiyuu>> GetOrderedPageAsync(Expression<Func<Domain.Entities.Seiyuu, bool>> predicate, int page = 0, int pageSize = 10)
		{
			var entities = _dbContext.Seiyuu.Where(predicate);
			var totalCount = await entities.CountAsync();
			var results = await entities
				.Skip(page * pageSize)
				.Take(pageSize)
				.OrderByDescending(x => x.Popularity)
				.ToListAsync();

			return new PagedResult<Domain.Entities.Seiyuu>
			{
				Results = results,
				Page = page,
				PageSize = pageSize,
				TotalCount = totalCount
			};
		}

		public async Task UpdateAsync(Domain.Entities.Seiyuu seiyuu)
		{
			_dbContext.Update(seiyuu);
			await _dbContext.SaveChangesAsync();
		}
	}
}
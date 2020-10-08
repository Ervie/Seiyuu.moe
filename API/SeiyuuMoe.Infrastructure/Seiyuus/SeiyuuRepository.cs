using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.WebEssentials;
using SeiyuuMoe.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Seiyuus
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
			await _dbContext.Seiyuus.AddAsync(seiyuu);
			await _dbContext.SaveChangesAsync();
		}

		public Task<Dictionary<Guid, long>> GetAllIdsAsync()
			=> _dbContext.Seiyuus.ToDictionaryAsync(x => x.Id, x => x.MalId);

		public Task<int> GetSeiyuuCountAsync() => _dbContext.Seiyuus.CountAsync();

		public Task<Domain.Entities.Seiyuu> GetAsync(long seiyuuMalId)
			=> _dbContext.Seiyuus.FirstOrDefaultAsync(x => x.MalId == seiyuuMalId);

		public async Task<PagedResult<Domain.Entities.Seiyuu>> GetOrderedPageByPopularityAsync(Expression<Func<Domain.Entities.Seiyuu, bool>> predicate, int page = 0, int pageSize = 10)
		{
			var entities = _dbContext.Seiyuus.Where(predicate);
			var totalCount = await entities.CountAsync();
			var results = await entities
				.OrderByDescending(x => x.Popularity)
				.Skip(page * pageSize)
				.Take(pageSize)
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
			seiyuu.ModificationDate = DateTime.UtcNow;
			_dbContext.Update(seiyuu);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<long?> GetLastSeiyuuMalId()
		{
			var lastSeiyuu = await _dbContext.Seiyuus.OrderBy(x => x.MalId).LastOrDefaultAsync();
			return lastSeiyuu?.MalId;
		}
	}
}
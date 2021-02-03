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

namespace SeiyuuMoe.Infrastructure.Seiyuus
{
	public class SeiyuuRepository : ISeiyuuRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public SeiyuuRepository(SeiyuuMoeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddAsync(Seiyuu seiyuu)
		{
			await _dbContext.Seiyuus.AddAsync(seiyuu);
			await _dbContext.SaveChangesAsync();
		}

		public Task<Dictionary<Guid, long>> GetAllIdsAsync()
			=> _dbContext.Seiyuus.ToDictionaryAsync(x => x.Id, x => x.MalId);

		public Task<int> GetSeiyuuCountAsync() => _dbContext.Seiyuus.CountAsync();

		public Task<Seiyuu> GetByMalIdAsync(long seiyuuMalId)
			=> _dbContext.Seiyuus.FirstOrDefaultAsync(x => x.MalId == seiyuuMalId);

		public Task<Seiyuu> GetByVndbIdAsync(long seiyuuVndbId)
			=> _dbContext.Seiyuus.FirstOrDefaultAsync(x => x.VndbId == seiyuuVndbId);

		public Task<Seiyuu> GetByKanjiAsync(string kanjiName)
			=> _dbContext.Seiyuus.FirstOrDefaultAsync(x => x.KanjiName == kanjiName);

		public async Task<PagedResult<Seiyuu>> GetOrderedPageByPopularityAsync(Expression<Func<Seiyuu, bool>> predicate, int page = 0, int pageSize = 10)
		{
			var entities = _dbContext.Seiyuus.Where(predicate);
			var totalCount = await entities.CountAsync();
			var results = await entities
				.OrderByDescending(x => x.Popularity)
				.Skip(page * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return new PagedResult<Seiyuu>
			{
				Results = results,
				Page = page,
				PageSize = pageSize,
				TotalCount = totalCount
			};
		}

		public async Task UpdateAsync(Seiyuu seiyuu)
		{
			seiyuu.ModificationDate = DateTime.UtcNow;
			_dbContext.Update(seiyuu);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<long?> GetLastSeiyuuMalIdAsync()
		{
			var lastSeiyuu = await _dbContext.Seiyuus.OrderBy(x => x.MalId).LastOrDefaultAsync();
			return lastSeiyuu?.MalId;
		}

		public async Task<IReadOnlyList<Seiyuu>> GetOlderThanModifiedDateAsync(DateTime olderThan, int pageSize = 150)
			=> await _dbContext.Seiyuus
				.Where(x => x.ModificationDate < olderThan)
				.OrderBy(x => x.ModificationDate)
				.ThenBy(x => x.Id)
				.Take(pageSize)
				.ToListAsync();

		public Task<List<long>> GetAllVndbIdsAsync()
			=> _dbContext.Seiyuus
			.Where(x => x.VndbId.HasValue)
			.Select(x => x.VndbId.Value)
			.ToListAsync();
	}
}
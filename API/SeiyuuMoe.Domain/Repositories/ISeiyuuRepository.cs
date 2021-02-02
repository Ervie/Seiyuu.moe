using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.WebEssentials;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Repositories
{
	public interface ISeiyuuRepository
	{
		Task<Seiyuu> GetByVndbIdAsync(int seiyuuVndbId);

		Task<Seiyuu> GetByMalIdAsync(long seiyuuMalId);

		Task<Seiyuu> GetByKanjiAsync(string kanjiName);

		Task<PagedResult<Seiyuu>> GetOrderedPageByPopularityAsync(Expression<Func<Seiyuu, bool>> predicate, int page = 0, int pageSize = 10);

		Task AddAsync(Seiyuu seiyuu);

		Task UpdateAsync(Seiyuu seiyuu);

		Task<Dictionary<Guid, long>> GetAllIdsAsync();

		Task<int> GetSeiyuuCountAsync();

		Task<long?> GetLastSeiyuuMalId();

		Task<IReadOnlyList<Seiyuu>> GetOlderThanModifiedDate(DateTime olderThan, int pageSize = 150);
	}
}
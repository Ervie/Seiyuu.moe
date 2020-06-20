using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.WebEssentials;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SeiyuuMoe.Repositories.Repositories
{
	public interface ISeiyuuRepository
	{
		Task<Seiyuu> GetAsync(long seiyuuMalId);

		Task<PagedResult<Seiyuu>> GetOrderedPageAsync(Expression<Func<Seiyuu, bool>> predicate, int page, int pageSize);

		Task AddAsync(Seiyuu seiyuu);

		Task UpdateAsync(Seiyuu seiyuu);

		Task<ICollection<long>> GetAllIdsAsync();

		Task<int> GetAnimeCountAsync();
	}
}
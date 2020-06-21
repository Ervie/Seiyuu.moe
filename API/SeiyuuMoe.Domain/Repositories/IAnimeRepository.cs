using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.WebEssentials;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Repositories
{
	public interface IAnimeRepository
	{
		Task<Anime> GetAsync(long animeMalId);

		Task AddAsync(Anime anime);

		Task UpdateAsync(Anime anime);

		Task<IReadOnlyList<Anime>> GetAllAsync();

		Task<IReadOnlyList<Anime>> GetAllAsync(Expression<Func<Anime, bool>> predicate);

		Task<PagedResult<Anime>> GetOrderedPageAsync(Expression<Func<Anime, bool>> predicate, int page = 0, int pageSize = 10);

		Task<int> GetAnimeCountAsync();
	}
}
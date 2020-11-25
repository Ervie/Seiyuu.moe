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

		Task<IReadOnlyList<Anime>> GetAllBySeasonAndTypeAsync(long seasonId, AnimeTypeId animeTypeId);

		Task<PagedResult<Anime>> GetOrderedPageByPopularityAsync(Expression<Func<Anime, bool>> predicate, int page = 0, int pageSize = 10);

		Task<PagedResult<Anime>> GetOrderedPageByAsync(Expression<Func<Anime, bool>> predicate, int page = 0, int pageSize = 10);

		Task<IReadOnlyList<Anime>> GetOlderThanModifiedDate(DateTime olderThan, int pageSize = 150);

		Task<int> GetAnimeCountAsync();
	}
}
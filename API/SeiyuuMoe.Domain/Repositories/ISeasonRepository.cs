using SeiyuuMoe.Domain.Entities;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Repositories
{
	public interface ISeasonRepository
	{
		Task<AnimeSeason> GetAsync(Expression<Func<AnimeSeason, bool>> predicate);

		Task AddAsync(AnimeSeason season);
	}
}
using SeiyuuMoe.Domain.Entities;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Repositories
{
	public interface ISeasonRepository
	{
		Task<Season> GetAsync(Expression<Func<Season, bool>> predicate);

		Task AddAsync(Season season);
	}
}
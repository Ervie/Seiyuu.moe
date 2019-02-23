using SeiyuuMoe.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SeiyuuMoe.Repositories.Generic
{
	public interface IRepository<T>
	{
		Func<IQueryable<T>, IQueryable<T>> IncludeExpression { get; }

		Task<T> GetAsync(long id);

		Task<T> GetAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> includeExpression = null);

		Task<IReadOnlyList<T>> GetAllAsync();

		Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate);

		Task<PagedResult<T>> GetPageAsync(Expression<Func<T, bool>> predicate, int page, int pageSize);

		Task<PagedResult<T>> GetOrderedPageAsync(Expression<Func<T, bool>> predicate, string sortExpression, int page, int pageSize);

		Task<long> CountAsync(Expression<Func<T, bool>> predicate);

		Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

		void Add(T entity);

		void Update(T entity);

		void Delete(T entity);

		void Commit();

		Task CommitAsync();
	}
}

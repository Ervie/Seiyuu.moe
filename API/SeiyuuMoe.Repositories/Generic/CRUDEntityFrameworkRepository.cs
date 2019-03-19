using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Data.Context;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Repositories.Models;
using SeiyuuMoe.Repositories.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SeiyuuMoe.Repositories.Generic
{
	public class CRUDEntityFrameworkRepository<TEntity, TContext, TKey> : EntityFrameworkRepository<TContext>, IRepository<TEntity>
	where TEntity : class
	where TContext : ISeiyuuMoeContext
	{
		protected readonly DbSet<TEntity> DbSetEntities;

		protected Expression<Func<TEntity, TKey>> DefaultKey { get; }

		protected CRUDEntityFrameworkRepository(TContext dbContext, Expression<Func<TEntity, TKey>> defaultKey) : base(dbContext)
		{
			DefaultKey = defaultKey;
			DbSetEntities = (Context as DbContext).Set<TEntity>();
		}

		public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeExpression = null)
		{
			return includeExpression == null
				? await DbSetEntities.SingleOrDefaultAsync(predicate)
				: await includeExpression(DbSetEntities).SingleOrDefaultAsync(predicate);
		}

		public async Task<TEntity> GetAsync(long id)
		{
			return await DbSetEntities.FindAsync(id);
		}

		public async Task<IReadOnlyList<TEntity>> GetAllAsync()
		{
			return await IncludeListReferences(DbSetEntities)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
		{
			return await IncludeListReferences(DbSetEntities)
				.Where(predicate)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeExpression)
		{
			return await includeExpression(DbSetEntities)
				.Where(predicate)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<PagedResult<TEntity>> GetPageAsync(Expression<Func<TEntity, bool>> predicate, int page, int pageSize)
		{
			var entities = DbSetEntities.Where(predicate);
			var totalCount = await entities.CountAsync();
			var results = await IncludeListReferences(entities
				.Skip(page * pageSize)
				.Take(pageSize))
				.AsNoTracking()
				.ToListAsync();
			return new PagedResult<TEntity>
			{
				Results = results,
				Page = page,
				PageSize = pageSize,
				TotalCount = totalCount
			};
		}

		public async Task<PagedResult<TEntity>> GetOrderedPageAsync(
			Expression<Func<TEntity, bool>> predicate,
			string sortExpression,
			int page,
			int pageSize)
		{
			var pageIndex = page < 0 ? 0 : page;
			var entities = DbSetEntities.Where(predicate);
			var totalCount = await entities.CountAsync();
			var results = await IncludeListReferences(entities
				.SortBy(sortExpression, DefaultKey)
				.Skip(pageIndex * pageSize)
				.Take(pageSize))
				.AsNoTracking()
				.ToListAsync();
			return new PagedResult<TEntity>
			{
				Results = results,
				Page = page,
				PageSize = pageSize,
				TotalCount = totalCount
			};
		}

		public async Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate)
		{
			return await DbSetEntities.CountAsync(predicate);
		}

		public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
		{
			return await DbSetEntities.AnyAsync(predicate);
		}

		public void Add(TEntity entity)
		{
			DbSetEntities.Add(entity);
		}

		public void Update(TEntity entity)
		{
			Context.SetAttached(entity);
			Context.SetModified(entity);
		}

		public void Delete(TEntity entity)
		{
			Context.SetAttached(entity);
			Context.SetDeleted(entity);
		}

		public void Commit()
		{
			(Context as DbContext).SaveChanges();
		}

		public async Task CommitAsync()
		{
			await (Context as DbContext).SaveChangesAsync();
		}

		protected virtual IQueryable<TEntity> IncludeListReferences(IQueryable<TEntity> entities)
		{
			return entities;
		}

		public virtual Func<IQueryable<TEntity>, IQueryable<TEntity>> IncludeExpression => entity => entity;
	}
}

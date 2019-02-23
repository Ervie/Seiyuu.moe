using SeiyuuMoe.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SeiyuuMoe.Repositories.Utilities
{
	public static class SortExpressionParser
	{
		public static IOrderedQueryable<TEntity> SortBy<TEntity, TKey>(this IQueryable<TEntity> source, string sortExpression, Expression<Func<TEntity, TKey>> defaultKeySelector)
		{
			if (string.IsNullOrWhiteSpace(sortExpression))
			{
				return source.OrderBy(defaultKeySelector);
			}

			var sortExpressions = GetSortExpression(sortExpression);

			if (sortExpressions.Count < 1)
			{
				return source.OrderBy(defaultKeySelector);
			}

			try
			{
				return MultipleSort(source, sortExpressions);
			}
			catch (ArgumentException)
			{
				return source.OrderBy(defaultKeySelector);
			}
		}

		private static ICollection<SortExpression> GetSortExpression(string source) =>
			source?
			.Trim()
			.Split(',')
			.Select(sort => sort.Split(' '))
			.Where(x => x.Length == 2)
			.Select(x =>
				(new SortExpression(x[0], x[1].ToUpperInvariant() == "DESC")))
			.ToList();

		private static IOrderedQueryable<T> MultipleSort<T>(IQueryable<T> source, IEnumerable<SortExpression> sortExpressions)
		{
			var firstExpression = sortExpressions.First();
			return sortExpressions.Skip(1).Aggregate(source.OrderBy(firstExpression.Column, firstExpression.Desc), (current, expression) => current.ThenBy(expression.Column, expression.Desc));
		}

		private static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName, bool descending = false)
		{
			return OrderingHelper(source, propertyName, descending, false);
		}

		private static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName, bool descending = false)
		{
			return OrderingHelper(source, propertyName, descending, true);
		}

		private static IOrderedQueryable<T> OrderingHelper<T>(IQueryable<T> source, string propertyName, bool descending, bool anotherLevel)
		{
			var param = Expression.Parameter(typeof(T), "p");
			MemberExpression property;
			if (!propertyName.Contains('.'))
			{
				property = Expression.PropertyOrField(param, propertyName);
			}
			else
			{
				var splittedPropertyName = propertyName.Split('.');
				property = Expression.PropertyOrField(param, splittedPropertyName[0]);
				for (int i = 1; i < splittedPropertyName.Length; i++)
				{
					property = Expression.PropertyOrField(property, splittedPropertyName[i]);
				}
			}

			var sort = Expression.Lambda(property, param);

			var call = Expression.Call(
				typeof(Queryable),
				(!anotherLevel ? "OrderBy" : "ThenBy") + (descending ? "Descending" : string.Empty),
				new[] { typeof(T), property.Type },
				source.Expression,
				Expression.Quote(sort));

			return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
		}
	}
}

using System;
using System.Linq;
using System.Linq.Expressions;

namespace SeiyuuMoe.Infrastructure.Database.Utilities
{
	public static class PredicateBuilder
	{
		public static Expression<Func<T, bool>> True<T>() => f => true;

		public static Expression<Func<T, bool>> False<T>() => f => false;

		public static Expression<Func<T, bool>> True<T>(IQueryable<T> query) => f => true;

		public static Expression<Func<T, bool>> False<T>(IQueryable<T> query) => f => false;

		public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, bool condition, Func<Expression<Func<T, bool>>> second) 
			=> condition
				? first.Or(second())
				: first;

		public static Expression<Func<T, bool>> Or<T>(
			this Expression<Func<T, bool>> expr1,
			Expression<Func<T, bool>> expr2)
		{
			var secondBody = expr2.Body.Replace(expr2.Parameters[0], expr1.Parameters[0]);
			return Expression.Lambda<Func<T, bool>>(
				  Expression.OrElse(expr1.Body, secondBody), expr1.Parameters);
		}

		public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, bool condition, Func<Expression<Func<T, bool>>> second) 
			=> condition
				? first.And(second())
				: first;

		public static Expression<Func<T, bool>> And<T>(
			this Expression<Func<T, bool>> expr1,
			Expression<Func<T, bool>> expr2)
		{
			var secondBody = expr2.Body.Replace(expr2.Parameters[0], expr1.Parameters[0]);
			return Expression.Lambda<Func<T, bool>>(
				  Expression.AndAlso(expr1.Body, secondBody), expr1.Parameters);
		}

		public static Expression Replace(this Expression expression, Expression searchEx, Expression replaceEx) => new ReplaceVisitor(searchEx, replaceEx).Visit(expression);

		internal class ReplaceVisitor : ExpressionVisitor
		{
			private readonly Expression _from;
			private readonly Expression _to;

			public ReplaceVisitor(Expression from, Expression to)
			{
				_from = from;
				_to = to;
			}

			public override Expression Visit(Expression node)
			{
				return node == _from ? _to : base.Visit(node);
			}
		}
	}
}

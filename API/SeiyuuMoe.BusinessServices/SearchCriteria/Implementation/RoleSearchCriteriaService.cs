using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Infrastructure.Utilities;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.BusinessServices.SearchCriteria
{
	internal class RoleSearchCriteriaService : IRoleSearchCriteriaService
	{
		public Expression<Func<Role, bool>> BuildExpression(RoleSearchCriteria searchCriteria)
		{
			var predicate = PredicateBuilder.True<Role>();
			return searchCriteria != null ? ExtendExpressionWithSearchCriteria(predicate, searchCriteria) : predicate;
		}

		private Expression<Func<Role, bool>> ExtendExpressionWithSearchCriteria(Expression<Func<Role, bool>> predicate, RoleSearchCriteria searchCriteria)
		{
			return predicate
				.And(searchCriteria.AnimeId != null && searchCriteria.AnimeId.Count > 0, () => role => searchCriteria.AnimeId.Contains(role.AnimeId.Value))
				.And(searchCriteria.RoleTypeId.HasValue && searchCriteria.RoleTypeId.Value > 0, () => role => searchCriteria.RoleTypeId.Equals(role.RoleTypeId.Value));
		}
	}
}
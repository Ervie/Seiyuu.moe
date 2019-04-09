using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Repositories.Utilities;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.BusinessServices.SearchCriteria
{
	class RoleSearchCriteriaService: IRoleSearchCriteriaService
	{
		public Expression<Func<Role, bool>> BuildExpression(RoleSearchCriteria searchCriteria)
		{
			var predicate = PredicateBuilder.True<Role>();
			return searchCriteria != null ? ExtendExpressionWithSearchCriteria(predicate, searchCriteria) : predicate;
		}

		private Expression<Func<Role, bool>> ExtendExpressionWithSearchCriteria(Expression<Func<Role, bool>> predicate, RoleSearchCriteria searchCriteria)
		{
			return predicate
				.And(searchCriteria.AnimeId != null && searchCriteria.AnimeId.Count > 0, () => role => searchCriteria.AnimeId.Contains(role.AnimeId.Value));
		}
	}
}

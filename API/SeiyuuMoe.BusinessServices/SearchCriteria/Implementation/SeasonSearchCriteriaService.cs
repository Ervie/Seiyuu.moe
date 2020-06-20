using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Infrastructure.Utilities;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.BusinessServices.SearchCriteria
{
	internal class SeasonSearchCriteriaService : ISeasonSearchCriteriaService
	{
		public SeasonSearchCriteriaService()
		{
		}

		public Expression<Func<Season, bool>> BuildExpression(SeasonSearchCriteria searchCriteria)
		{
			var predicate = PredicateBuilder.True<Season>();
			return searchCriteria != null ? ExtendExpressionWithSearchCriteria(predicate, searchCriteria) : predicate;
		}

		private Expression<Func<Season, bool>> ExtendExpressionWithSearchCriteria(Expression<Func<Season, bool>> predicate, SeasonSearchCriteria searchCriteria)
		{
			return predicate
				.And(!string.IsNullOrWhiteSpace(searchCriteria.Name), () => season => season.Name.ToLower().Contains(searchCriteria.Name.ToLower()))
				.And(searchCriteria.Year.HasValue, () => season => season.Year.Equals(searchCriteria.Year));
		}
	}
}
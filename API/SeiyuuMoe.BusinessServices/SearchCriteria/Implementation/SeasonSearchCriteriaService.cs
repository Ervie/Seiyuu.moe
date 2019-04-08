using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Repositories.Utilities;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.BusinessServices.SearchCriteria
{
	internal class SeasonSearchCriteriaService : ISeasonSearchCriteriaService
	{
		public Expression<Func<Season, bool>> BuildExpression(SeasonSearchCriteria searchCriteria)
		{
			var predicate = PredicateBuilder.True<Season>();
			return searchCriteria != null ? ExtendExpressionWithSearchCriteria(predicate, searchCriteria) : predicate;
		}

		private Expression<Func<Season, bool>> ExtendExpressionWithSearchCriteria(Expression<Func<Season, bool>> predicate, SeasonSearchCriteria searchCriteria)
		{
			return predicate
				.And(searchCriteria.Year > 1917 && searchCriteria.Year < DateTime.Now.AddYears(1).Year, () => season => searchCriteria.Year.Equals(season.Year))
				.And(!string.IsNullOrWhiteSpace(searchCriteria.Season), () => season => season.Name.Equals(searchCriteria.Season, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}
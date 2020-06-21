using SeiyuuMoe.Application.Season.GetSeasonSummaries;
using SeiyuuMoe.Infrastructure.Utilities;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.Application.Season
{
	internal class SeasonSearchCriteriaService : ISeasonSearchCriteriaService
	{
		public Expression<Func<Domain.Entities.Season, bool>> BuildExpression(GetSeasonSummariesQuery query)
		{
			var predicate = PredicateBuilder.True<Domain.Entities.Season>();
			return query != null ? ExtendExpressionWithSearchCriteria(predicate, query) : predicate;
		}

		private Expression<Func<Domain.Entities.Season, bool>> ExtendExpressionWithSearchCriteria(Expression<Func<Domain.Entities.Season, bool>> predicate, GetSeasonSummariesQuery query)
		{
			return predicate
				.And(!string.IsNullOrWhiteSpace(query.Season), () => season => season.Name.ToLower().Contains(query.Season.ToLower()))
				.And(query.Year > 0, () => season => season.Year.Equals(query.Year));
		}
	}
}
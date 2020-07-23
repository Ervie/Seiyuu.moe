using SeiyuuMoe.Application.Seasons.GetSeasonSummaries;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Infrastructure.Utilities;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.Application.Seasons
{
	internal class SeasonSearchCriteriaService : ISeasonSearchCriteriaService
	{
		public Expression<Func<Season, bool>> BuildExpression(GetSeasonSummariesQuery query)
		{
			var predicate = PredicateBuilder.True<Domain.Entities.Season>();
			return query != null ? ExtendExpressionWithSearchCriteria(predicate, query) : predicate;
		}

		private Expression<Func<Season, bool>> ExtendExpressionWithSearchCriteria(Expression<Func<Season, bool>> predicate, GetSeasonSummariesQuery query)
		{
			return predicate
				.And(!string.IsNullOrWhiteSpace(query.Season), () => season => season.Name.ToLower().Contains(query.Season.ToLower()))
				.And(query.Year > 0, () => season => season.Year.Equals(query.Year));
		}
	}
}
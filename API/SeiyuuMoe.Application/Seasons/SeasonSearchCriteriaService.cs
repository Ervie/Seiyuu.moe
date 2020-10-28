using SeiyuuMoe.Application.Seasons.GetSeasonSummaries;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Infrastructure.Utilities;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.Application.Seasons
{
	internal class SeasonSearchCriteriaService : ISeasonSearchCriteriaService
	{
		public Expression<Func<AnimeSeason, bool>> BuildExpression(GetSeasonSummariesQuery query)
		{
			var predicate = PredicateBuilder.True<AnimeSeason>();
			return query != null ? ExtendExpressionWithSearchCriteria(predicate, query) : predicate;
		}

		private Expression<Func<AnimeSeason, bool>> ExtendExpressionWithSearchCriteria(Expression<Func<AnimeSeason, bool>> predicate, GetSeasonSummariesQuery query)
		{
			return predicate
				.And(!string.IsNullOrWhiteSpace(query.Season), () => season => season.Name.ToLower().Contains(query.Season.ToLower()))
				.And(query.Year > 0, () => season => season.Year.Equals(query.Year));
		}
	}
}
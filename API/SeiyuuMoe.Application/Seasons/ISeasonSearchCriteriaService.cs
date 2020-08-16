using SeiyuuMoe.Application.Seasons.GetSeasonSummaries;
using SeiyuuMoe.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.Application.Seasons
{
	public interface ISeasonSearchCriteriaService
	{
		Expression<Func<AnimeSeason, bool>> BuildExpression(GetSeasonSummariesQuery query);
	}
}
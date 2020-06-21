using SeiyuuMoe.Application.Season.GetSeasonSummaries;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.Application.Season
{
	public interface ISeasonSearchCriteriaService
	{
		Expression<Func<Domain.Entities.Season, bool>> BuildExpression(GetSeasonSummariesQuery query);
	}
}
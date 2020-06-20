using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.BusinessServices.SearchCriteria
{
	public interface ISeasonSearchCriteriaService
	{
		Expression<Func<Season, bool>> BuildExpression(SeasonSearchCriteria searchCriteria);
	}
}
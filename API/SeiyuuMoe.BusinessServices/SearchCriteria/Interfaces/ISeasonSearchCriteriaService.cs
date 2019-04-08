using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Data.Model;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.BusinessServices.SearchCriteria
{
	public interface ISeasonSearchCriteriaService
	{
		Expression<Func<Season, bool>> BuildExpression(SeasonSearchCriteria searchCriteria);
	}
}
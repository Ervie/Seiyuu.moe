using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.BusinessServices.SearchCriteria
{
	public interface ISeiyuuSearchCriteriaService
	{
		Expression<Func<Seiyuu, bool>> BuildExpression(SeiyuuSearchCriteria searchCriteria);
	}
}
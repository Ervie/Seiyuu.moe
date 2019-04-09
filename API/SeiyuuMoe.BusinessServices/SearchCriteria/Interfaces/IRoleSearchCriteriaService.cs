using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Data.Model;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.BusinessServices.SearchCriteria
{
	public interface IRoleSearchCriteriaService
	{
		Expression<Func<Role, bool>> BuildExpression(RoleSearchCriteria searchCriteria);
	}
}
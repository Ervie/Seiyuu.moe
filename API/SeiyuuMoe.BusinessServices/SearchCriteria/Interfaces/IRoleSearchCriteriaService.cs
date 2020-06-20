using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.BusinessServices.SearchCriteria
{
	public interface IRoleSearchCriteriaService
	{
		Expression<Func<Role, bool>> BuildExpression(RoleSearchCriteria searchCriteria);
	}
}
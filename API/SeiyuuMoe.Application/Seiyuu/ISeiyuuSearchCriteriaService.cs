using SeiyuuMoe.Application.Seiyuu.SearchSeiyuu;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.Application.Seiyuu
{
	public interface ISeiyuuSearchCriteriaService
	{
		Expression<Func<Domain.Entities.Seiyuu, bool>> BuildExpression(SearchSeiyuuQuery query);
	}
}
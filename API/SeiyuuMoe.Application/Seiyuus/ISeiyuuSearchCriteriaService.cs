using SeiyuuMoe.Application.Seiyuus.SearchSeiyuu;
using SeiyuuMoe.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.Application.Seiyuus
{
	public interface ISeiyuuSearchCriteriaService
	{
		Expression<Func<Seiyuu, bool>> BuildExpression(SearchSeiyuuQuery query);
	}
}
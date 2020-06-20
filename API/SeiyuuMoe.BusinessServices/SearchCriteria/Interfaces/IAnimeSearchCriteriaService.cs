using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.BusinessServices.SearchCriteria
{
	public interface IAnimeSearchCriteriaService
	{
		Expression<Func<Anime, bool>> BuildExpression(AnimeSearchCriteria searchCriteria);
	}
}
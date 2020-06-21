using SeiyuuMoe.Application.Anime.SearchAnime;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.Application
{
	public interface IAnimeSearchCriteriaService
	{
		Expression<Func<Domain.Entities.Anime, bool>> BuildExpression(SearchAnimeQuery query);
	}
}
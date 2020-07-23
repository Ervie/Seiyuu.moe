using SeiyuuMoe.Application.Animes.SearchAnime;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.Application.Animes
{
	public interface IAnimeSearchCriteriaService
	{
		Expression<Func<Domain.Entities.Anime, bool>> BuildExpression(SearchAnimeQuery query);
	}
}
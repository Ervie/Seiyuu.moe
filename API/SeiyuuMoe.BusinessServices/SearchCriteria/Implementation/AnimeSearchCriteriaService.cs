using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Repositories.Utilities;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SeiyuuMoe.BusinessServices.SearchCriteria
{
	public class AnimeSearchCriteriaService : IAnimeSearchCriteriaService
	{
		public AnimeSearchCriteriaService()
		{
		}

		public async Task<Expression<Func<Anime, bool>>> BuildExpression(AnimeSearchCriteria searchCriteria)
		{
			var predicate = PredicateBuilder.True<Anime>();
			return searchCriteria != null ? ExtendExpressionWithSearchCriteria(predicate, searchCriteria) : predicate;
		}

		private Expression<Func<Anime, bool>> ExtendExpressionWithSearchCriteria(Expression<Func<Anime, bool>> predicate, AnimeSearchCriteria searchCriteria)
		{
			return predicate
				.And(searchCriteria.MalId != null && searchCriteria.MalId.Count > 0, () => anime => searchCriteria.MalId.Contains(anime.MalId))
				.And(!string.IsNullOrWhiteSpace(searchCriteria.Title), () => anime => anime.Title.Contains(searchCriteria.Title));
		}
	}
}
using SeiyuuMoe.Application.Animes.SearchAnime;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Infrastructure.Utilities;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.Application.Animes
{
	public class AnimeSearchCriteriaService : IAnimeSearchCriteriaService
	{
		public Expression<Func<Anime, bool>> BuildExpression(SearchAnimeQuery query)
		{
			var predicate = PredicateBuilder.True<Anime>();
			return query != null ? ExtendExpressionWithSearchCriteria(predicate, query) : predicate;
		}

		private Expression<Func<Anime, bool>> ExtendExpressionWithSearchCriteria(Expression<Func<Anime, bool>> predicate, SearchAnimeQuery query)
		{
			return predicate
				.And(!string.IsNullOrWhiteSpace(query.Title), () => anime =>
					anime.Title.ToLower().Contains(query.Title.ToLower()) ||
					(!string.IsNullOrWhiteSpace(anime.KanjiTitle) && anime.KanjiTitle.ToLower().Contains(query.Title.ToLower())) ||
					(!string.IsNullOrWhiteSpace(anime.EnglishTitle) && anime.EnglishTitle.ToLower().Contains(query.Title.ToLower())) ||
					(!string.IsNullOrWhiteSpace(anime.TitleSynonyms) && anime.TitleSynonyms.ToLower().Contains(query.Title.ToLower())));
		}
	}
}
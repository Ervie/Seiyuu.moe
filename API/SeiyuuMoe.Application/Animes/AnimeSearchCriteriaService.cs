using SeiyuuMoe.Application.Animes.SearchAnime;
using SeiyuuMoe.Infrastructure.Utilities;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.Application.Animes
{
	public class AnimeSearchCriteriaService : IAnimeSearchCriteriaService
	{
		public Expression<Func<Domain.Entities.Anime, bool>> BuildExpression(SearchAnimeQuery query)
		{
			var predicate = PredicateBuilder.True<Domain.Entities.Anime>();
			return query != null ? ExtendExpressionWithSearchCriteria(predicate, query) : predicate;
		}

		private Expression<Func<Domain.Entities.Anime, bool>> ExtendExpressionWithSearchCriteria(Expression<Func<Domain.Entities.Anime, bool>> predicate, SearchAnimeQuery query)
		{
			return predicate
				.And(query.MalId != null && query.MalId.Count > 0, () => anime => query.MalId.Contains(anime.MalId))
				.And(!string.IsNullOrWhiteSpace(query.Title), () => anime =>
					anime.Title.ToLower().Contains(query.Title.ToLower()) ||
					(!string.IsNullOrWhiteSpace(anime.JapaneseTitle) && anime.JapaneseTitle.ToLower().Contains(query.Title.ToLower())) ||
					(!string.IsNullOrWhiteSpace(anime.EnglishTitle) && anime.EnglishTitle.ToLower().Contains(query.Title.ToLower())) ||
					(!string.IsNullOrWhiteSpace(anime.TitleSynonyms) && anime.TitleSynonyms.ToLower().Contains(query.Title.ToLower())))
				.And(query.SeasonId.HasValue, () => anime => query.SeasonId.Equals(anime.SeasonId.Value))
				.And(query.AnimeTypeId.HasValue && query.AnimeTypeId.Value > 0, () => anime => query.AnimeTypeId.Equals(anime.TypeId.Value));
		}
	}
}
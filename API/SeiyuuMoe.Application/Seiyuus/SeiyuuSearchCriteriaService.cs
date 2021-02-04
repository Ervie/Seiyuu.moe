using SeiyuuMoe.Application.Seiyuus.SearchSeiyuu;
using SeiyuuMoe.Infrastructure.Extensions;
using SeiyuuMoe.Infrastructure.Database.Utilities;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.Application.Seiyuus
{
	internal class SeiyuuSearchCriteriaService : ISeiyuuSearchCriteriaService
	{
		public Expression<Func<Domain.Entities.Seiyuu, bool>> BuildExpression(SearchSeiyuuQuery query)
		{
			var predicate = PredicateBuilder.True<Domain.Entities.Seiyuu>();
			return query != null ? ExtendExpressionWithSearchCriteria(predicate, query) : predicate;
		}

		private Expression<Func<Domain.Entities.Seiyuu, bool>> ExtendExpressionWithSearchCriteria(Expression<Func<Domain.Entities.Seiyuu, bool>> predicate, SearchSeiyuuQuery query)
		{
			var swappedNameSurname = query.Name.SwapWords();

			return predicate
				.And(!string.IsNullOrWhiteSpace(query.Name), () => seiyuu =>
					seiyuu.Name.ToLower().Contains(query.Name.ToLower()) ||
					seiyuu.Name.ToLower().Contains(swappedNameSurname.ToLower()) ||
					seiyuu.KanjiName.ToLower().Contains(query.Name.ToLower()));
		}
	}
}
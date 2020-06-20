using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Infrastructure.Extensions;
using SeiyuuMoe.Infrastructure.Utilities;
using System;
using System.Linq.Expressions;

namespace SeiyuuMoe.BusinessServices.SearchCriteria
{
	internal class SeiyuuSearchCriteriaService : ISeiyuuSearchCriteriaService
	{
		public SeiyuuSearchCriteriaService()
		{
		}

		public Expression<Func<Seiyuu, bool>> BuildExpression(SeiyuuSearchCriteria searchCriteria)
		{
			var predicate = PredicateBuilder.True<Seiyuu>();
			return searchCriteria != null ? ExtendExpressionWithSearchCriteria(predicate, searchCriteria) : predicate;
		}

		private Expression<Func<Seiyuu, bool>> ExtendExpressionWithSearchCriteria(Expression<Func<Seiyuu, bool>> predicate, SeiyuuSearchCriteria searchCriteria)
		{
			var swappedNameSurname = searchCriteria.Name.SwapWords();

			return predicate
				.And(!string.IsNullOrWhiteSpace(searchCriteria.Name), () => seiyuu =>
					seiyuu.Name.ToLower().Contains(searchCriteria.Name.ToLower()) ||
					seiyuu.Name.ToLower().Contains(swappedNameSurname.ToLower()) ||
					seiyuu.JapaneseName.ToLower().Contains(searchCriteria.Name.ToLower()));
		}
	}
}
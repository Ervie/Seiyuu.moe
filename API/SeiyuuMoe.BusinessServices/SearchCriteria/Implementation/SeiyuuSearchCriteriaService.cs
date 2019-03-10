using SeiyuuMoe.Common.Extensions;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Repositories.Utilities;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SeiyuuMoe.BusinessServices.SearchCriteria
{
	class SeiyuuSearchCriteriaService : ISeiyuuSearchCriteriaService
	{
		public SeiyuuSearchCriteriaService()
		{
		}

		public async Task<Expression<Func<Seiyuu, bool>>> BuildExpression(SeiyuuSearchCriteria searchCriteria)
		{
			var predicate = PredicateBuilder.True<Seiyuu>();
			return searchCriteria != null ? ExtendExpressionWithSearchCriteria(predicate, searchCriteria) : predicate;
		}

		private Expression<Func<Seiyuu, bool>> ExtendExpressionWithSearchCriteria(Expression<Func<Seiyuu, bool>> predicate, SeiyuuSearchCriteria searchCriteria)
		{
			return predicate
				.And(!string.IsNullOrWhiteSpace(searchCriteria.Name), () => seiyuu =>
					seiyuu.Name.Contains(searchCriteria.Name, StringComparison.InvariantCultureIgnoreCase) ||
					seiyuu.Name.SwapNameSurname().Contains(searchCriteria.Name, StringComparison.InvariantCultureIgnoreCase) ||
					seiyuu.JapaneseName.Contains(searchCriteria.Name, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}

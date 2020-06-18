﻿using SeiyuuMoe.Common.Extensions;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Repositories.Utilities;
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
			var swappedNameSurname = searchCriteria.Name.SwapNameSurname();

			return predicate
				.And(!string.IsNullOrWhiteSpace(searchCriteria.Name), () => seiyuu =>
					seiyuu.Name.ToLower().Contains(searchCriteria.Name.ToLower()) ||
					seiyuu.Name.ToLower().Contains(swappedNameSurname.ToLower()) ||
					seiyuu.JapaneseName.ToLower().Contains(searchCriteria.Name.ToLower()));
		}
	}
}
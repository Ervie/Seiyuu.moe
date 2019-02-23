using System;
using System.Collections.Generic;
using System.Text;

namespace SeiyuuMoe.Repositories.Models
{
	public class PagedResult<TEntity>
	{
		public IReadOnlyList<TEntity> Results { get; set; }
		public int Page { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }

		public PagedResult<T> CreateNew<T>(IReadOnlyList<T> list)
		{
			return new PagedResult<T>
			{
				Results = list,
				Page = Page,
				PageSize = PageSize,
				TotalCount = TotalCount
			};
		}
	}
}

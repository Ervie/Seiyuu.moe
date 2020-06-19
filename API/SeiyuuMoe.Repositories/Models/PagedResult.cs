using System.Collections.Generic;
using System.Linq;

namespace SeiyuuMoe.Repositories.Models
{
	public class PagedResult<TEntity>
	{
		public IReadOnlyCollection<TEntity> Results { get; set; }
		public int Page { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }

		public PagedResult<D> Map<T, D>(IEnumerable<D> mappedEntities)
			=> new PagedResult<D>
			{
				Results = mappedEntities.ToList().AsReadOnly(),
				Page = Page,
				PageSize = PageSize,
				TotalCount = TotalCount
			};
	}
}
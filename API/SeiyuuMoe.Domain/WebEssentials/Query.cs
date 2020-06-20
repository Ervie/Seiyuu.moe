namespace SeiyuuMoe.Domain.WebEssentials
{
	public class Query<T>
	{
		public T SearchCriteria { get; set; }

		public int Page { get; set; }

		public int PageSize { get; set; }

		public string SortExpression { get; set; }
	}
}
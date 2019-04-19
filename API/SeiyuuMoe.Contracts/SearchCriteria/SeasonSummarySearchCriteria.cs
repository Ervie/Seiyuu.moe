namespace SeiyuuMoe.Contracts.SearchCriteria
{
	public class SeasonSummarySearchCriteria
	{
		public long Year { get; set; }

		public string Season { get; set; }

		public long? Id { get; set; }

		public bool MainRolesOnly { get; set; }

		public bool TVSeriesOnly { get; set; }
	}
}
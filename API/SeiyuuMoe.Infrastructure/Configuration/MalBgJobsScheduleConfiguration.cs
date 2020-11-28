namespace SeiyuuMoe.Infrastructure.Configuration
{
	public class MalBgJobsScheduleConfiguration
	{
		public int UpdateBatchSize { get; set; }

		public int DelayBetweenCallsInSeconds { get; set; }

		public int DelayBetweenMessagesInSeconds { get; set; }

		public int InsertSeiyuuBatchSize { get; set; }
	}
}
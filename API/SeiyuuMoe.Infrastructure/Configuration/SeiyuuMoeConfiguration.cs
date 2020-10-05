namespace SeiyuuMoe.Infrastructure.Configuration
{
	public class SeiyuuMoeConfiguration
	{
		public string JikanUrl { get; set; }
		public DatabaseConfiguration DatabaseConfiguration { get; set; }

		public MalBgJobsScheduleConfiguration ScheduleConfiguration { get; set; }
	}
}
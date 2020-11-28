namespace SeiyuuMoe.Domain.MalUpdateData
{
	public class MalSeasonUpdateData
	{
		public int NewestSeasonYear { get; }

		public string NewestSeasonName { get; }

		public MalSeasonUpdateData(int newestSeasonYear, string newestSeasonName)
		{
			NewestSeasonYear = newestSeasonYear;
			NewestSeasonName = newestSeasonName;
		}
	}
}
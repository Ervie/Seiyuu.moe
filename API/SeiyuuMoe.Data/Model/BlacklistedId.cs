namespace SeiyuuMoe.Data.Model
{
	public class BlacklistedId
	{
		public long Id { get; set; }
		public long MalId { get; set; }
		public string EntityType { get; set; }
		public string Reason { get; set; }
	}
}
namespace SeiyuuMoe.Application.Seiyuus.GetSeiyuuCardInfo
{
	public class GetSeiyuuCardInfoQuery
	{
		public long MalId { get; }

		public GetSeiyuuCardInfoQuery(long malId)
		{
			MalId = malId;
		}
	}
}
namespace SeiyuuMoe.Application.Seiyuu.GetSeiyuuCardInfo
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
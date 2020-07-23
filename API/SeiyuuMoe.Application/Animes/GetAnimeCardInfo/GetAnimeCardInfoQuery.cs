namespace SeiyuuMoe.Application.Animes.GetAnimeCardInfo
{
	public class GetAnimeCardInfoQuery
	{
		public long MalId { get; }

		public GetAnimeCardInfoQuery(long malId)
		{
			MalId = malId;
		}
	}
}
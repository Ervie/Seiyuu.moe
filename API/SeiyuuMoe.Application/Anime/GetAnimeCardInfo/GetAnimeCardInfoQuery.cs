namespace SeiyuuMoe.Application.Anime.GetAnimeCardInfo
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
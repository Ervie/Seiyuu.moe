namespace SeiyuuMoe.JikanToDBParser
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			JikanParser jikanParser = new JikanParser();
			//JikanParser.ParseAnime();
			//JikanParser.ParseSeiyuu();
			//JikanParser.ParseSeason();
			//JikanParser.ParseSeiyuuAdditional();
			//JikanParser.ParseAnimeAdditional();
			//JikanParser.ParseSeasonAdditional();
			//JikanParser.ParseCharacter();
			//JikanParser.ParseRole();
			jikanParser.FilterNonJapanese();
		}
	}
}
using Microsoft.AspNetCore.Mvc;

namespace SeiyuuMoe.API.Controllers
{
	[Route("api/anime")]
	public class AnimeController
	{
		public AnimeController()
		{
		}

		//	[HttpGet("{query}")]
		//	public ICollection<Anime> Get(string query) =>
		//		 _dbContext.AnimeSet
		//		.Where(x => x.Title.ToLower().Contains(query.ToLower()) || x.TitleSynonyms.ToLower().Contains(query.ToLower()))
		//		.OrderByDescending(x => x.Popularity).ToList();

		//	[HttpGet]
		//	[Route("AiringDates")]
		//	public ICollection<AnimeAiring> Get([FromQuery] ICollection<long> malId) =>
		//		 _dbContext.AnimeSet
		//		.Where(x => malId.Contains(x.MalId))
		//		.Select(entry => new AnimeAiring{ MalId = entry.MalId, AiringFrom = entry.AiringFrom })
		//		.OrderByDescending(x => x.AiringFrom).ToList();
		//}
	}
}
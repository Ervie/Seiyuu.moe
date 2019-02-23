using Microsoft.AspNetCore.Mvc;
using SeiyuuMoe.Data;
using SeiyuuMoe.Data.Context;
using SeiyuuMoe.Data.Model;
using System.Collections.Generic;
using System.Linq;

namespace SeiyuuMoe.API.Controllers
{
	[Route("api/anime")]
	public class AnimeController
	{
		private readonly DatabaseContext _dbContext = new DatabaseContext();

		public AnimeController()
		{
		}

		[HttpGet("{query}")]
		public ICollection<Anime> Get(string query) =>
			 _dbContext.AnimeSet
			.Where(x => x.Title.ToLower().Contains(query.ToLower()) || x.TitleSynonyms.ToLower().Contains(query.ToLower()))
			.OrderByDescending(x => x.Popularity).ToList();

		[HttpGet]
		[Route("AiringDates")]
		public ICollection<AnimeAiring> Get([FromQuery] ICollection<long> malId) =>
			 _dbContext.AnimeSet
			.Where(x => malId.Contains(x.MalId))
			.Select(entry => new AnimeAiring{ MalId = entry.MalId, AiringFrom = entry.AiringFrom })
			.OrderByDescending(x => x.AiringFrom).ToList();
	}
}
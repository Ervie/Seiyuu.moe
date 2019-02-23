using Microsoft.AspNetCore.Mvc;
using SeiyuuMoe.Data;
using SeiyuuMoe.Data.Context;
using SeiyuuMoe.Data.Model;
using System.Collections.Generic;
using System.Linq;

namespace SeiyuuMoe.API.Controllers
{
	[Route("api/[controller]")]
	public class SeiyuuController : Controller
	{
		private readonly DatabaseContext _dbContext = new DatabaseContext();

		public SeiyuuController()
		{
		}

		[HttpGet]
		public IEnumerable<Seiyuu> Get() =>
			_dbContext.SeiyuuSet.OrderByDescending(x => x.Popularity);

		[HttpGet("{id}")]
		public Seiyuu Get(long id) =>
			_dbContext.SeiyuuSet.Find(id);
	}
}
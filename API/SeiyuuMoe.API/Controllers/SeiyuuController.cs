using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace SeiyuuMoe.API.Controllers
{
	[Route("api/[controller]")]
	public class SeiyuuController : Controller
	{
		public SeiyuuController()
		{
		}

		//[HttpGet]
		//public IEnumerable<Seiyuu> Get() =>
		//	_dbContext.SeiyuuSet.OrderByDescending(x => x.Popularity);

		//[HttpGet("{id}")]
		//public Seiyuu Get(long id) =>
		//	_dbContext.SeiyuuSet.Find(id);
	}
}
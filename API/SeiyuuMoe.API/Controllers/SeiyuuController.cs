using Microsoft.AspNetCore.Mvc;
using SeiyuuMoe.Data;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Services;
using System.Collections.Generic;
using System.Linq;

namespace SeiyuuMoe.API.Controllers
{
	[Route("api/[controller]")]
	public class SeiyuuController : Controller
	{
		private readonly DatabaseContext _dbContext = new DatabaseContext();

		private readonly JSONSerializerService _jSONSerializer;

		public SeiyuuController(JSONSerializerService jSONSerializer)
		{
			_jSONSerializer = jSONSerializer;
		}

		[HttpGet]
		public IEnumerable<Seiyuu> Get() =>
			_dbContext.Seiyuus.OrderByDescending(x => x.Popularity);

		[HttpGet("{id}")]
		public Seiyuu Get(long id) =>
			_dbContext.Seiyuus.Find(id);
	}
}
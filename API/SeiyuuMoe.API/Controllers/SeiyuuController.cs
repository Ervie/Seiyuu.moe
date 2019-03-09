using Microsoft.AspNetCore.Mvc;
using SeiyuuMoe.API.Controllers.Base;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Services;
using SeiyuuMoe.WebEssentials;
using System.Threading.Tasks;

namespace SeiyuuMoe.API.Controllers
{
	[Route("api/seiyuu")]
	public class SeiyuuController : BaseController
	{
		private readonly ISeiyuuService seiyuuService;

		public SeiyuuController(ISeiyuuService seiyuuService)
		{
			this.seiyuuService = seiyuuService;
		}

		[HttpGet]
		public Task<IActionResult> Get([FromQuery] Query<SeiyuuSearchCriteria> query)
		{
			return Handle(async () => HandleServiceResult(await seiyuuService.GetAsync(query)));
		}
	}
}
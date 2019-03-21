using Microsoft.AspNetCore.Mvc;
using SeiyuuMoe.API.Controllers.Base;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Logger;
using SeiyuuMoe.Services;
using SeiyuuMoe.WebEssentials;
using System.Threading.Tasks;

namespace SeiyuuMoe.API.Controllers
{
	[Route("api/seiyuu")]
	public class SeiyuuController : BaseController
	{
		private readonly ISeiyuuService seiyuuService;

		public SeiyuuController(ISeiyuuService seiyuuService, ILoggingService loggingService) : base(loggingService)
		{
			this.seiyuuService = seiyuuService;
		}

		[HttpGet]
		[Route("{id}")]
		public Task<IActionResult> GetCardInfo(long id)
		{
			return Handle(async () => HandleServiceResult(await seiyuuService.GetSingleAsync(id)));
		}

		[HttpGet]
		public Task<IActionResult> Get([FromQuery] Query<SeiyuuSearchCriteria> query)
		{
			return Handle(async () => HandleServiceResult(await seiyuuService.GetAsync(query)));
		}

		[HttpGet]
		[Route("Compare")]
		public Task<IActionResult> GetComparison([FromQuery] Query<RoleSearchCriteria> query)
		{
			return Handle(async () => HandleServiceResult(await seiyuuService.GetSeiyuuComparison(query)));
		}
	}
}
using Microsoft.AspNetCore.Mvc;
using SeiyuuMoe.Infrastructure.Extensions;
using SeiyuuMoe.Infrastructure.Logger;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.API.Controllers.Base
{
	public abstract class BaseController : ControllerBase
	{
		private readonly ILoggingService _loggingService;

		public BaseController(ILoggingService loggingService)
		{
			_loggingService = loggingService;
		}

		protected async Task<TEntity> HandleAsync<TEntity>(Func<Task<TEntity>> func)
		{
			try
			{
				return await func();
			}
			catch (Exception ex)
			{
				_loggingService.Log($"Internal Error: {ex.GetType().Name}, Message: {ex.GetFullMessage()}, stack trace: {ex.StackTrace}");
				throw ex;
			}
		}
	}
}
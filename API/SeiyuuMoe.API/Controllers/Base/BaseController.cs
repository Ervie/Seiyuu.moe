using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SeiyuuMoe.Common.Extensions;
using SeiyuuMoe.Logger;
using SeiyuuMoe.WebEssentials;

namespace SeiyuuMoe.API.Controllers.Base
{
    public abstract class BaseController : ControllerBase
    {
		private readonly ILoggingService loggingService;

		public BaseController(ILoggingService loggingService)
		{
			this.loggingService = loggingService;
		}

		protected async Task<IActionResult> Handle(Func<Task<IActionResult>> func)
		{
			try
			{
				return await func();
			}
			catch (Exception ex) when (EnsureExceptionFilter(ex))
			{
				ApiResult<string> result = new ApiResult<string>
				{
					Payload = null,
					ValidationErrors = new Dictionary<string, string>
					{
						{ "General", ex.Message }
					}
				};

				return StatusCode(400, result);
			}
			catch (Exception ex)
			{
				loggingService.Log($"Internal Error: {ex.GetType().Name}, Message: {ex.GetFullMessage()}, stack trace: {ex.StackTrace}");

				return StatusCode(500);
			}
		}

		protected IActionResult HandleServiceResult<TEntity>(QueryResponse<TEntity> serviceResponse)
			where TEntity : class
		{
			if (serviceResponse == null)
			{
				return StatusCode(500);
			}

			if (!serviceResponse.Found)
			{
				return NotFound();
			}

			if (serviceResponse.ValidationErrors.Any())
			{
				return StatusCode(400, MapFromServiceResponse(serviceResponse));
			}

			return Ok(MapFromServiceResponse(serviceResponse));
		}

		protected bool EnsureExceptionFilter(Exception ex) => ex is ArgumentException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException;

		protected ApiResult<TEntity> MapFromServiceResponse<TEntity>(QueryResponse<TEntity> response)
			where TEntity : class
		{
			return new ApiResult<TEntity>
			{
				ValidationErrors = response.ValidationErrors.ToDictionary(error => error.Key, error => error.Value),
				Error = response.Error,
				Payload = response.Payload
			};
		}
    }
}
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Lambda.Base
{
	public abstract class BaseLambda
	{
		public async Task InvokeAsync()
		{
			XRayTracing.Configure();

			try
			{
				await HandleAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}

		protected abstract Task HandleAsync();
	}
}
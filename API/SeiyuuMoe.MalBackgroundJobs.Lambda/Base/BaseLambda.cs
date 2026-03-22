using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Lambda.Base
{
	public abstract class BaseLambda
	{
		public async Task InvokeAsync()
		{
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
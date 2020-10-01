using Amazon.Lambda.SQSEvents;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Amazon.Lambda.SQSEvents.SQSEvent;

namespace SeiyuuMoe.MalBackgroundJobs.Lambda.Base
{
	public abstract class BaseSqsLambda
	{
		public async Task InvokeAsync(SQSEvent @event)
		{
			XRayTracing.Configure();

			try
			{
				var sqsMessage = @event.Records.Single();
				await HandleAsync(sqsMessage);
			}
			catch (Exception)
			{
				throw;
			}
		}

		protected abstract Task HandleAsync(SQSMessage message);
	}
}
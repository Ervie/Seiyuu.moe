using Amazon.Lambda.SQSEvents;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Lambda.Base
{
	public abstract class BaseSqsLambda<T>
	{
		public async Task InvokeAsync(SQSEvent @event)
		{
			XRayTracing.Configure();

			try
			{
				var sqsMessage = @event.Records.Single();

				var deserializedMessage = JsonSerializer.Deserialize<T>(sqsMessage.Body);

				await HandleAsync(deserializedMessage);
			}
			catch (Exception)
			{
				throw;
			}
		}

		protected abstract Task HandleAsync(T deserializedMessage);
	}
}
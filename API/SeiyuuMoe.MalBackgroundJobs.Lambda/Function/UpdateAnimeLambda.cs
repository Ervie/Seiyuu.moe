using SeiyuuMoe.MalBackgroundJobs.Lambda.Base;
using System;
using System.Threading.Tasks;
using static Amazon.Lambda.SQSEvents.SQSEvent;

namespace SeiyuuMoe.MalBackgroundJobs.Lambda.Function
{
	internal class UpdateAnimeLambda : BaseSqsLambda
	{
		protected async override Task HandleAsync(SQSMessage message)
		{
			Console.WriteLine($"UpdateAnimeLambda was imvoked with message {message}");
		}
	}
}
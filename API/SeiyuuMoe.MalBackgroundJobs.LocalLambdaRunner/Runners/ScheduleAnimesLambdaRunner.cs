using SeiyuuMoe.MalBackgroundJobs.Lambda.Function;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.LocalLambdaRunner.Runners
{
	internal class ScheduleAnimesLambdaRunner
	{
		public async Task RunAsync()
		{
			var lambda = new ScheduleAnimesLambda();

			await lambda.InvokeAsync();
		}
	}
}
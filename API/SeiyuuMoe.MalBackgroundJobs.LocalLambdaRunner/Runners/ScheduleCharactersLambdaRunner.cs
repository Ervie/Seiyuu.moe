using SeiyuuMoe.MalBackgroundJobs.Lambda.Function;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.LocalLambdaRunner.Runners
{
	internal class ScheduleCharactersLambdaRunner
	{
		public async Task RunAsync()
		{
			var lambda = new ScheduleCharactersLambda();

			await lambda.InvokeAsync();
		}
	}
}
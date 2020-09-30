using SeiyuuMoe.MalBackgroundJobs.Lambda.Base;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Lambda.Function
{
	public class ScheduleAnimesLambda : BaseLambda
	{
		protected async override Task HandleAsync()
		{
			Console.WriteLine("ScheduleAnimesLambda was imvoked");
		}
	}
}
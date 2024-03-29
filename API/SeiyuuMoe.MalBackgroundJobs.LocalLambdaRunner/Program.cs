﻿using SeiyuuMoe.MalBackgroundJobs.LocalLambdaRunner.Runners;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.LocalLambdaRunner
{
	internal class Program
	{
		private static async Task Main()
		{
			SetupEnvironmentalVariables();

			// var runner = new UpdateAnimeLambdaRunner();
			//var runner = new ScheduleAnimesLambdaRunner();
			var runner = new UpdateSeiyuuLambdaRunner();

			await runner.RunAsync();
		}

		private static void SetupEnvironmentalVariables()
		{
			System.Environment.SetEnvironmentVariable("AnimeToUpdateQueueUrl", "https://sqs.eu-central-1.amazonaws.com/038836351219/AnimeToUpdateQueue");
			System.Environment.SetEnvironmentVariable("CharactersToUpdateQueueUrl", "https://sqs.eu-central-1.amazonaws.com/038836351219/CharactersToUpdateQueue");
			System.Environment.SetEnvironmentVariable("EnvironmentType", "dev");
			System.Environment.SetEnvironmentVariable("StackName", "seiyuu-moe-mal-bg-jobs-dev");
		}
	}
}
using SeiyuuMoe.Infrastructure.Configuration;
using SeiyuuMoe.Infrastructure.Context;
using SeiyuuMoe.Infrastructure.Warehouse;
using SeiyuuMoe.VndbBackgroundJobs.Job.Factory;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.VndbBackgroundJobs.Job
{
	class Program
	{
		private static async Task Main()
		{
			Environment.SetEnvironmentVariable("EnvironmentType", "dev"); //for testing only
			Console.WriteLine("Starting Vndb jobs.");

			var seiyuuMoeContext = new SeiyuuMoeContext(ConfigurationReader.DatabaseConfiguration);
			var warehouseContext = new WarehouseDbContext(ConfigurationReader.WarehouseDatabaseConfiguration);

			var handlerFactory = new HandlerFactory(seiyuuMoeContext, warehouseContext);

			var jobHandlers = handlerFactory.CreateAllHandlers();

			foreach (var jobHandler in jobHandlers)
			{
				await jobHandler.HandleAsync();
			}

			Console.WriteLine("Finished Vndb job.");
		}
	}
}

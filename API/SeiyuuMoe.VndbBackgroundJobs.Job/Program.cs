using SeiyuuMoe.Infrastructure.Configuration;
using SeiyuuMoe.Infrastructure.Database.Configuration;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Warehouse;
using SeiyuuMoe.Infrastructure.Warehouse.Configuration;
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

			var seiyuuMoeContext = new SeiyuuMoeContext(DatabaseConfigurationReader.GetDatabaseConfiguration());
			var warehouseContext = new WarehouseDbContext(WarehouseConfigurationReader.GetWarehouseConfiguration());

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

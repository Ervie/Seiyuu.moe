using Microsoft.Extensions.DependencyInjection;
using SeiyuuMoe.Infrastructure.Logger;

namespace SeiyuuMoe.API.DI
{
	public static class InfrastructureServiceCollectionExtensions
	{
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
		{
			services.AddTransient<ILoggingService, LoggingService>();

			return services;
		}
	}
}

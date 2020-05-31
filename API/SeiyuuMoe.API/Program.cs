using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace SeiyuuMoe.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		private static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseKestrel()
				.UseIISIntegration()
				.ConfigureServices(services => services.AddAutofac())
				.UseStartup<Startup>()
				.UseUrls("http://localhost:5000/")
				.Build();
	}
}
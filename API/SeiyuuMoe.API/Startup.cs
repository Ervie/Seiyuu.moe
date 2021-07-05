using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SeiyuuMoe.API.DI;
using SeiyuuMoe.Application;
using SeiyuuMoe.Infrastructure.Configuration;
using System;

namespace SeiyuuMoe.API
{
	public class Startup
	{
		private IConfigurationRoot Configuration { get; }

		public ILifetimeScope AutofacContainer { get; private set; }

		public Startup()
		{
			var configurationBuilder = new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
				.AddJsonFile("appsettings.json");

			Configuration = configurationBuilder.Build();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddCors();
			services.AddOptions();
			services.AddControllers();
			services.AddSingleton<IConfiguration>(Configuration);
		}

		public void ConfigureContainer(ContainerBuilder builder)
		{
			var seiyuuMoeConfig = Configuration.GetSection("Config").Get<SeiyuuMoeConfiguration>();
			builder.RegisterInstance(seiyuuMoeConfig).As<SeiyuuMoeConfiguration>();
			builder.RegisterInstance(seiyuuMoeConfig.DatabaseConfiguration).As<DatabaseConfiguration>();

			builder.RegisterModule(new InfrastructureModule());
			builder.RegisterModule(new DatabaseModule());
			builder.RegisterModule(new ApplicationModule());
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHttpsRedirection();
			}

			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
			});

			AutofacContainer = app.ApplicationServices.GetAutofacRoot();

			app.UseCors(builder => builder
				.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader());

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
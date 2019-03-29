using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SeiyuuMoe.BusinessServices;
using SeiyuuMoe.Data;
using SeiyuuMoe.JikanToDBParser;
using SeiyuuMoe.Logger;
using SeiyuuMoe.Repositories;
using SeiyuuMoe.Services;
using System;

namespace SeiyuuMoe.API
{
	public class Startup
	{
		public IContainer ApplicationContainer { get; private set; }

		public IConfigurationRoot Configuration { get; }

		public Startup()
		{
			var configurationBuilder = new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
				.AddJsonFile("appsettings.json");

			Configuration = configurationBuilder.Build();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().AddControllersAsServices();
			services.AddSingleton<IConfiguration>(Configuration);
			services.AddHangfire(x => x.UseMemoryStorage());
			services.AddCors();

			var builder = new ContainerBuilder();

			builder.Populate(services);

			builder.RegisterModule(new ContextModule(Configuration["Config:pathToDB"]));
			builder.RegisterModule(new RepositoriesModule());
			builder.RegisterModule(new BusinessServicesModule());
			builder.RegisterModule(new ServicesModule());
			builder.RegisterModule(new LoggerModule());
			builder.RegisterModule(new JikanParserModule());

			ApplicationContainer = builder.Build();

			return new AutofacServiceProvider(this.ApplicationContainer);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
			});

			app.UseHangfireServer();
			app.UseHangfireDashboard("/api/jobs");

			SetupRecurringJobs();

			app.UseCors(builder => builder
			.AllowAnyOrigin()
			.AllowAnyMethod()
			.AllowAnyHeader()
			.AllowCredentials());

			app.UseMvc();
		}

		public void SetupRecurringJobs()
		{
			// ToDo: Declare
			RecurringJob.AddOrUpdate<IJikanParser>(jikanParser => jikanParser.UpdateSeasons(), Cron.Monthly);
			RecurringJob.AddOrUpdate<IJikanParser>(jikanParser => jikanParser.ParseRoles(), "* * * * 7");
			RecurringJob.AddOrUpdate<IJikanParser>(jikanParser => jikanParser.UpdateAllSeiyuu(), "0 0 1 * *");
			RecurringJob.AddOrUpdate<IJikanParser>(jikanParser => jikanParser.UpdateAllAnime(), "0 0 8 * *");
			RecurringJob.AddOrUpdate<IJikanParser>(jikanParser => jikanParser.UpdateAllCharacters(), "0 0 15 * *");
			RecurringJob.AddOrUpdate<IJikanParser>(jikanParser => jikanParser.InsertSeiyuu(), "0 20 * * *");
		}
	}
}
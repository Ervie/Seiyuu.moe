using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SeiyuuMoe.Application;
using SeiyuuMoe.Infrastructure;
using SeiyuuMoe.Infrastructure.Configuration;
using SeiyuuMoe.JikanToDBParser;
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
			services.AddHangfire(x => x.UseMemoryStorage(new MemoryStorageOptions
			{
				FetchNextJobTimeout = TimeSpan.FromDays(14)
			}));
		}

		public void ConfigureContainer(ContainerBuilder builder)
		{
			var seiyuuMoeConfig = Configuration.GetSection("Config").Get<SeiyuuMoeConfiguration>();
			builder.RegisterInstance(seiyuuMoeConfig).As<SeiyuuMoeConfiguration>();
			builder.RegisterInstance(seiyuuMoeConfig.DatabaseConfiguration).As<DatabaseConfiguration>();

			builder.RegisterModule(new InfrastructureModule());
			builder.RegisterModule(new DomainModule());
			builder.RegisterModule(new ApplicationModule());
			builder.RegisterModule(new JikanParserModule());
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
			app.UseHangfireServer();
			app.UseHangfireDashboard("/api/jobs");

			SetupRecurringJobs();

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

		private static void SetupRecurringJobs()
		{
			// Workaround for to never run automatically - set to run on 31st February. Expression for jobs on demand (run only manually).
			const string runNeverCronExpression = "0 0 31 2 1";

			RecurringJob.AddOrUpdate<IJikanParser>(jikanParser => jikanParser.UpdateSeasonsAsync(), Cron.Monthly);
			RecurringJob.AddOrUpdate<IJikanParser>(jikanParser => jikanParser.ParseRolesAsync(), "0 12 * * 7");
			RecurringJob.AddOrUpdate<IJikanParser>(jikanParser => jikanParser.UpdateAllSeiyuuAsync(), "0 0 1 * *");
			RecurringJob.AddOrUpdate<IJikanParser>(jikanParser => jikanParser.UpdateAllAnimeAsync(), "0 0 8 * *");
			RecurringJob.AddOrUpdate<IJikanParser>(jikanParser => jikanParser.UpdateAllCharactersAsync(), "0 0 15 * *");
			RecurringJob.AddOrUpdate<IJikanParser>(jikanParser => jikanParser.InsertNewSeiyuuAsync(), "0 0 * * 7");
			RecurringJob.AddOrUpdate<IJikanParser>(jikanParser => jikanParser.InsertOldSeiyuuAsync(), runNeverCronExpression);
		}
	}
}
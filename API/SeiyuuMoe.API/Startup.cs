using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SeiyuuMoe.BusinessServices;
using SeiyuuMoe.Data;
using SeiyuuMoe.FileHandler;
using SeiyuuMoe.FileHandler.DatabaseBackupService;
using SeiyuuMoe.JikanToDBParser;
using SeiyuuMoe.Logger;
using SeiyuuMoe.Repositories;
using SeiyuuMoe.Services;
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
			services.AddOptions();
			services.AddControllers();
			services.AddSingleton<IConfiguration>(Configuration);
			services.AddHangfire(x => x.UseMemoryStorage(new MemoryStorageOptions
			{
				FetchNextJobTimeout = TimeSpan.FromDays(14)
			}));
			services.AddCors();
		}

		public void ConfigureContainer(ContainerBuilder builder)
		{
			builder.RegisterModule(new ContextModule(Configuration["Config:pathToDB"]));
			builder.RegisterModule(new RepositoriesModule());
			builder.RegisterModule(new BusinessServicesModule());
			builder.RegisterModule(new ServicesModule());
			builder.RegisterModule(new LoggerModule());
			builder.RegisterModule(new JikanParserModule(Configuration["Config:jikanREST"]));
			builder.RegisterModule(new FileHandlerModule(Configuration["Config:pathToDB"], Configuration["Config:pathToBackupDB"]));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			app.UseHttpsRedirection();

			app.UseRouting();

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

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}

		private static void SetupRecurringJobs()
		{
			// Workaround for to never run automatically - set to run on 31st February. Expression for jobs on demand (run only manually).
			const string runNeverCronExpression = "0 0 31 2 1";

			RecurringJob.AddOrUpdate<IJikanParser>(jikanParser => jikanParser.UpdateSeasons(), Cron.Monthly);
			RecurringJob.AddOrUpdate<IJikanParser>(jikanParser => jikanParser.ParseRoles(), "0 12 * * 7");
			RecurringJob.AddOrUpdate<IJikanParser>(jikanParser => jikanParser.UpdateAllSeiyuu(), "0 0 1 * *");
			RecurringJob.AddOrUpdate<IJikanParser>(jikanParser => jikanParser.UpdateAllAnime(), "0 0 8 * *");
			RecurringJob.AddOrUpdate<IJikanParser>(jikanParser => jikanParser.UpdateAllCharacters(), "0 0 15 * *");
			RecurringJob.AddOrUpdate<IJikanParser>(jikanParser => jikanParser.InsertNewSeiyuu(), "0 0 * * 7");
			RecurringJob.AddOrUpdate<IJikanParser>(jikanParser => jikanParser.InsertOldSeiyuu(), runNeverCronExpression);

			RecurringJob.AddOrUpdate<IDatabaseBackupService>(databaseBackupService => databaseBackupService.BackupDatabase(), "0 12 3 * *");
			RecurringJob.AddOrUpdate<IDatabaseBackupService>(databaseBackupService => databaseBackupService.RestoreDatabase(), runNeverCronExpression);
		}
	}
}
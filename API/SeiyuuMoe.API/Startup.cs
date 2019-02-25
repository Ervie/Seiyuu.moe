using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SeiyuuMoe.BusinessServices;
using SeiyuuMoe.Data;
using SeiyuuMoe.Repositories;
using SeiyuuMoe.Services;
using System;

namespace SeiyuuMoe.API
{
	public class Startup
	{
		public IContainer ApplicationContainer { get; private set; }

		public IConfigurationRoot Configuration { get; }

		public Startup(IConfiguration configuration)
		{
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().AddControllersAsServices();
			services.AddCors();

			var builder = new ContainerBuilder();

			builder.Populate(services);

			builder.RegisterModule(new ContextModule());
			builder.RegisterModule(new RepositoriesModule());
			builder.RegisterModule(new BusinessServicesModule());
			builder.RegisterModule(new ServicesModule());

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


			app.UseCors(builder => builder
			.AllowAnyOrigin()
			.AllowAnyMethod()
			.AllowAnyHeader()
			.AllowCredentials());

			app.UseMvc();
		}
	}
}
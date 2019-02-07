using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SeiyuuMoe.Data;
using SeiyuuMoe.Services;
using System;

namespace SeiyuuMoe.API
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			var builder = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("configuration.json");

			Configuration = builder.Build();
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();
			services.AddCors();
			services.AddSingleton<IConfiguration>(Configuration);
			services.AddEntityFrameworkSqlite().AddDbContext<DatabaseContext>();
			services.AddSingleton<JSONSerializerService>();
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
using System;
using System.Reflection;
using DiffingAPI.Helper;
using DiffingAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace DiffingAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            //services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DiffingAPI", Version = "v1" });
            });
            services.AddDbContext<DiffContext>(
                ob =>ob.UseSqlServer(Configuration["Connection"],
                sso => sso.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name)
                )); 
            //services.AddSingleton<IDiffDataHelper, DiffDataHelper>(); 
            services.AddScoped(typeof(IDiffDataHelper), typeof(DiffDataHelper));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
         IWebHostEnvironment env,
         IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DiffingAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            serviceProvider.GetService<DiffContext>()
            .Database.GetService<IMigrator>()
            .Migrate(Configuration.GetValue<string>("Migration"));
        }
    }
}

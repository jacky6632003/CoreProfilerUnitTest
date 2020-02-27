using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreProfiler.Web;
using CoreProfilerUnitTest.Repository.Implement;
using CoreProfilerUnitTest.Repository.Interface;
using CoreProfilerUnitTest.Repository2.Helper;
using CoreProfilerUnitTest.Service.Implement;
using CoreProfilerUnitTest.Service.Infrastructure.Mapping;
using CoreProfilerUnitTest.Service.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace CoreProfilerUnitTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private string WLDOonnection { get; set; }

        private string MySQLConnection { get; set; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            services.AddControllers();

            // database connection - Wldo
            this.WLDOonnection = this.Configuration.GetConnectionString("WLDO");

            // database connection - Mysql
            this.MySQLConnection = this.Configuration.GetConnectionString("MySQL");

            //services.AddDbContext<MyContext>(options =>
            //{
            //    options.UseSqlServer(this.Configuration.GetConnectionString("WLDO"));
            //});

            //DB
            services.AddScoped<IDatabaseHelper>(x => new DatabaseHelper(WLDOonnection, MySQLConnection));

            services.AddTransient<IStationRepository, StationRepository>();

            services.AddTransient<IYoubikeService, YoubikeService>();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ASP.NET Core 3.1 Sample",
                    Version = "v1"
                });

                // Set the comments path for the Swagger JSON and UI.
                var basePath = AppContext.BaseDirectory;
                var xmlFiles = Directory.EnumerateFiles(basePath, "*.xml", SearchOption.TopDirectoryOnly);

                foreach (var xmlFile in xmlFiles)
                {
                    options.IncludeXmlComments(xmlFile);
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCoreProfiler(true);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "ASP.NET Core 3.1 Sample V1"); });

            app.UseReDoc(options =>
            {
                options.SpecUrl("/swagger/v1/swagger.json");
                options.RoutePrefix = "redoc";
                options.DocumentTitle = "ASP.NET Core 3.1 Sample V1";
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
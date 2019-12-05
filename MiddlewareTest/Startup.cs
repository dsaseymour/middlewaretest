using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiddlewareTest.Controllers;
using Serilog;
using Microsoft.OpenApi.Models;

namespace MiddlewareTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddMvcCore().AddXmlSerializerFormatters().AddJsonFormatters();
            //Register Swagger Generator, Define Swagger Documents
            services.AddSwaggerGen((options) => {
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo {
                        Title="Chillflix API",
                        Version="v1",
                        Description="Chillflix an ASP.NET Core Application",
                        Contact = new OpenApiContact
                        {
                            Name = "Danny Seymour",
                            Url = new Uri("https://dannyseymour.me"),
                        }
                    }
                    );
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSerilogRequestLogging();
            app.ConfigureCustomExceptionMiddleware();
            app.UseHttpsRedirection();
            //Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();
            //Enable middleware to server swagger ui
            //Specify Swagger JSON Endpoint
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json","ChillFlix APIV1");
            });
            app.UseMvc();
        }
    }
}

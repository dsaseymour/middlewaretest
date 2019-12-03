using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace MiddlewareTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //setup config begins
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            //setup config ends

            //configure logger begins
            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
            //configure logger ends

            try
            {
                Log.Information("Application Starting Up");
                CreateWebHostBuilder(args).Build().Run();
                
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application Failed to Start");
                
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
               .UseSerilog();
          
    }
}

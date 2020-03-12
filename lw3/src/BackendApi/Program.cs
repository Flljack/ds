using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;

namespace BackendApi
{
    public class Program
    {
        //private static ConfigJsonService _configService;
        public static void Main(string[] args)//, ConfigJsonService configService)
        {
            CreateHostBuilder(args).Build().Run();
            //Program._configService = configService;
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {   //var configq2 = _configService.Config;
                    var config = new ConfigurationBuilder()  
                        .SetBasePath(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName + "/config")  
                        .AddJsonFile("config.json", optional: false)  
                        .Build();
                    //Console.WriteLine(configq2.GetValue<int>("BackendAPI:port"));
                    webBuilder.ConfigureKestrel(options =>
                    {
                        // Setup a HTTP/2 endpoint without TLS.
                        options.ListenLocalhost(config.GetValue<int>("BackendAPI:port"), o => o.Protocols =
                                    HttpProtocols.Http2);
                    }); 
                    webBuilder.UseUrls($"https://*:{config.GetValue<int>("BackendAPI:port")}", $"http://*:{config.GetValue<int>("BackendAPI:port")}");
                    webBuilder.UseStartup<Startup>();
                });

    }
}

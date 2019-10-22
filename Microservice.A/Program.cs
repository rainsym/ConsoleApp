using Elmah.Io.Extensions.Logging;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Microservice.A
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging((ctx, logging) =>
                {
                    logging.AddElmahIo(options =>
                    {
                        options.ApiKey = "7fd051f574ba4906811dfeaf6a4eba48";
                        options.LogId = new Guid("e6fd62d7-292a-4fc3-b723-a89d66d53861");
                    });
                    logging.AddFilter<ElmahIoLoggerProvider>(null, LogLevel.Information);
                });
    }
}

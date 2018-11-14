using Microservice.Common.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Common.RawRabbit
{
    public static class EventSetup
    {
        private const string deployFileName = "deploy-tracker.log";

        /// <summary>
        /// Log deploy date into log file
        /// </summary>
        /// <param name="app"></param>
        public static void LogDeployDate(this IApplicationBuilder app)
        {
            var _env = app.ApplicationServices.GetService<IHostingEnvironment>();
            var versionTrackingFile = _env.ContentRootFileProvider.GetFileInfo(deployFileName);
            if (!versionTrackingFile.Exists)
            {
                File.Create($"{_env.ContentRootPath}/{deployFileName}").Dispose();
            }

            DateTime.Now.WriteJsonFile($"{_env.ContentRootPath}/{deployFileName}");
        }

        /// <summary>
        /// Check subscriber is removed or not.
        /// If a CreatedAt is less than DeployDate, subscriber should be removed
        /// </summary>
        /// <param name="app"></param>
        public static void CheckEventRegistered(this IApplicationBuilder app, string serviceName)
        {
            //get deploy date
            var _env = app.ApplicationServices.GetService<IHostingEnvironment>();
            var versionTrackingFile = _env.ContentRootFileProvider.GetFileInfo(deployFileName);
            var date = versionTrackingFile.PhysicalPath.ReadJsonFile<DateTime>();

            //delete old version
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EventDbContext>();

                context.Database.ExecuteSqlCommand($"delete RawRabbitEvent where Subscriber = {serviceName} and CreatedAt < {date}");
            }
        }
    }
}

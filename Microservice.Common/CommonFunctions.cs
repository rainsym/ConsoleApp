using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Microservice.Common
{
    public static class CommonFunctions
    {
        public static TResponse ReadJsonFile<TResponse>(this string path)
        {
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<TResponse>(json);
            }
        }

        public static void WriteJsonFile(this object model, string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(model));
        }

        public static bool IsNullOrEmpty(this string val)
        {
            return string.IsNullOrEmpty(val);
        }

        public static T ConvertJson<T>(this object obj)
        {
            if (obj == null) return default;

            var json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string GetNamespace<T>()
        {
            return typeof(T).Assembly.FullName.Split(',')[0];
        }

        public static ILogger<TResponse> GetLogging<TResponse>(this IApplicationBuilder app)
        {
            //if you want to write log the response, please add logging into your service in the starup.cs (services.AddLogging())
            var loggingFactory = app.ApplicationServices.GetService<ILoggerFactory>();
            ILogger<TResponse> logger = null;
            logger = loggingFactory != null ? loggingFactory.CreateLogger<TResponse>() : null;

            return logger;
        }

        public static FirestoreDb InitiFirestore(IHostingEnvironment env)
        {
            var fireStoreKeysFolder = Path.Combine(env.ContentRootPath, "..", "Microservice.Common", $"firestore.{env.EnvironmentName}.json");
            var cred = GoogleCredential.FromFile(fireStoreKeysFolder);
            var channel = new Channel(FirestoreClient.DefaultEndpoint.Host, FirestoreClient.DefaultEndpoint.Port, cred.ToChannelCredentials());
            var client = FirestoreClient.Create(channel);
            return FirestoreDb.Create("taskkap", client);
        }
    }
}

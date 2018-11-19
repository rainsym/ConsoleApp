using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
    }
}

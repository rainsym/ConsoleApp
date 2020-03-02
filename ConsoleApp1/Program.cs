using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var stripeAPI = new StripeAPI();
            var cardNumber = "4242424242424242";
            var expMonth = 2;
            var expYear = 2021;
            var cvc = "123";
            //var customer = stripeAPI.GetCustomer("cus_Gj5714I5g0hSJ6");
            //var customer = stripeAPI.CreateCustomer();
            //stripeAPI.Charge("cus_Gj5714I5g0hSJ6", 50, 7411);
            //stripeAPI.Refund("ch_1GBepPBqhCJKs3KhGGW4kFej", 60);
            //stripeAPI.CreateAccount("Rain", "Sym", "tanhn90@gmail.com");
            stripeAPI.Payout(100, "acct_1GI5jLABdCxpahjD", 120);

            Console.WriteLine("Done!");

            Console.ReadLine();
        }

        private static Dictionary<string, object> ConvertToDictionary(this object obj)
        {
            return obj.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(prop => prop.Name, prop => prop.GetValue(obj) == null ? "" : prop.GetValue(obj));
        }

        public static void ElasticSearch()
        {
            var settings = new ConnectionSettings(new Uri("https://search-search-rvezy-sdxhps7oh6vgvoy7f3ctpxnh7q.us-east-2.es.amazonaws.com")).DefaultIndex("rv-local");
            var client = new ElasticClient(settings);

            //client.DeleteIndexAsync("rv-local").Wait();
            //return;

            client.CreateIndex("rv-local", c => c.Mappings(mp => mp.Map<RV>(m => m.AutoMap())));

            var path = "D:\\Work\\Personal\\ConsoleApp\\ConsoleApp1\\rv.json";
            var rvs = path.ReadJsonFile<List<RV>>();

            var indexResponse = client.IndexMany(rvs);
            return;

            QueryContainer queryContainer = null;
            queryContainer &= new QueryContainerDescriptor<RV>().Match(c => c.Field(p => p.IsDeleted).Query(false.ToString().ToLower()));
            queryContainer &= new QueryContainerDescriptor<RV>().Match(c => c.Field(p => p.IsPublish).Query(true.ToString().ToLower()));

            queryContainer &= new QueryContainerDescriptor<RV>()
                                   .Bool(b =>
                                       b.Must(m =>
                                           m.Bool(mb =>
                                               mb.Filter(ft =>
                                                   ft.Nested(nt =>
                                                       nt.Path(np => np.ListAddOns)
                                                           .Query(qr =>
                                                               qr.Bool(qrb =>
                                                                   qrb.Must(qrm =>
                                                                       qrm.QueryString(q => q.DefaultField(d => d.ListAddOns[0].Name).Query("delivery"))
                                                                       ))))))));

            var scriptFields = new ScriptFields
                {
                    { "distance", new ScriptField
                        {
                            Script = new InlineScript(@"
                                    double distance = doc['point'].arcDistance(params.lat, params.lon) * 0.001;
                                    double score = doc['score'].value;
                                    if(distance <= 30) { score += 4 }
                                    if(distance > 30 && distance <= 100) { score += 3 }
                                    if(distance > 100 && distance <= 250) { score += 2 }
                                    return score;
                                ")
                            {
                                Lang = "painless",
                                Params = new FluentDictionary<string, object>
                                {
                                    { "lat", 45.517254 },
                                    { "lon", -73.515305 }
                                }
                            }
                        }
                    },
                    { "abc", new ScriptField
                        {
                            Script = new InlineScript("doc['point'].arcDistance(params.lat, params.lon) * 0.001")
                            {
                                Lang = "painless",
                                Params = new FluentDictionary<string, object>
                                {
                                    { "lat", 45.517254 },
                                    { "lon", -73.515305 }
                                }
                            }
                        }
                    }
                };

            var searchRequest = new SearchRequest<RV>
            {
                Query = queryContainer,
                //ScriptFields = scriptFields,
                //Source = new Union<bool, ISourceFilter>(true),
                From = 0,
                Size = 10,
                //Sort = new List<ISort>
                //    {
                //        new SortField
                //        {
                //            Field = "score",
                //            Order = SortOrder.Descending
                //        }
                //    }
            };

            var json = client.RequestResponseSerializer.SerializeToString(searchRequest);

            var searchResponse = client.Search<RV>(searchRequest);

            //var rvs = searchResponse.Documents;
            //foreach (var hit in searchResponse.Hits)
            //{
            //    var rv = hit.Source;
            //    var field = hit.Fields["distance"];
            //    if (field != null)
            //    {
            //        rv.Distance = field.As<JArray>().Value<double>(0);
            //    }

            //    rvs.Add(rv);
            //}

            return;
        }

        public static void UpdateElasticSearch()
        {
            var settings = new ConnectionSettings(new Uri("https://search-search-rvezy-sdxhps7oh6vgvoy7f3ctpxnh7q.us-east-2.es.amazonaws.com")).DefaultIndex("rv-local");
            var client = new ElasticClient(settings);

            var descriptor = new BulkDescriptor();
            for (int i = 0; i < 2; i++)
            {
                var id = "a306e70e-1ab1-4dde-9c58-31e677762af6";
                var score = 8.8;
                if (i == 1)
                {
                    id = "8fbfb724-8ebb-4bc0-82b0-5a1cf52fd2cc";
                    score = 10;
                }
                descriptor.Update<RV>(u => u
                                            .Id(id)
                                            .Doc(new RV { Id = new Guid(id), Score = score }));
            }

            var result = client.Bulk(descriptor);
        }

        public static TResponse ReadJsonFile<TResponse>(this string path)
        {
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<TResponse>(json);
            }
        }

        public static string ReadFile(this string path)
        {
            using (StreamReader r = new StreamReader(path))
            {
                var json = r.ReadToEnd();
                return json;
            }
        }

        public static void WriteJsonFile(this object model, string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(model));
        }

        public static string ConvertToUnSign(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }

    public class Test
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
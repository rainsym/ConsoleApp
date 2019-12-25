using Elasticsearch.Net;
using ImageMagick;
using Itenso.TimePeriod;
using Nest;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ConsoleApp1
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var amazonSQS = new AmazonSQS();
            amazonSQS.ReceiveMessage();
            amazonSQS.SendMessage(JsonConvert.SerializeObject(new LogFile { Count = 1, Name = "Tan Hoang" }));
            amazonSQS.SendMessage(JsonConvert.SerializeObject(new RV { Name = "RV 1", Length = 5, AliasName = "123/rv", HSEndDate = DateTime.Now }));

            Console.WriteLine("Done!");

            Console.ReadLine();
        }

        private static void UploadImage()
        {
            var rootPath = @"C:\Users\rainsym\Desktop\New folder\";
            var directory = new DirectoryInfo(rootPath);
            var files = directory.GetFiles().ToList();
            var paths = new List<string>
            {
                //$"{rootPath}20190103_eb0596db11724580b91f152f7ff85354.jpg",
                //$"{rootPath}IMG_20190221_133711.jpg",
                //$"{rootPath}IMG_20190221_133704.jpg",
                //$"{rootPath}20190119_3d1d7da408d64eeb9ddedd319f7fa860.jpg",
                //$"{rootPath}20190119_4a1198410c8848a38aa634917e0da1d8.jpg",
                //$"{rootPath}20190119_8bc3ef9d4c484ffdb01de47f888758c2.jpg",
                //$"{rootPath}20190119_90b81a2aa5eb4595a9e62372eeb4acb6.jpg",
                //$"{rootPath}20190119_233ae14490374f75b18a85e2a14bbc9b.jpg",
                //$"{rootPath}20190119_66986b79f8944951a43c4060932fde8a.jpg",
                //$"{rootPath}20190119_294163e7a02a4622b7c1e32f64976ef4.jpg",
                //$"{rootPath}20190119_a0ee5d391e9b4a73b373205039a92cef.jpg",
                //$"{rootPath}20190119_afeaae689fbc476b83f35a0505569f93.jpg",
                //$"{rootPath}20190119_b87857e46e024d12ab1a7a3aa0702686.jpg",
                //$"{rootPath}20190119_cfca4bfbc5b6486cbbbe289923c5312f.jpg",
                //$"{rootPath}20190119_f89cf2442e47496f991938c4a57a0957.jpg",
                //$"{rootPath}20190124_d84f8afb16f54042b5d8892624fb8877.jpg",
            };
            paths = files.Select(t => t.FullName).ToList();
            foreach (var path in paths)
            {
                var extention = Path.GetExtension(path);
                var fileName = Path.GetFileName(path).Replace(extention, "");

                var img = File.ReadAllBytes(path);
                ImageHelper.UploadImage(img, rootPath, $"{fileName}_resized{extention}");
            }
        }

        static void ConvertXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<MGResponse xmlns=\"urn: crif - messagegateway:2006 - 08 - 23\"><RI_Req_Output xmlns=\"\"><Subject /><Error><Category>2</Category><Code>212</Code><Description>FIELD LENGTH IS NOT CORRECT</Description><Value>'APPL00012608_636893932576230092'</Value></Error></RI_Req_Output></MGResponse>");

            string xpath = "MGResponse";
            var nodes = xmlDoc.SelectNodes(xpath);

            foreach (XmlNode childrenNode in nodes)
            {
                Console.WriteLine(childrenNode.SelectSingleNode("//Description").Value);
            }
        }

        private static Dictionary<string, object> ConvertToDictionary(this object obj)
        {
            return obj.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(prop => prop.Name, prop => prop.GetValue(obj) == null ? "" : prop.GetValue(obj));
        }

        static void CountLog()
        {
            var files = new List<LogFile>();
            DirectoryInfo d = new DirectoryInfo(@"D:\This PC\Desktop\drive-download-20190525T021738Z-001\");//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files
            foreach (FileInfo fileLog in Files)
            {
                string line;
                var file = new StreamReader(fileLog.FullName);
                var fileName = fileLog.Name;
                while ((line = file.ReadLine()) != null)
                {

                    if (line.Contains("URL:"))
                    {
                        var temp = files.FirstOrDefault(t => t.Name == line);
                        if (temp != null) temp.Count++;
                        else if (line.Contains("booking-documents/of-booking"))
                        {
                            GetLine(files, line, "booking-documents/of-booking");
                        }
                        else if (line.Contains("generate-download-url"))
                        {
                            GetLine(files, line, "generate-download-url");
                        }
                        else if (line.Contains("/threadId"))
                        {
                            GetLine(files, line, "/threadId");
                        }
                        else files.Add(new LogFile { Name = line, Count = 1 });
                    }
                    else if(line.Contains("Subscribe on Event"))
                    {
                        line = line.Substring(line.IndexOf('S'));
                        var ev = line.Replace("Subscribe on Event", "").Trim().Split(' ')[0];
                        var temp = files.FirstOrDefault(t => t.Name.Replace("Subscribe on Event ", "") == ev);
                        if (temp != null) temp.Count++;
                        else files.Add(new LogFile { Name = $"Subscribe on Event {ev}", Count = 1 });
                    }
                    else if (line.Contains("RabbitMQ - Request type:"))
                    {
                        var temp = files.FirstOrDefault(t => t.Name == line);
                        if (temp != null) temp.Count++;
                        else files.Add(new LogFile { Name = line, Count = 1 });
                    }
                }

                file.Close();
            }

            files = files.OrderByDescending(t => t.Count).ToList();
            Console.WriteLine($"Total request: {files.Sum(s => s.Count)}");
            foreach (var item in files)
            {
                Console.WriteLine($"{item.Count} - {item.Name}");
            }
        }

        private static void GetLine(List<LogFile> files, string line, string text)
        {
            var temp = files.FirstOrDefault(t => t.Name.Contains(text));
            if (temp != null) temp.Count++;
            else files.Add(new LogFile { Name = line, Count = 1 });
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

        public static void ConvertAreaCode()
        {
            var path = "C:\\Users\\rainsym\\Documents\\Visual Studio 2017\\Projects\\ConsoleApp1\\ConsoleApp1\\cities.json";
            var groups = path.ReadJsonFile<List<City>>().GroupBy(t => t.CITY_CODE).ToList();
            var locations = new List<Location>();
            foreach (var group in groups)
            {
                var firstItem = group.First();
                var location = new Location { City = firstItem.CITY_NAME, Code = firstItem.CITY_CODE, ZipCode = firstItem.ZIP_CODE, Districts = new List<Districts>() };
                foreach (var item in group)
                {
                    location.Districts.Add(new Districts
                    {
                        Code = item.AREA_CODE,
                        District = item.AREA_NAME
                    });
                }

                locations.Add(location);
            }

            path = "C:\\Users\\rainsym\\Documents\\Visual Studio 2017\\Projects\\ConsoleApp1\\ConsoleApp1\\cities-converted.json";
            locations.WriteJsonFile(path);
        }

        public static void UpdateLocation()
        {
            var path = "C:\\Users\\rainsym\\Documents\\Visual Studio 2017\\Projects\\ConsoleApp1\\ConsoleApp1\\locations.json";
            var locations = path.ReadJsonFile<List<Location>>();

            path = "C:\\Users\\rainsym\\Documents\\Visual Studio 2017\\Projects\\ConsoleApp1\\ConsoleApp1\\cities-converted.json";
            var cities = path.ReadJsonFile<List<Location>>();

            foreach (var location in locations)
            {
                var city = cities.FirstOrDefault(t => t.Code == location.Code);
                if (city == null) continue;

                foreach (var dis in location.Districts)
                {
                    var district = city.Districts.FirstOrDefault(t => t.District.Replace("'", " ").ToLower().Trim() == dis.District.Replace("'", " ").ToLower().Trim());
                    if (district == null)
                    {
                        district = city.Districts.FirstOrDefault(t => dis.District.Replace("'", " ").ToLower().Trim().Contains(t.District.Replace("'", " ").ToLower().Trim()));
                    }
                    if (district == null) continue;

                    dis.Code = district.Code;
                }
            }
            path = "C:\\Users\\rainsym\\Documents\\Visual Studio 2017\\Projects\\ConsoleApp1\\ConsoleApp1\\locations-updated.json";
            locations.WriteJsonFile(path);
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

        public static void PubRabbitMQ(IModel channel)
        {
            string message = "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "logs",
                                 routingKey: "",
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine(" [x] Sent {0}", message);
        }

        public static void SubRabbitMQ(IModel channel)
        {
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                              exchange: "logs",
                              routingKey: "");

            Console.WriteLine($"[{queueName}] Waiting for logs.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var receiverBody = ea.Body;
                var receiverMessage = Encoding.UTF8.GetString(receiverBody);
                Console.WriteLine($"[{queueName}] Received {0}", receiverMessage);
            };
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }
    }

    public class LogFile
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
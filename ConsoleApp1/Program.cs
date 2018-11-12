using Elasticsearch.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RawRabbit.Context;
using RawRabbit.Extensions.Client;
using RawRabbit.Extensions.MessageSequence;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleApp1
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var xml = @"<SOAP-ENV:Envelope xmlns:SOAP-ENV='http://schemas.xmlsoap.org/soap/envelope/'>
                           <SOAP-ENV:Header/>
                           <SOAP-ENV:Body>
                              <ns5:createApplicationResponse xmlns:ns5='http://schema.applicationservices.ws.pro.finnone.nucleus.com' xmlns:ns10='http://www.nucleus.com/PersonInfo' xmlns:ns11='http://www.nucleus.com/FamilyDetail' xmlns:ns12='http://www.nucleus.com/EducationDetail' xmlns:ns13='http://www.nucleus.com/IdentificationDetail' xmlns:ns14='http://www.nucleus.com/OccupationInfo' xmlns:ns15='http://www.nucleus.com/Address' xmlns:ns16='http://www.nucleus.com/CommunicationDetail' xmlns:ns17='http://www.nucleus.com/schemas/integration/leadCreationService' xmlns:ns18='http://schema.document.ws.pro.finnone.nucleus.com' xmlns:ns19='http://www.nucleus.com/schemas/InterfaceStatusUpdateService' xmlns:ns2='http://cancellation.disbursal.schema.neo.nucleus.com' xmlns:ns20='http://schema.subsequent.disbursal.ws.pro.finnone.nucleus.com' xmlns:ns21='http://schema.author.status.ws.pro.finnone.nucleus.com' xmlns:ns22='http://www.nucleus.com/schemas/CASLoanApplicationInfoService' xmlns:ns3='http://schema.loancontract.ws.pro.finnone.nucleus.com' xmlns:ns4='http://cancellation.loan.schema.neo.nucleus.com' xmlns:ns6='http://schema.cas.common.base.ws.pro.finnone.nucleus.com' xmlns:ns7='http://www.nucleus.com/EligibleAmount' xmlns:ns8='http://www.nucleus.com/ConsumerDurable' xmlns:ns9='http://schema.base.ws.pro.finnone.nucleus.com'>
                                 <ns5:applicationNumber>APPL00009054</ns5:applicationNumber>
                                 <ns5:customerNumber>CUST00013285</ns5:customerNumber>
                              </ns5:createApplicationResponse>
                           </SOAP-ENV:Body>
                        </SOAP-ENV:Envelope>";
            var stringwriter = new StringWriter();
            var xmlSerializer = new XmlSerializer(typeof(abc));
            var a = xmlSerializer.Deserialize(new StringReader(xml));

            Console.ReadLine();
        }

        static void ResizeImage()
        {
            var extention = Path.GetExtension(@"C:\Users\rainsym\Desktop\New folder\original_20181030_586a2ec464cc4e60b438836c8ed62c98.png");
            var fileArray = File.ReadAllBytes(@"C:\Users\rainsym\Desktop\New folder\original_20181030_586a2ec464cc4e60b438836c8ed62c98.png");
            ImageHelper.UploadImage(@"C:\Users\rainsym\Desktop\New folder\original_20181030_586a2ec464cc4e60b438836c8ed62c98.png", @"C:\Users\rainsym\Desktop\New folder\", $"resized{extention}");
        }

        [XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public partial class abc
        {
            [XmlElement("Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
            public body Body { get; set; }
        }

        public class body
        {
            [XmlElement("createApplicationResponse", Namespace = "http://schema.applicationservices.ws.pro.finnone.nucleus.com")]
            public createApplicationResponse createApplicationResponse { get; set; }
        }

        public class createApplicationResponse
        {
            [XmlElement("applicationNumber", Namespace = "http://schema.applicationservices.ws.pro.finnone.nucleus.com")]
            public string applicationNumber;

            [XmlElement("customerNumber", Namespace = "http://schema.applicationservices.ws.pro.finnone.nucleus.com")]
            public string customerNumber;

            [XmlElement("cifNumber", Namespace = "http://schema.applicationservices.ws.pro.finnone.nucleus.com")]
            public string cifNumber;

            [XmlElement("collateralNumber", Namespace = "http://schema.applicationservices.ws.pro.finnone.nucleus.com")]
            public string collateralNumber;

            [XmlElement("interestRate", Namespace = "http://schema.applicationservices.ws.pro.finnone.nucleus.com")]
            public decimal interestRate;

            public createApplicationResponse()
            {
            }

            public createApplicationResponse(string applicationNumber, string customerNumber, string cifNumber, string collateralNumber, decimal interestRate)
            {
                this.applicationNumber = applicationNumber;
                this.customerNumber = customerNumber;
                this.cifNumber = cifNumber;
                this.collateralNumber = collateralNumber;
                this.interestRate = interestRate;
            }
        }

        static void CountLog()
        {
            int lineCount = 0;
            int fileCount = 0;
            DirectoryInfo d = new DirectoryInfo(@"C:\Users\rainsym\Documents\Visual Studio 2017\Projects\ConsoleApp1\ConsoleApp1\logs");//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files
            foreach (FileInfo fileLog in Files)
            {
                fileCount++;
                string line;
                System.IO.StreamReader file = new System.IO.StreamReader(fileLog.FullName);
                while ((line = file.ReadLine()) != null)
                {

                    if (line.Contains("SendEmailByTemplateThroughMandrillMsg:"))
                        lineCount++;
                }

                file.Close();
            }

            Console.WriteLine($"File Count: #{fileCount}");
            Console.WriteLine($"Line Count: #{lineCount}");
        }

        public static void ElasticSearch()
        {
            var settings = new ConnectionSettings(new Uri("https://search-search-rvezy-sdxhps7oh6vgvoy7f3ctpxnh7q.us-east-2.es.amazonaws.com")).DefaultIndex("rv-local");
            var client = new ElasticClient(settings);
            //client.CreateIndex("rv-local", c => c.Mappings(mp => mp.Map<RV>(m => m.AutoMap())));

            //var path = "C:\\Users\\rainsym\\Documents\\Visual Studio 2017\\Projects\\ConsoleApp1\\ConsoleApp1\\rv.json";
            //var rvs = path.ReadJsonFile<List<RV>>();

            //var indexResponse = client.IndexMany(rvs);

            QueryContainer queryContainer = null;
            queryContainer &= new QueryContainerDescriptor<RV>().Match(c => c.Field(p => p.IsDeleted).Query(false.ToString().ToLower()));
            queryContainer &= new QueryContainerDescriptor<RV>().Match(c => c.Field(p => p.IsPublish).Query(true.ToString().ToLower()));

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
            var rvs = new List<RV>();
            foreach (var hit in searchResponse.Hits)
            {
                var rv = hit.Source;
                var field = hit.Fields["distance"];
                if (field != null)
                {
                    rv.Distance = field.As<JArray>().Value<double>(0);
                }

                rvs.Add(rv);
            }

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

        public static string FormatFullName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                return fullName;
            }

            fullName = ConvertToUnSign(fullName);
            while (fullName.Length > 19)
            {
                var result = ShortenFullName(fullName);
                if (!result.CanOptimize)
                {
                    break;
                }
                else
                {
                    fullName = result.FullName;
                }
            }

            return fullName;
        }

        public static (string FullName, bool CanOptimize) ShortenFullName(string fullName)
        {
            //there are no middle name
            var temp = fullName.Split(' ');
            if (temp.Length <= 2)
            {
                return (fullName, false);
            }

            var lastName = temp[0];
            var firstName = temp[temp.Length - 1];

            var middles = temp.Skip(1).Take(temp.Length - 2).ToList();
            var canOptimize = false;
            for (var i = 0; i < middles.Count; i++)
            {
                if (middles[i].Length > 1)
                {
                    middles[i] = middles[i].Substring(0, 1);
                    canOptimize = true;
                    break;
                }
            }

            if (!canOptimize)
            {
                return (fullName, false);
            }

            middles.Insert(0, lastName);
            middles.Add(firstName);

            return (string.Join(' ', middles), true);
        }

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

        public static string ConvertToUnSign(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
}
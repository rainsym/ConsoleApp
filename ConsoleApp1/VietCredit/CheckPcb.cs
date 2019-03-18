using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Xml.Serialization;

namespace ConsoleApp1.VietCredit
{
    public static class CheckPcb
    {
        public static void Run()
        {
            var request = new PcbRequest
            {
                Contract = new PcbContract
                {
                    CodCurrency = "VND",
                    CreditCard = new CreditCard
                    {
                        AmountCreditLimitAccount = 100000,
                        CodPaymentPeriodicity = "M"
                    },
                    DateRequestContract = DateTime.Now.ToString("MMddyyyy"),
                    FIContractCode = "APPL000124",
                    OperationType = "T"
                },
                Person = new PcbPerson
                {
                    Address = new PcbAddress
                    {
                        Additional = new Additional
                        {
                            FullAddress = "123 Gò dầu"
                        },
                        Main = new PcbMain
                        {
                            FullAddress = "456 gò dầu"
                        }
                    },
                    CountryOfBirth = "VN",
                    DateOfBirth = DateTime.Now.ToString("MMddyyyy"),
                    Gender = "M",
                    IDCard = "1234556789",
                    Name = "Rainsym"
                },
                Role = "M"
            };

            var xml = request.ToXML();
        }

        public static void SendMessageViaSocket()
        {
            var path = "D:\\Work\\Personal\\ConsoleApp\\ConsoleApp1\\pcb.xml";
            var pcb = path.ReadFile();
            var message = $"APPL00000395~{pcb}~<EOF>";
            TcpClient client = new TcpClient();

            client.Client.Connect(IPAddress.Parse("172.27.6.20"), 9999);

            client.Client.Send(Encoding.ASCII.GetBytes(message));

            ReadDataLoop(client);
        }

        private static void ReadDataLoop(TcpClient client)
        {
            var flag = true;
            while (flag)
            {
                if (!client.Connected)
                    break;

                var result = ReadData(client);
                if (result.ToLower() == "ok")
                {
                    client.Close();
                    flag = false;
                    Console.WriteLine($"Result: {result}");
                }
            }
        }

        private static string ReadData(TcpClient client)
        {
            string retVal;
            byte[] data = new byte[1024];

            NetworkStream stream = client.GetStream();


            byte[] myReadBuffer = new byte[1024];
            StringBuilder myCompleteMessage = new StringBuilder();
            int numberOfBytesRead = 0;


            do
            {
                numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);

                myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

            }
            while (stream.DataAvailable);



            retVal = myCompleteMessage.ToString();


            return retVal;
        }

        public static string ToXML(this object obj)
        {
            var stringwriter = new StringWriter();
            var xmlSerializer = new XmlSerializer(obj.GetType());
            xmlSerializer.Serialize(stringwriter, obj);
            return stringwriter.ToString();
        }

        [MessageContract(WrapperName = "RI_Req_Input", IsWrapped = true)]
        public class PcbRequest
        {
            public PcbContract Contract { get; set; }
            public PcbPerson Person { get; set; }
            public string Role { get; set; }
        }

        //[MessageContract(WrapperName = "Contract", IsWrapped = true)]
        public class PcbContract
        {
            public string FIContractCode { get; set; }
            public string DateRequestContract { get; set; }
            public string OperationType { get; set; }
            public string CodCurrency { get; set; }
            public CreditCard CreditCard { get; set; }
        }

        public class CreditCard
        {
            public string CodPaymentPeriodicity { get; set; }
            public double AmountCreditLimitAccount { get; set; }
        }

        public class Subject
        {
            public string FISubjectCode { get; set; }
            public PcbPerson Person { get; set; }
        }

        public class PcbPerson
        {
            public string Name { get; set; }
            public string Gender { get; set; }
            public string DateOfBirth { get; set; }
            public string CountryOfBirth { get; set; }
            public string IDCard { get; set; }
            public PcbAddress Address { get; set; }
        }

        public class PcbAddress
        {
            public PcbMain Main { get; set; }
            public Additional Additional { get; set; }
        }

        public class PcbMain
        {
            public string FullAddress { get; set; }
        }

        public class Additional : PcbMain { }
    }
}

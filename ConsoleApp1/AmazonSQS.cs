using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using System;

namespace ConsoleApp1
{
    public class AmazonSQS
    {
        private const string queueUrl = "https://sqs.ap-southeast-1.amazonaws.com/706495706457/taskkap-local-queue";
        private const string accessKey = "AKIA2I7TM6VMVDEMUASL";
        private const string secrectKey = "2V6Ugq37pOHKfKVljyPKmhjVX26mWCYS3wX2V93z";

        public void SendMessage(string body)
        {
            try
            {
                Console.WriteLine("SendMessage - Start");
                var sqs = new AmazonSQSClient(accessKey, secrectKey, RegionEndpoint.APSoutheast1);
                var sqsMessageRequest = new SendMessageRequest
                {
                    QueueUrl = queueUrl,
                    MessageBody = body

                };

                var response = sqs.SendMessageAsync(sqsMessageRequest).Result;
                Console.WriteLine($"SendMessage - Done: {response.HttpStatusCode}");
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void ReceiveMessage()
        {
            var sqs = new AmazonSQSClient(accessKey, secrectKey, RegionEndpoint.APSoutheast1);
            var receiveMessageRequest = new ReceiveMessageRequest { QueueUrl = queueUrl };
            var receiveMessageResponse = sqs.ReceiveMessageAsync(receiveMessageRequest).Result;
            foreach (var message in receiveMessageResponse.Messages)
            {
                Console.WriteLine($"Message body: {message.Body}");
            }
        }
    }
}

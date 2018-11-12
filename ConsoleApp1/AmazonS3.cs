using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleApp1
{
    public static class AmazonS3
    {
        static string bucketName = "kiple-files";
        static string filePath = "C:\\Users\\rainsym\\Desktop\\floor_plan_0.png";
        static IAmazonS3 client;

        public static void Upload()
        {
            using (client = new AmazonS3Client("AKIAI4YXWZLJGJQBSZ6Q", "FctcgO3mUeaTt+oXOj5WxQWXSzTkOWiuMav26dz5", Amazon.RegionEndpoint.APSoutheast1))
            {
                using (FileStream fileToUpload = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        var fileTransferUtility = new TransferUtility(client);
                        TransferUtilityUploadRequest fileTransferUtilityRequest = new TransferUtilityUploadRequest
                        {
                            BucketName = bucketName + "/store",
                            InputStream = fileToUpload,
                            Key = "abc.jpg",
                            AutoCloseStream = true,
                            StorageClass = S3StorageClass.ReducedRedundancy,
                            CannedACL = S3CannedACL.PublicRead
                        };

                        fileTransferUtility.Upload(fileTransferUtilityRequest);

                        Console.WriteLine("Upload 1 completed");
                    }
                    catch (AmazonS3Exception s3Exception)
                    {
                        Console.WriteLine(s3Exception.Message, s3Exception.InnerException);
                    }
                }
            }
        }

        public static void Delete()
        {
            using (client = new AmazonS3Client("AKIAI4YXWZLJGJQBSZ6Q", "FctcgO3mUeaTt+oXOj5WxQWXSzTkOWiuMav26dz5", Amazon.RegionEndpoint.APSoutheast1))
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = "abc.jpg"
                };

                client.DeleteObjectAsync(deleteObjectRequest).Wait();

                Console.WriteLine("Delete successfull");
            }
        }

        public static void Copy()
        {
            using (client = new AmazonS3Client("AKIAI4YXWZLJGJQBSZ6Q", "FctcgO3mUeaTt+oXOj5WxQWXSzTkOWiuMav26dz5", Amazon.RegionEndpoint.APSoutheast1))
            {
                var copyObjectRequest = new CopyObjectRequest
                {
                    SourceBucket = bucketName + "/temp",
                    SourceKey = "17d8d41345aa4185b591ba5f55ef53fc.png",
                    DestinationBucket = bucketName + "/store",
                    DestinationKey = "17d8d41345aa4185b591ba5f55ef53fc.png",
                };

                client.CopyObjectAsync(copyObjectRequest).Wait();

                Console.WriteLine("Copy successfull");
            }
        }
    }
}

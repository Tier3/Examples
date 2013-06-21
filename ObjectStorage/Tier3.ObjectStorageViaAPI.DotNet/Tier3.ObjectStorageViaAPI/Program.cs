using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//adding S3 references
using Amazon.S3;
using Amazon.S3.Model;

using System.Security.Cryptography;     //for authorization header encryption
using System.Net.Http;                  //for HttpClient
using System.Xml.Linq;                  //for XDocument    

namespace Tier3.ObjectStorageViaAPI
{
    class Program
    {
        //set credentials
        private static string adminAccessKey = System.Configuration.ConfigurationManager.AppSettings["ObjectStorageKey"];
        private static string adminAccessSecret = System.Configuration.ConfigurationManager.AppSettings["ObjectStorageSecret"];

        static void Main(string[] args)
        {

            UseS3Api();
            //UseAwsSdk();
        }

        /// <summary>
        /// Uses the REST API to talk to Tier 3 Object Storage
        /// </summary>
        private static void UseS3Api()
        {
            Console.WriteLine(":: Calling Tier 3 Object Storage through S3 API ::");
            Console.WriteLine();
            Console.WriteLine("ACTION: List all the buckets");

            //set up variables used by this method
            string s3Url = "https://ca.tier3.io/";
            XDocument resultDoc = new XDocument();

            /*
             * List buckets
             */

            string timestamp = String.Format("{0:r}", DateTime.UtcNow); //need UtcNow, not just Now or get wrong time

            //call helper method to generate correct header
            string authHeader = GenerateApiAuthHeader("GET", timestamp, string.Empty, "/", adminAccessSecret);

            //make RESTful call to S3-compliant endpoint
            using (var client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, s3Url);

                //add token to header
                request.Headers.Add("Authorization", "AWS " + adminAccessKey + ":" + authHeader);
                request.Headers.Add("x-amz-date", timestamp);

                var response = client.SendAsync(request);

                if (response.Result.IsSuccessStatusCode)
                {
                    //synchronous call
                    var responseContent = response.Result.Content;
                    //synchronous read
                    string responseString = responseContent.ReadAsStringAsync().Result;

                    //throw into XDocument to get nice XML formatting
                    resultDoc = XDocument.Parse(responseString);
                    Console.WriteLine(resultDoc.ToString());
                }
                else
                {
                    //error
                    Console.WriteLine("ERROR: " + response.Result.StatusCode.ToString());
                }
            }

            Console.ReadLine();

            /*
            * List objects in a single bucket
            */

            Console.WriteLine("ACTION: Enter the name of a bucket to open: ");
            string inputbucket = Console.ReadLine();

            //url updated to include the resource (bucket) value
            s3Url = "https://ca.tier3.io/" + inputbucket;

            timestamp = String.Format("{0:r}", DateTime.UtcNow); //need UtcNow, not just Now or get wrong time

            //call helper method to generate correct header
            authHeader = GenerateApiAuthHeader("GET", timestamp, string.Empty, "/" + inputbucket, adminAccessSecret);

            //make RESTful call to S3-compliant endpoint
            using (var client2 = new HttpClient())
            {
                HttpRequestMessage request2 = new HttpRequestMessage(HttpMethod.Get, s3Url);

                //add token to header
                request2.Headers.Add("Authorization", "AWS " + adminAccessKey + ":" + authHeader);
                request2.Headers.Add("x-amz-date", timestamp);

                var response2 = client2.SendAsync(request2);

                if (response2.Result.IsSuccessStatusCode)
                {
                    //synchronous call
                    var responseContent2 = response2.Result.Content;
                    //synchronous read
                    string responseString2 = responseContent2.ReadAsStringAsync().Result;

                    //throw into XDocument to get nice XML formatting
                    resultDoc = XDocument.Parse(responseString2);
                    Console.WriteLine(resultDoc.ToString());
                }
                else
                {
                    //error
                    Console.WriteLine("ERROR: " + response2.Result.StatusCode.ToString());
                }

                Console.ReadLine();
            }
        }

        /// <summary>
        /// Uses the AWS SDK for .NET to talk to Tier 3 Object Storage
        /// </summary>
        private static void UseAwsSdk()
        {
            Console.WriteLine(":: Calling Tier 3 Object Storage from AWS SDK for .NET ::");
            Console.WriteLine();

            //create configuration that points to different URL
            AmazonS3Config config = new AmazonS3Config()
            {
                ServiceURL = "ca.tier3.io"
            };

            AmazonS3Client client = new AmazonS3Client(adminAccessKey, adminAccessSecret, config);

            /*
             * List buckets
             */
            Console.WriteLine("ACTION: List all the buckets");
            ListBucketsResponse resp = client.ListBuckets();

            foreach (S3Bucket bucket in resp.Buckets)
            {
                Console.WriteLine("-" + bucket.BucketName);
            }

            Console.WriteLine();

            /*
             * List objects in a single bucket
             */
            Console.WriteLine("ACTION: Enter the name of a bucket to open: ");
            string inputbucket = Console.ReadLine();


            ListObjectsRequest objReq = new ListObjectsRequest() { BucketName = inputbucket };
            ListObjectsResponse objResp = client.ListObjects(objReq);

            foreach (S3Object obj in objResp.S3Objects)
            {
                Console.WriteLine("-" + obj.Key);
            }

            /*
             * Upload object to bucket
             */
            //Console.Write("Type [Enter] to upload an object to the opened bucket");
            //Console.ReadLine();

            //PutObjectRequest putReq = new PutObjectRequest() { BucketName = inputbucket, FilePath = @"C:\image.png", ContentType = "image/png" };
            //PutObjectResponse putResp = client.PutObject(putReq);

            //Console.WriteLine("Object uploaded.");
            Console.ReadLine();
        }

        /// <summary>
        /// Helper function to generate the required authentication header for messages to S3 endpoint
        /// </summary>
        private static string GenerateApiAuthHeader(string verb, string timestamp, string contentType, string resource, string secret)
        {
            string authHeader = verb + "\n" +       //HTTP verb
            "\n" +                                  //content-md5
            contentType + "\n" +                    //conten-type
            "\n" +                                  //date
            "x-amz-date:" + timestamp + "\n" +      //optionally, AMZ headers
            resource;                               //resource    

            Encoding encoding = new UTF8Encoding();
            HMACSHA1 signature = new HMACSHA1();
            signature.Key = encoding.GetBytes(secret);

            byte[] bytes = signature.ComputeHash(encoding.GetBytes(authHeader));

            return Convert.ToBase64String(bytes);
        }
    }
}

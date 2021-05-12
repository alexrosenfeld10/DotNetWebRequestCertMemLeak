using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetWebRequestCertMemLeak
{
    class Program
    {
        static void Main(string[] args)
        {
            ReconfigureAfterDelay();
            for (var i = 0; i < 50; i++)
            {
                Console.WriteLine($"Iteration {i} starting.");
                try
                {
                    MakeRequest();
                }
                catch (Exception ex)
                {
                    // Ignore the failed requests
                    Console.WriteLine($"WebRequest failed, but that's okay. Error: {ex.Message}");
                }
                finally
                {
                    Console.WriteLine($"Iteration {i} complete.");
                    Thread.Sleep(1000);
                }
            }
        }

        static void MakeRequest()
        {
            var request = WebRequest.Create("https://collector.newrelic.com");
            request.Method = "POST";
            request.ContentType = "application/octet-stream";
            using (var outputStream = request.GetRequestStream())
            {
                if (outputStream == null)
                    throw new NullReferenceException("outputStream");

                outputStream.Write(Encoding.ASCII.GetBytes("{}"), 0, (int) request.ContentLength);
            }

            using (var response = request.GetResponse())
            {
                var responseStream = response.GetResponseStream();
                if (responseStream == null)
                    throw new NullReferenceException("responseStream");
                if (response.Headers == null)
                    throw new NullReferenceException("response.Headers");

                var contentTypeEncoding = response.Headers.Get("content-encoding");
                if ("gzip".Equals(contentTypeEncoding))
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);

                using (responseStream)
                using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    var responseBody = reader.ReadLine();
                    Console.WriteLine(responseBody != null ? responseBody : "{}");
                }
            }
        }

        static void ReconfigureAfterDelay()
        {
            Task.Run(async () => {
                await Task.Delay(TimeSpan.FromSeconds(20));
                Console.WriteLine("Reconfiguring!");
                ServicePointManager.CheckCertificateRevocationList = true;
                Console.WriteLine("Reconfigured!");
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotNetNewRelicMemoryLeak.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MemoryLeakController : ControllerBase
    {
        private readonly ILogger<MemoryLeakController> _logger;
        private readonly HttpClient _httpClient;

        public MemoryLeakController(ILogger<MemoryLeakController> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            _logger.LogInformation("Reconfigurating!");
            ServicePointManager.CheckCertificateRevocationList = true;
            return Ok("ServicePointManager.CheckCertificateRevocationList = true;");
        }

        [HttpGet("gccollect")]
        public ActionResult GcCollect()
        {
            GC.Collect();
            GC.WaitForFullGCComplete();
            return Ok("GC complete");
        }

        public IDictionary<string, object> StandardFields()
        {
            return new Dictionary<string, object>() {
                {"id", Guid.NewGuid().ToString()}
            };
        }

        [HttpGet("makerequest")]
        public async Task<ActionResult> MakeRequest()
        {
            try
            {
                var agent = global::NewRelic.Api.Agent.NewRelic.GetAgent();
                var transaction = agent.CurrentTransaction;
                if (transaction != null) {
                    foreach (var kvp in StandardFields())
                    {
                        transaction.AddCustomAttribute(kvp.Key, kvp.Value);
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error decorating NewRelic transaction");
                // bury
            }

            NewRelic.Api.Agent.NewRelic.RecordCustomEvent("DotNetNewRelicMemoryLeak",
                    new Dictionary<string, object>() {
                        {"type", "Dummy"},
                        {"id", Guid.NewGuid().ToString()
                    }});

            var response = await _httpClient.GetAsync("https://collector.newrelic.com");
            return Ok(response.Content.ReadAsStringAsync());
        }

        [HttpGet("memoryinfo")]
        public ActionResult MemoryInfo()
        {
            var process = Process.GetCurrentProcess();
            var memInfo = GC.GetGCMemoryInfo();

            return Ok(new
            {
                InstanceId = Environment.MachineName,
                TotalMemory = GetBytesReadable(GC.GetTotalMemory(false)),
                TotalAllocated = GetBytesReadable(GC.GetTotalAllocatedBytes()),
                Gen1CollectionCount = GC.CollectionCount(1),
                Gen2CollectionCount = GC.CollectionCount(2),
                Gen3CollectionCount = GC.CollectionCount(3),
                HeapSize = GetBytesReadable(memInfo.HeapSizeBytes),
                MemoryLoad = GetBytesReadable(memInfo.MemoryLoadBytes),
                HighMemoryLoadThreshold = GetBytesReadable(memInfo.HighMemoryLoadThresholdBytes),
                Fragmented = GetBytesReadable(memInfo.FragmentedBytes),
                TotalAvailableMemory = GetBytesReadable(memInfo.TotalAvailableMemoryBytes),
                Handles = process.HandleCount,
                PagedMemorySize64 = GetBytesReadable(process.PagedMemorySize64),
                WorkingSet = GetBytesReadable(process.WorkingSet64),
                PrivateMemorySize = GetBytesReadable(process.PrivateMemorySize64),
                VirtualMemorySize = GetBytesReadable(process.VirtualMemorySize64),
                PeakVirtualMemorySize = GetBytesReadable(process.PeakVirtualMemorySize64),
                PeakWorkingSet = GetBytesReadable(process.PeakWorkingSet64)
            });
        }

        private static string GetBytesReadable(long i)
        {
            // Get absolute value
            long absolute_i = i < 0 ? -i : i;
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = i >> 20;
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = i >> 10;
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable = readable / 1024;
            // Return formatted number with suffix
            return readable.ToString("0.### ") + suffix;
        }
    }
}

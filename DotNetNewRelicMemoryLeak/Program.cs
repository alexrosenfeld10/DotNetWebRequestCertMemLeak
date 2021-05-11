using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using DotNetNewRelicMemoryLeak;

await Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
        webBuilder.UseUrls("http://*:5001");
    }).Build().RunAsync();

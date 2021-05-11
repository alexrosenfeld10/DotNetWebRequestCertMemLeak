using DotNetNewRelicMemoryLeak;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
        webBuilder.UseUrls("http://*:5001");
    }).Build().RunAsync();

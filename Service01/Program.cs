using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Service01
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Service 01";

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://localhost:5002")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}

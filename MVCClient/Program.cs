﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace MVCClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "MVC Client";

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://localhost:5001")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}

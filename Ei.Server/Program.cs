using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Ei.Server
{
    class Program
    {
        static void Main(string[] args) {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://10.211.55.4:5000", "http://localhost:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }
    }
}

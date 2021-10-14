using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ei.Simulation.Physiology;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ei.Server
{
    class Tester
    {
        public async Task Run() {
            
        }
    }
    class Program
    {
        static void Main(string[] args) {
            var tester = new Tester();

            //var ei = File.ReadAllText("Files/Ei.json");
            //var project = File.ReadAllText("Files/PhysiologyProject.json");

            //var handler = new EiHandler(null);
            //handler.Compile(ei);
            //handler.Run(project);

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(
                    // "http://10.211.55.4:5000", 
                    "http://localhost:5000"
                )
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
            host.Run();

            Console.ReadLine();
        }

        
    }
}

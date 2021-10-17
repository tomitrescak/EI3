using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ei.Compilation;
using Ei.Core.Ontology;
using Ei.Logs;
using Ei.Persistence.Json;
using Ei.Simulation.Behaviours;
using Ei.Simulation.Simulator;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using UnityEngine;

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
            //var handler = new EiHandler(null);
            //handler.Compile(ei);

            //var project = File.ReadAllText("Files/PhysiologyProject.json");
            //handler.Run(project);



            // init scene from code

            //var scene = new Scene();
            //scene.GameObjects = new System.Collections.Generic.List<GameObject>();

            //var managerGo = new GameObject();
            //managerGo.name = "Manager";
            //managerGo.Enabled = true;
            //managerGo.AddComponent<SimulationTimer>();
            //scene.GameObjects.Add(managerGo);


            //var projectGo = new GameObject();
            //projectGo.name = "Project";
            //projectGo.Enabled = true;

            //var project = projectGo.AddComponent<SimulationProject>();
            //scene.GameObjects.Add(projectGo);

            // init scene from text

            var source = File.ReadAllText("scene.txt");
            Scene scene = JsonConvert.DeserializeObject<Scene>(source, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.Objects,
                // SerializationBinder = knownTypesBinder
            });
            var project = scene.GameObjects.First(g => g.name == "Project").GetComponent<SimulationProject>();


            // register logging components

            Log.Register(new ConsoleLog());

            // compile institution

            var eiSource = File.ReadAllText("Files/Ei.json");
            var institution = JsonInstitutionLoader.Instance.Load(eiSource, null);
            var code = institution.GenerateAll();
            var result = Compiler.Compile(code, "DefaultInstitution", out Institution ei);

            File.WriteAllText("ei.txt", result.Code);

            project.Ei = ei;

  

            // save the current run

            string output = JsonConvert.SerializeObject(scene, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.Objects,
                // SerializationBinder = knownTypesBinder
            });
            File.WriteAllText("scene.txt", output);

            // run the game engine

            var gameEngine = new GameEngine(scene);
            gameEngine.Start();

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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ei.Compilation;
using Ei.Core.Ontology;
using Ei.Logs;
using Ei.Persistence.Json;
using Ei.Projects.Physiology;
using Ei.Projects.Physiology.Behaviours;
using Ei.Simulation.Behaviours;
using Ei.Simulation.Simulator;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using UnityEngine;

namespace Ei.Server
{
    class Tester
    {
        public async Task Run()
        {

        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var tester = new Tester();

            //var ei = File.ReadAllText("Files/Ei.json");
            //var handler = new EiHandler(null);
            //handler.Compile(ei);

            //var project = File.ReadAllText("Files/PhysiologyProject.json");
            //handler.Run(project);


            ////////////////////////////////////////////////////////////////////////////
            // init scene from code
            ////////////////////////////////////////////////////////////////////////////

            var scene = new Scene();

            // Timer

            var managerGo = new GameObject();
            managerGo.name = "Manager";
            managerGo.Enabled = true;
            var timer = managerGo.AddComponent<SimulationTimer>();
            timer.DayLengthInSeconds = 120;
            scene.GameObjects.Add(managerGo);

            // Project

            var projectGo = new GameObject();
            projectGo.name = "Project";
            projectGo.Enabled = true;

            var eiSource = File.ReadAllText("Files/Ei.json");

            var project = projectGo.AddComponent<PhysiologyProject>();
            project.Organisation = "default";
            project.InstitutionSource = eiSource;
            project.SpeedDiversity = new[] { 1f, 1f };
            project.PhysiologyDiversity = new[] { 1f, 1f };
            project.HungerTreshold = 0.5;
            project.FatigueTreshold = 5;
            project.ThirstThreshold = 0.5f;
            project.KillThirstThreshold = 2;
            project.KillHungerThreshold = 2f;
            project.RestSpeed = 1;

            scene.GameObjects.Add(projectGo);

            // Agent

            var spawnGo = new GameObject();
            spawnGo.name = "Spawn";
            var spawn = spawnGo.AddComponent<AgentSpawn>();

            var physiologyAgentGo = new GameObject();
            physiologyAgentGo.name = "Baker";
            var agent = physiologyAgentGo.AddComponent<PhysiologyBasedAgent>();
            agent.Groups = new string[][] { new[] { "Tribe", "Baker" } };
            agent.FreeTimeGoalDefinition = new[]
            {
                "Water=Min;Increment;1",
                "Bread=Min;Increment;1"
            };
            scene.Prefabs.Add(physiologyAgentGo);

            var navigation = physiologyAgentGo.AddComponent<LinearNavigation>();
            navigation.Speed = 1.47f;

            var agentProperties = new List<AgentProperties>();
            agentProperties.Add(new AgentProperties
            {
                Count = 1,
                agent = physiologyAgentGo
            });

            spawn.Agents = agentProperties.ToArray();
            scene.GameObjects.Add(spawnGo);

            // Environment

            var environmentGo = new GameObject();
            var environment = environmentGo.AddComponent<AgentEnvironment>();

            environment.Definition.MetersPerPixel = 5;
            environment.Definition.Width = 800;
            environment.Definition.Height = 800;
            environment.Definition.ActionsWithNoLocation = new EnvironmentDataAction[]
            {
            new EnvironmentDataAction
            {
                Id = "physiologyworkflow",
                Duration = 0
            }, new EnvironmentDataAction
            {
                Id = "bakerWorkflow",
                Duration = 0
            },new EnvironmentDataAction
            {
                Id = "drink",
                Duration = 60
            },  new EnvironmentDataAction
            {
                Id = "eatBread",
                Duration = 900
            }, new EnvironmentDataAction
            {
                Id = "FindWheat",
                Duration = 900
            }, new EnvironmentDataAction
            {
                Id = "FindWater",
                Duration = 900
            }, new EnvironmentDataAction
            {
                Id = "MakeBread",
                Duration = 900
            }, new EnvironmentDataAction
            {
                Id = "exchangeWorkflow",
                Duration = 0
            }, new EnvironmentDataAction
            {
                Id = "ExchangePot",
                Duration = 1000
            }
            };


            scene.GameObjects.Add(environmentGo);


            /////////////////////////////////////////////////////////////////////////////
            // init scene from text
            /////////////////////////////////////////////////////////////////////////////

            //var source = File.ReadAllText("scene.txt");
            //Scene scene = JsonConvert.DeserializeObject<Scene>(source, new JsonSerializerSettings
            //{
            //    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            //    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            //    TypeNameHandling = TypeNameHandling.Objects,
            //    // SerializationBinder = knownTypesBinder
            //});
            //var project = scene.GameObjects.First(g => g.name == "Project").GetComponent<SimulationProject>();


            // register logging components

            // Log.Register(new ConsoleLog());

            // compile institution

            //var eiSource = File.ReadAllText("Files/Ei.json");

            //var institution = JsonInstitutionLoader.Instance.Load(eiSource, null);
            //var code = institution.GenerateAll();
            //var result = Compiler.Compile(code, "DefaultInstitution", out Institution ei);

            //File.WriteAllText("ei.txt", result.Code);

            // project.Ei = ei;



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

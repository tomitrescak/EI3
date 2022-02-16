using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Reflection;
using System.Threading.Tasks;
using Ei.Compilation;
using Ei.Logs;
using Ei.Core.Ontology;
using Ei.Persistence.Json;
using Newtonsoft.Json;
using WebSocketManager;
using WebSocketManager.Common;
using Ei.Simulation.Simulator;
using Ei.Simulation.Statistics;
using UnityEngine;
using Ei.Simulation.Behaviours;
using Ei.Simulation.Behaviours.Actuators;
using Ei.Simulation.Behaviours.Environment;
using Ei.Simulation.Behaviours.Sensors;
using Ei.Simulation.Behaviours.Environment.Objects;
using Ei.Simulation.Behaviours.Reasoners;
using System.Linq;
using System.Diagnostics;

namespace Ei.Server
{

    public class EiHandler : WebSocketHandler
    {

        static int id = 0;

        public class SocketLog : ILog
        {
            public long QueryId { get; set; } = -1;
            private readonly EiHandler handler;


            public SocketLog(EiHandler handler)
            {
                this.handler = handler;
            }
            public void Log(ILogMessage message)
            {
                if (this.QueryId != -1)
                {
                    this.handler.InvokeClientMethodToAllAsync("MonitorInstitution", this.QueryId, message);
                }

                //Console.WriteLine("[LOG " + Thread.CurrentThread.ManagedThreadId + "] " + message.Code + " " + string.Join(";", message.Parameters));
                // Console.WriteLine((string.IsNullOrEmpty(message.Code) ? message.Message : (message.Code + message.Parameters)));
            }
        }

        static EiHandler()
        {
            Ei.Logs.Log.Register(new ConsoleLog());

        }

        enum ResultType
        {
            Error,
            Ok
        }

        class Result
        {
            public string ResultType;
            public string Message;
            public object Payload;
        }

        struct TypeInfo
        {
            public string Name;
            public Dictionary<string, TypeInfo> Fields;
        }

        private Institution currentEi;
        // private SimulationProject project;
        private GameEngine gameEngine;
        private SocketLog socketLog;

        //private void ListFields(FieldInfo[] fields, Dictionary<string, TypeInfo> recognised)
        //{
        //    foreach (var f in fields)
        //    {
        //        if (!Attribute.IsDefined(f, typeof(JsonIgnoreAttribute)))
        //        {
        //            Debug.WriteLine("---------------");
        //            Debug.WriteLine($"Name: {f.Name}");
        //            Debug.WriteLine($"Type: {f.FieldType.Name} {f.FieldType.AssemblyQualifiedName} {f.FieldType.Namespace} {f.FieldType.Assembly}");

        //            if (!recognised.ContainsKey(f.FieldType.AssemblyQualifiedName)) {
        //                var item = new TypeInfo
        //                {
        //                    Name = f.FieldType.Name,
        //                    Fields = new Dictionary<string, TypeInfo>()
        //                };
        //                recognised.Add(f.FieldType.AssemblyQualifiedName, item);
        //                item.Fields.Add()
        //            }

        //            Debug.WriteLine(" + Children");
        //            ListFields(f.FieldType.GetFields().Where(f => f.IsPublic).ToArray());
        //        }
        //        else
        //        {
        //            // Debug.WriteLine(String.Format("Ignore Name: {0}", f.Name));
        //        }
        //    }
        //}

        public EiHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
            this.socketLog = new SocketLog(this);
            Ei.Logs.Log.Register(this.socketLog);

            // Serialisation

            Console.WriteLine("Edit EI");
            var scene = RandomDecisionAgentWithEnvironmentScene();

            string output = JsonConvert.SerializeObject(scene, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            File.WriteAllText("./ei.json", output);

            //var deserialised = JsonConvert.DeserializeObject<Scene>(output, new JsonSerializerSettings
            //{
            //    TypeNameHandling = TypeNameHandling.Auto
            //});

            /// Reflection

            // Get all the types in your assembly. For testing purpose, I am using Aspose.3D, you can use any.
            //var allTypes = Assembly.GetAssembly(typeof(SimulationAgent)).GetTypes();

            //foreach (var myType in allTypes)
            //{
            //    // Check if this type is subclass of your base class
            //    bool isSubType = myType.IsSubclassOf(typeof(MonoBehaviour));

            //    // If it is sub-type, then print its name in Debug window.
            //    if (isSubType)
            //    {
            //        Debug.WriteLine("==============================================");
            //        Debug.WriteLine(myType.Name);
            //        // list all public fields
            //        ListFields(myType.GetFields().Where(f => f.IsPublic).ToArray());
            //    }
            //}
        }

        // state changes

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);

            var socketId = WebSocketConnectionManager.GetId(socket);

            var message = new Message()
            {
                MessageType = MessageType.Text,
                Data = $"{socketId} is now connected"
            };

            await SendMessageToAllAsync(message);
        }

        public override async Task OnDisconnected(WebSocket socket)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);

            await base.OnDisconnected(socket);

            var message = new Message()
            {
                MessageType = MessageType.Text,
                Data = $"{socketId} disconnected"
            };
            await SendMessageToAllAsync(message);
        }

        // tasks
        public async Task LoadInstitution(long queryId, string name)
        {
            Console.Write("Loading Institution");
            try
            {
                var ei = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Files/AllComponents.json"));
                await InvokeClientMethodToAllAsync("LoadInstitution", queryId, ei);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        private Scene CreateTestScene()
        {
            var scene = new Scene();

            // manager holds the timer that can simulate time

            var managerGo = new GameObject();
            managerGo.name = "Manager";
            managerGo.Enabled = true;

            var timer = managerGo.AddComponent<SimulationTimer>();
            timer.DayLengthInSeconds = 480;
            scene.GameObjects.Add(managerGo);

            // institution manager launches the institution

            // Project

            var projectGo = new GameObject();
            projectGo.name = "Project";
            projectGo.Enabled = true;

            var project = projectGo.AddComponent<SimulationProject>();
            project.Organisation = "default";
            project.Ei = this.currentEi;

            scene.GameObjects.Add(projectGo);

            // add spawn

            //var spawnGo = new GameObject();
            //spawnGo.name = "Spawn";
            //var spawn = spawnGo.AddComponent<AgentSpawn>();

            //spawn.Agents = agentProperties.ToArray();
            //scene.GameObjects.Add(spawnGo);

            // Environment

            var environmentGo = new GameObject();
            var environment = environmentGo.AddComponent<AgentEnvironment>();

            environment.Definition.MetersPerPixel = 1;
            environment.Definition.Width = 800;
            environment.Definition.Height = 800;
            environment.Definition.ActionsWithNoLocation = new NoLocationAction[]
            {
            new NoLocationAction
            {
                Id = "Die",
                Duration = 0
            }, new NoLocationAction
            {
                Id = "Empty",
                Duration = 0
            }, 
            //new EnvironmentAction
            //{
            //    Id = "Feed",
            //    Duration = 2000
            //}, 
            new NoLocationAction
            {
                Id = "Poop",
                Duration = 1000
            }
            };
            scene.GameObjects.Add(environmentGo);

            // add two gameobjects with feeding

            var feedGo = new GameObject("Feed_1");
            feedGo.transform.X = 100;
            feedGo.transform.Y = 100;

            var obj = feedGo.AddComponent<EnvironmentObject>();
            obj.Actions = new[] { new EnvironmentAction { Id = "Feed" } };
            feedGo.AddComponent<DelayAction>().DelayMs = 3000;
            scene.GameObjects.Add(feedGo);

            feedGo = new GameObject("Feed_2");
            feedGo.transform.X = 200;
            feedGo.transform.Y = 300;

            obj = feedGo.AddComponent<EnvironmentObject>(); ;
            obj.Actions = new[] { new EnvironmentAction { Id = "Feed" } };
            feedGo.AddComponent<DelayAction>().DelayMs = 3000;
            scene.GameObjects.Add(feedGo);

            return scene;
        }

        private Scene RandomDecisionAgentScene()
        {
            var scene = this.CreateTestScene();

            var agentGo = new GameObject();
            agentGo.name = "Human" + id++; ;
            var agent = agentGo.AddComponent<SimulationAgent>();
            agent.Groups = new string[][] { new[] { "Default", "Citizen" } };
            agentGo.AddComponent<Actuator>();
            agentGo.AddComponent<RandomDecisionReasoner>();
            scene.GameObjects.Add(agentGo);



            // dd the agent to the scene

            // scene.Prefabs.Add(agentGo);

            //var navigation = agentGo.AddComponent<LinearNavigation>();
            //navigation.Speed = 1.47f;

            //var agentProperties = new List<AgentProperties>();
            //agentProperties.Add(new AgentProperties
            //{
            //    Count = 1,
            //    agent = agentGo
            //});

            return scene;
        }

        private Scene RandomDecisionAgentWithEnvironmentScene()
        {
            var scene = this.CreateTestScene();

            var agentGo = new GameObject();
            agentGo.name = "Human_" + id++;
            var agent = agentGo.AddComponent<SimulationAgent>();
            agent.Groups = new string[][] { new[] { "Default", "Citizen" } };
            agentGo.AddComponent<RandomDecisionReasoner>();
            agentGo.AddComponent<EnvironmentalActuator>();
            agentGo.AddComponent<Sensor>();
            var navigation = agentGo.AddComponent<LinearNavigation>();
            navigation.SpeedKmH = 1;
            scene.GameObjects.Add(agentGo);

            return scene;
        }

        private Scene PlanningAgent()
        {
            var scene = this.CreateTestScene();

            var agentGo = new GameObject();
            agentGo.name = "Human_" + id++;
            var agent = agentGo.AddComponent<SimulationAgent>();
            agent.Groups = new string[][] { new[] { "Default", "Citizen" } };
            var planner = agentGo.AddComponent<PlanningReasoner>();
            planner.GoalDefinition = new string[] { "Age=Min;Increment;1" };
            planner.WaitForActuatorToFinish = true;
            agentGo.AddComponent<EnvironmentalActuator>();
            agentGo.AddComponent<Sensor>();
            var navigation = agentGo.AddComponent<LinearNavigation>();
            navigation.SpeedKmH = 1;
            scene.GameObjects.Add(agentGo);

            return scene;
        }

        public void Run(string projectSource)
        {
            Console.WriteLine("Running Project");

            if (this.currentEi == null)
            {
                throw new Exception("You need to compile the institution first!");
            }

            // init the project that contains parameters of the simulation
            //this.project = projectType == "Physiology" ? JsonConvert.DeserializeObject(projectSource, typeof(PhysiologyProject)) as PhysiologyProject;


            //// physiology runner is a behaviour that launches a new project
            //var physiologyProjectRunner = new EiProjectStarter();
            //physiologyProjectRunner.AgentsPerSecond = 1;
            //physiologyProjectRunner.InitProject(this.project, this.currentEi);


            //// add new game object that will be added to the scene (Experiment)
            //var go = new GameObject("EiProjectStarter");

            //go.AddComponent(physiologyProjectRunner);
            //go.Enabled = true;

            //// add behaviour to the scene
            //var scene = new Scene(new List<GameObject> {
            //    go
            //});

            if (this.gameEngine != null)
            {
                Log.Info("Game Engine", "Stopping previous instance of the game");
                this.gameEngine.Stop();
            }

            // var scene = JsonConvert.DeserializeObject(projectSource, typeof(Scene)) as Scene;
            var scene = PlanningAgent();

            //string output = JsonConvert.SerializeObject(scene);
            //File.WriteAllText("./ei.json", output);

            // initialise runner that launches current scene
            this.gameEngine = new GameEngine(scene);

            this.gameEngine.StatisticTraitUpdated +=
                (trait, point) => Console.WriteLine("Stats for " + trait.Name + ": " + point);


            // start the simulation
            this.gameEngine.Start();
        }

        //var handler = new EiHandler(null);
        //handler.Compile(ei);
        //handler.Run(project);

        public async Task RunInstitution(long queryId, string projectSource)
        {
            if (this.currentEi == null)
            {
                var result = new Result
                {
                    ResultType = ResultType.Error.ToString(),
                    Message = "Institution Not Compiled"
                };
                var json = JsonConvert.SerializeObject(result);
                await InvokeClientMethodToAllAsync("RunInstitution", queryId, json);
                return;
            }
            try
            {
                this.Run(projectSource);
            }
            catch (Exception ex)
            {
                var result = new Result
                {
                    ResultType = ResultType.Error.ToString(),
                    Message = ex.Message
                };
                var json = JsonConvert.SerializeObject(result);
                await InvokeClientMethodToAllAsync("RunInstitution", queryId, json);
            }

            //            try
            //            {
            //                var ei = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Files/AllComponents.json"));
            //                await InvokeClientMethodToAllAsync("queryResult", queryId, ei);
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.Write(ex.Message);
            //            }
        }

        public async Task MonitorInstitution(long queryId)
        {
            Console.Write("Monitoring Institution");

            this.socketLog.QueryId = queryId;

            // send the state of all current objects

            //            try
            //            {
            //                var ei = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Files/AllComponents.json"));
            //                await InvokeClientMethodToAllAsync("queryResult", queryId, ei);
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.Write(ex.Message);
            //            }
        }

        public Compiler.CompilationResult Compile(string source)
        {
            this.currentEi = null;

            var institution = JsonInstitutionLoader.Instance.Load(source, null);
            var code = institution.GenerateAll();
            var result = Compiler.Compile(code, "DefaultInstitution", out Institution TestEi);

            this.currentEi = TestEi;
            return result;
        }

        public async Task CompileInstitution(long queryId, string source, bool shouldRun)
        {
            Console.WriteLine("Compiling Institution: " + shouldRun);
            try
            {
                var result = this.Compile(source);

                var json = JsonConvert.SerializeObject(result);
                await InvokeClientMethodToAllAsync("CompileInstitution", queryId, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Compilation Failed");
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                var result = new Compiler.CompilationResult
                {
                    Success = false,
                    Code = "Parsing Error",
                    Errors = new[] {
                        new Compiler.CompilationError {
                            Code = new string[0],
                            Line = 0,
                            Message = ex.Message,
                            Severity = "error"
                        }
                    }
                };
                var json = JsonConvert.SerializeObject(result);
                await InvokeClientMethodToAllAsync("CompileInstitution", queryId, json);
            }
        }


        public async Task LoadInstitution1(string name)
        {
            var ei = File.ReadAllText("Files/AllComponents.json");
            await SendMessageToAllAsync(new Message
            {
                MessageType = MessageType.Text,
                Data = ei
            });
        }

        public async Task SendMessage(string socketId, string message)
        {
            await InvokeClientMethodToAllAsync("receiveMessage", socketId, message);
        }
    }
}

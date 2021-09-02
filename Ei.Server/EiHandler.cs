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
using Ei.Simulation.Agents.Behaviours;
using Ei.Simulation.Physiology;
using Newtonsoft.Json;
using WebSocketManager;
using WebSocketManager.Common;
using Ei.Simulation.Simulator;
using Ei.Simulation.Statistics;
using UnityEngine;

namespace Ei.Server
{

    public class EiHandler : WebSocketHandler
    {

   
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

        static EiHandler() {
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
            public string Mesasage;
            public object Payload;
        }
        
        private Institution currentEi;
        private Project project;
        private Simulation.Simulator.Runner runner;
        private SocketLog socketLog;

        public EiHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
            this.socketLog = new SocketLog(this);
            Ei.Logs.Log.Register(this.socketLog);
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

        public void Run(string projectSource) {
            Console.Write("Running Project");

            if (this.currentEi == null) {
                throw new Exception("You need to compile the institution first!");
            }
            
            // init the project that contains parameters of the simulation
            this.project = JsonConvert.DeserializeObject(projectSource, typeof(PhysiologyProject)) as PhysiologyProject;
            
            
            // physiology runner is a behaviour that launches a new project
            var physiologyProjectRunner = new EiProjectStarter();
            physiologyProjectRunner.AgentsPerSecond = 1;
            physiologyProjectRunner.InitProject(this.project, this.currentEi);
            
            
            // add new game object that will be added to the scene (Experiment)
            var go = new GameObject("EiProjectStarter");
            
            go.AddComponent(physiologyProjectRunner);
            go.Enabled = true;
            
            // add behaviour to the scene
            var scene = new Scene(new List<GameObject> {
                go
            });
            
            // initialise runner that launches current scene
            this.runner = new Runner(scene);
            this.runner.GameObjectAdded += (runner1, o) => Console.WriteLine("Added: " + o.name);
            this.runner.StatisticTraitUpdated +=
                (trait, point) => Console.WriteLine("Stats for " + trait.Name + ": " + point);
            this.runner.ProcessStatistics(new FpsStatistics());
            
            // start the simulation
            this.runner.Start();
        }
        
        public async Task RunInstitution(long queryId, string projectSource)
        {
            try {
                this.Run(projectSource);
            }
            catch (Exception ex) {
                var result = new Result {
                    ResultType = ResultType.Error.ToString(),
                    Mesasage = ex.Message
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

        public Compiler.CompilationResult Compile(string source) {
            this.currentEi = null;
                
            var institution = JsonInstitutionLoader.Instance.Load(source, null);
            var code = institution.GenerateAll();
            var result = Compiler.Compile(code, "DefaultInstitution", out Institution TestEi);
            
            this.currentEi = TestEi;
            return result;
        }
        
        public async Task CompileInstitution(long queryId, string source)
        {
            Console.Write("Compiling Institution");
            try {
                var result = this.Compile(source);
                
                var json = JsonConvert.SerializeObject(result);
                await InvokeClientMethodToAllAsync("CompileInstitution", queryId, json);
            }
            catch (Exception ex) {
                if (ex.InnerException != null) {
                    ex = ex.InnerException;
                }
                var result = new Compiler.CompilationResult {
                    Success = false,
                    Code = "Parsing Error",
                    Errors = new [] {
                        new Compiler.CompilationError {
                            Code = new string[0],
                            Line = 0,
                            Message = ex.Message,
                            Severity = "error"
                        }
                    }
                };
                var json = JsonConvert.SerializeObject(result);
                await InvokeClientMethodToAllAsync("queryResult", queryId, json);
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

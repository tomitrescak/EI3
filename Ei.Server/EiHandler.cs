using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Reflection;
using System.Threading.Tasks;
using Ei.Compilation;
using Ei.Ontology;
using Ei.Persistence.Json;
using Ei.Simulation.Physiology;
using Ei.Simulation.Physiology.Behaviours;
using Newtonsoft.Json;
using WebSocketManager;
using WebSocketManager.Common;
using Ei.Simulation.Simulator;
using UnityEngine;

namespace Ei.Server
{
    public class EiHandler : WebSocketHandler
    {
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
        
        public EiHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
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
                await InvokeClientMethodToAllAsync("queryResult", queryId, ei);
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

            // physilogy runner is simulation behaviour
            var physiologyProjectRunner = new PhysiologyProjectRunner();
            physiologyProjectRunner.InitProject(this.project);
            
            // add new game object
            var go = new GameObject("PhysiologyProjectRunner");
            go.AddComponent<PhysiologyProjectRunner>();
            
            // add behaviour to the scene
            var scene = new Scene(new List<GameObject> {
                go
            });
            
            // initialise runner
            this.runner = new Runner(scene);
            
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
                await InvokeClientMethodToAllAsync("queryResult", queryId, json);
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
            // send the state of all current objects
            Console.Write("Initialising Project");
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
        
        public async Task CompileInstitution(long queryId, string source)
        {
            Console.Write("Compiling Institution");
            try {
                this.currentEi = null;
                
                var institution = JsonInstitutionLoader.Instance.Load(source, null);
                var code = institution.GenerateAll();
                var result = Compiler.Compile(code, "DefaultInstitution", out Institution TestEi);
                var json = JsonConvert.SerializeObject(result);
                await InvokeClientMethodToAllAsync("queryResult", queryId, json);

                this.currentEi = TestEi;
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

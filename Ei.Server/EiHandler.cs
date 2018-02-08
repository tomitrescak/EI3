using System;
using System.IO;
using System.Net.WebSockets;
using System.Reflection;
using System.Threading.Tasks;
using Ei.Compilation;
using Ei.Ontology;
using Ei.Persistence.Json;
using Newtonsoft.Json;
using WebSocketManager;
using WebSocketManager.Common;

namespace Ei.Server
{
    public class EiHandler : WebSocketHandler
    {
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
        
        public async Task CompileInstitution(long queryId, string source)
        {
            Console.Write("Compiling Institution");
            try {
                var institution = JsonInstitutionLoader.Instance.Load(source, null);
                var code = institution.GenerateAll();
                var result = Compiler.Compile(code, "DefaultInstitution", out Institution TestEi);
                var json = JsonConvert.SerializeObject(result);
                await InvokeClientMethodToAllAsync("queryResult", queryId, json);
            }
            catch (Exception ex)
            {
                await InvokeClientMethodToAllAsync("queryResult", queryId, "{ \"result\": \"" + ex.Message + "\"}");
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

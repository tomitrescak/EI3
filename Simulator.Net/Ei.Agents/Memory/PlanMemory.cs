using Ei.Logs;
using Ei.Core.Ontology;
using Ei.Core.Runtime;
using Ei.Core.Runtime.Planning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;

namespace Ei.Planning.Memory
{
    public static class PlanMemory
    {
        const string plansdb = "plansDb.yaml";

        public static List<MemoryItem> Plans { get; set; }

        private static StreamWriter sw;
        private static Serializer ser;
        private static Deserializer dser;

        static object locker = new object();

        static PlanMemory() {
            dser = new Deserializer();
            ser = new Serializer(SerializationOptions.Roundtrip);
            if (File.Exists(plansdb)) {
                var file = File.ReadAllText(plansdb);
                Plans = dser.Deserialize<List<MemoryItem>>(file);
            }

            if (Plans == null) {
                Plans = new List<MemoryItem>();
            }
            sw = File.AppendText(plansdb);

        }

        public static List<AStarNode> FindPlan(Governor agent, string group, WorkflowPosition agentPosition, GoalState[] goals) {
            if (agentPosition == null) {
                return null;
            }

            lock (locker) {
                var plan = Plans.FirstOrDefault(p =>
                    // all goals are fulfiled
                    p.Role == group &&
                    p.StartStateId == agentPosition.Id &&
                    p.StartWorkflowId == agentPosition.Workflow.Id &&
                    goals.All(g => p.Goals.Any(pg => pg.Fulfils(g)))
                );
                Log.Debug(agent.Name, "Planning", "Found Plan: " + plan == null ? "NONE" : "YES");
                if (plan != null) {
                    return plan.RetrieveMemoryPlan(agent, agentPosition);
                }
            }


            return null;
        }

        public static void AddPlan(string role, WorkflowPosition position, GoalState[] goals, List<AStarNode> plan) {
            lock (locker) {
                var memoryItem = new MemoryItem(role, position, goals, plan);
                if (Plans.All(p => !p.Equals(memoryItem))) {
                    Plans.Add(memoryItem);

                    var newser = new List<MemoryItem> { memoryItem };
                    var result = ser.Serialize(newser);
                    sw.WriteLine(result.Substring(result.IndexOf('\n') + 1));
                    sw.Flush();
                }
            }
        }
    }
}
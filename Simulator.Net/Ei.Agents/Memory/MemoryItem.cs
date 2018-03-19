using Ei.Ontology;
using Ei.Ontology.Actions;
using Ei.Runtime;
using Ei.Runtime.Planning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Planning.Memory
{
    public class MemoryItem
    {
        private List<AStarNode> plan;

        public string Role { get; set; }
        public string StartStateId { get; set; }
        public string StartWorkflowId { get; set; }
        public Goal[] Goals { get; set; }
        public List<string> Connections { get; set; }

        public MemoryItem() { }

        public MemoryItem(string role, WorkflowPosition position, GoalState[] goals, List<AStarNode> plan) {
            if (plan[0].Arc.Action is ActionExitWorkflow) {
                string m = "4";
            }
            this.Role = role;
            this.Goals = goals.Select(g => new Goal(g)).ToArray();
            this.Connections = plan.Select(i => i.Arc.Action is ActionExitWorkflow ? "ExitWorkflow" : i.Arc.Id).Where(i => i != null).ToList();
            this.StartStateId = position.Id;
            this.StartWorkflowId = position.Workflow.Id;
        }

        // institution

        public List<AStarNode> RetrieveMemoryPlan(Governor agent, WorkflowPosition startPosition) {
            if (plan == null) {
                plan = new List<AStarNode>();
                // add initial connection
                plan.Add(new AStarNode(new Connection(null, null, startPosition)));
                // add all remaining connections
                var position = startPosition;

                // rememeber all join workflows
                Stack<Connection> joinWorkflowConnections = new Stack<Connection>();
                joinWorkflowConnections.Push(agent.CurrentContext.Connection);

                foreach (var connection in this.Connections) {
                    // deal with exit workflows
                    if (connection == "ExitWorkflow") {
                        plan.Add(new AStarNode(new Connection(null, null, null, null, new ActionExitWorkflow())));

                        // if we have used a join workflow connection we pop it from current context
                        var pos = joinWorkflowConnections.Pop();
                        position = pos.To;

                        continue;
                    }

                    // find connection
                    var conn = position.Outs.FirstOrDefault(o => o.Id == connection);
                    if (conn == null) {

                        // try exit
                        plan.Add(new AStarNode(new Connection(null, null, null, null, new ActionExitWorkflow())));

                        var pos = joinWorkflowConnections.Pop();
                        if (pos != null) {
                            position = pos.To;
                            conn = position.Outs.FirstOrDefault(o => o.Id == connection);
                        } else return plan;
                    }

                    // if it is a workflow action we 
                    var workflowConnection = conn.Action as ActionJoinWorkflow;
                    if (workflowConnection != null) {
                        position = workflowConnection.TestWorkflow.Start;
                        joinWorkflowConnections.Push(conn);
                    } else {
                        position = conn.To;
                    }
                    plan.Add(new AStarNode(conn));
                }
            }
            return plan;
        }

        public override bool Equals(object obj) {
            var mi = obj as MemoryItem;
            if (obj == null) {
                return false;
            }
            if (this.Role != mi.Role) {
                return false;
            }
            if (mi.Goals.Any(g => this.Goals.All(tg => !tg.Fulfils(g)))) {
                return false;
            }
            if (mi.Connections.Count != this.Connections.Count) {
                return false;
            }
            for (var i = 0; i < this.Connections.Count; i++) {
                if (this.Connections[i] != mi.Connections[i]) {
                    return false;
                }
            }
            return true;


        }
    }
}

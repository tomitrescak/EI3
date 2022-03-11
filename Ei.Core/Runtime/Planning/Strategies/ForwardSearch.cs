using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ei.Core.Ontology;
using Ei.Core.Runtime;
using Ei.Core.Runtime.Planning.Heuristics;

namespace Ei.Core.Runtime.Planning.Strategies
{
    internal class ForwardSearch : IStrategy
    {
        //private AStarNode initialNode;
        
        protected Group[] Groups { get; set; }

        internal ForwardSearch(WorkflowPosition agentPosition, Governor.GovernorState agentState, Workflow.Store workflowState, Group[] agentGroups)
        {
            this.Groups = agentGroups;

            this.InitialNode = new AStarNode(new Connection(null, null, agentPosition));
            this.InitialNode.Parent = null;
            this.InitialNode.Resources = agentState;
            this.InitialNode.WorkflowState = workflowState.Clone();
        }

        // properties

       

        public AStarNode InitialNode { get; }

        // methods

        public virtual SearchableState CloneProperties(SearchableState source)
        {
            return source.Clone();
        }

        public virtual Connection[] ViableConnections(AStarNode node)
        {            
            var list = node.Arc.To.ViableConnections(this.Groups, node.Resources);  
            
            // check for loops
            return FilterByLoops(node, list);
        }

        public static Connection[] FilterByLoops(AStarNode node, Connection[] connections)
        {
            var result = new List<Connection>();
            foreach (var conn in connections)
            {
                //if (conn.Action == null)
                //{
                //    result.Add(conn);
                //    continue;
                //}


                var used = 0;
                var parent = node;
                while (parent != null)
                {
                    if (parent.Arc != null && parent.Arc.Id == conn.Id)
                    // if (parent.Arc != null && parent.Arc.Action != null && parent.Arc.Action.Id == conn.Action.Id)
                    {
                        used ++;
                    }
                    parent = parent.Parent;
                }
                if (used <= conn.AllowLoops)
                {
                    result.Add(conn);
                } else {
                    Console.WriteLine("Max Loops Reached: " + conn.Id);
                }
            }
            return result.ToArray();
        }

        public virtual void ApplyPostConditions(AStarNode node)
        {
            if (node.Arc != null)
            {
                node.Arc.ApplyPostconditions(node.Resources, node.WorkflowState, null, true);
            }
        }

        public void ApplyEffect(AStarNode node, AccessCondition effect)
        {
            effect.ApplyPostconditions(node.Resources, null, null, true);
        }

        public IStrategy CreateNested(AStarNode currentNode, Workflow.Instance workflow, Connection conn)
        {
            // the start state is the current state
            var nestedInitialState = currentNode.Resources.Clone();

            // apply backward preconditions
            // currentNode.Arc.ApplyBacktrackPostconditions(this.Groups, nestedInitialState);

            // we need to find a way of how to get from "Start" node to the goal state
            return new ForwardSearch(workflow.Start, nestedInitialState, workflow.Resources, this.Groups);
        }

        public bool ExpandEffects { get { return true; } }
        public IHeuristics CreateHeuristicsForNestedSearch(AStarNode currentNode)
        {
            var finalState = currentNode.Resources.Clone();

            // we apply postcondition on the workflow node
            currentNode.Arc.ApplyExpectedEffects(finalState, null, currentNode.AppliedEffect);

            // we create this as a goal state
            GoalState[] nestedFinish = finalState.ToGoalState();

            return new ResourceBasedHeuristics(nestedFinish);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ei.Ontology;
using Ei.Runtime.Planning.Heuristics;

namespace Ei.Runtime.Planning.Strategies
{
    internal class ForwardSearch : IStrategy
    {
        //private AStarNode initialNode;
        
        protected Group[] Groups { get; set; }

        internal ForwardSearch(WorkflowPosition agentPosition, Governor.GovernorState agentState, Group[] agentGroups)
        {
            this.Groups = agentGroups;

            this.InitialNode = new AStarNode(new Connection(null, agentPosition));
            this.InitialNode.Parent = null;
            this.InitialNode.Resources = agentState;
        }

        // properties

       

        public AStarNode InitialNode { get; }

        // methods

        public virtual ResourceState CloneProperties(ResourceState source)
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
                if (conn.Action == null)
                {
                    result.Add(conn);
                    continue;
                }


                var used = 0;
                var parent = node;
                while (parent != null)
                {
                    if (parent.Arc != null && parent.Arc.Action != null && parent.Arc.Action.Id == conn.Action.Id)
                    {
                        used ++;
                    }
                    parent = parent.Parent;
                }
                if (used <= conn.AllowLoops)
                {
                    result.Add(conn);
                }
            }
            return result.ToArray();
        }

        public virtual void ApplyPostConditions(AStarNode node)
        {
            if (node.Arc != null)
            {
                node.Arc.ApplyPostconditions(node.Resources, null, true);
            }
        }

        public void ApplyEffect(AStarNode node, AccessCondition effect)
        {
            effect.ApplyPostconditions(node.Resources, null, true);
        }

        public IStrategy CreateNested(AStarNode currentNode, Workflow.Instance workflow, Connection conn)
        {
            // the start state is the current state
            var nestedInitialState = currentNode.Resources.Clone();

            // apply backward preconditions
            currentNode.Arc.ApplyBacktrackPostconditions(this.Groups, nestedInitialState);

            // we need to find a way of how to get from "Start" node to the goal state
            return new ForwardSearch(workflow.Start, nestedInitialState, this.Groups);
        }



        public bool ExpandEffects { get { return true; } }
        public IHeuristics CreateHeuristicsForNestedSearch(AStarNode currentNode)
        {
            var finalState = currentNode.Resources.Clone();

            // we apply postcondition on the workflow node
            currentNode.Arc.ApplyExpectedEffects(finalState, currentNode.AppliedEffect);

            // we create this as a goal state
            GoalState[] nestedFinish = finalState.ToGoalState();

            return new ResourceBasedHeuristics(nestedFinish);
        }
    }
}
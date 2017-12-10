//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using Ei.Ontology;
//using Ei.Runtime.Planning.Heuristics;
//using Ei.Runtime;

//namespace Ei.Runtime.Planning.Strategies
//{
//    internal class BackwardSearch : IStrategy
//    {
//        protected Connection StartConnection { get; private set; }
//        public Governor.GovernorVariableState VariableState { get; private set; }
//        protected Group[] Groups { get; private set; }
//        protected WorkflowPosition Position { get; private set; }
//        protected Workflow.Instance Workflow { get; private set; }
//        public bool ExpandEffects { get { return false; } }
        

//        private GoalState[] startState;

//        private BackwardSearch(Workflow.Instance workflow, Governor.GovernorVariableState agentState, Group[] agentGroups, WorkflowPosition position)
//        {
//            this.VariableState = agentState;
//            this.Groups = agentGroups;
//            this.Position = position;
//            this.Workflow = workflow;
//        }

////        internal BackwardSearch(Workflow workflow, IAgentState agentState, Group[] agentGroups, WorkflowPosition position, GoalState[] startState):
////            this(workflow, agentState, agentGroups, position)
////        {
////            // this is used in nested workflow
////            this.startState = startState;
////
////            // browse all connections and 
////            // if one of them positively changes current state to the desired state
////            // we assume that this is the connection we want to consider
////            foreach (var conn in workflow.Connections)
////            {
////                var state = agentState.DeepClone();
////
////                // apply postconditions
////                conn.ApplyPostconditions(agentGroups, state);
////
////                if (startState.All(goal => goal.IsValid(state.GetParameter(goal.Name))))
////                {
////                    this.StartConnection = conn;
////                    break;
////                }
////
////                // connection can contain EFFECTS (which are all postconditions from the contained workflow)
////                if (conn.GeneratedNestedEffects != null)
////                {
////                    // test each effect separately
////
////                    foreach (var effect in conn.GeneratedNestedEffects)
////                    {
////                        state = agentState.DeepClone();
////                        effect.ApplyPostconditions(state);
////
////                        if (startState.All(goal => goal.IsValid(state.GetParameter(goal.Name))))
////                        {
////                            this.StartConnection = conn;
////                            break;
////                        }
////                    }
////                }
////
////                if (this.StartConnection != null)
////                {
////                    break;
////                }
////            }
////
////            if (this.StartConnection == null)
////            {
////                throw new Exception(string.Format("It is not possible to satisfy goal '{0}' in the current workflow", string.Join("; ", startState.Select(w => w.ToString()).ToArray())));
////            }
////            
////        }

//        /// <summary>
//        /// Make sure that initial state is cloned state used to construct heuristic goals!
//        /// </summary>
//        public BackwardSearch(Workflow.Instance workflow, Governor.GovernorVariableState agentState, Group[] agentGroups, WorkflowPosition position, Connection startConnection, GoalState[] startState) :
//            this(workflow, agentState, agentGroups, position)
//        {
//            this.startState = startState;
//            this.StartConnection = startConnection;
//        }

//        // properties

            

//        public AStarNode InitialNode
//        {
//            get
//            {
//                AStarNode currentNode = new AStarNode(this.StartConnection);
//                currentNode.Parent = null;
//                currentNode.VariableState = this.VariableState.Clone();

//                return currentNode;
//            }
//        }

//        // methods

//        public virtual VariableState CloneProperties(VariableState source)
//        {
//            return source.Clone();
//        }

//        public virtual Connection[] ViableConnections(AStarNode node)
//        {
//            // TODO: Nested Workflows
//            var inputs = node.Arc.From.ViableInputs(this.Groups, node.VariableState);
//            var accessibleNodes = inputs.Where(w => this.Workflow.CanAccess(this.Position, w.From)).ToArray();

//            return accessibleNodes;
//        }

//        // in backtracking we use preconditions as postconditions
//        public virtual void ApplyPostConditions(AStarNode node)
//        {
//            if (node.Arc != null)
//            {
//                node.Arc.ApplyBacktrackPostconditions(this.Groups, node.VariableState);
//            }
//        }

//        public IStrategy CreateNested(AStarNode currentNode, Workflow.Instance workflow, Connection conn)
//        {

//            // all other plan elements are trying to satisfy backtracking preconditions on the connection
//            var state = currentNode.VariableState.Clone();

//            // we apply postcondition on the workflow node
//            conn.ApplyExpectedEffects(this.Groups, state, currentNode.AppliedEffect);

//            // we create this as a goal state
//            GoalState[] nestedStart = state.ToGoalState(true);        

//            // the state BEFORE the is backtracked state
//            var nestedInitialState = this.VariableState.Clone();
//            conn.ApplyBacktrackPostconditions(this.Groups, nestedInitialState);

//            // we need to find a way of how to get from "Start" node to the goal state
//            var goals = Governor.FindGoals(workflow.Workflow, nestedInitialState, this.Groups, nestedStart);
//            return new BackwardSearch(workflow, nestedInitialState, this.Groups, workflow.Start, goals[0].Connection, this.startState);


//            //return new BackwardSearch(workflow, nestedInitialState, this.Groups, workflow.Start, nestedStart);
//        }

//        public IHeuristics CreateHeuristicsForNestedSearch(AStarNode currentNode)
//        {
//            return new ResourceBasedHeuristics(this.VariableState.ToGoalState()); ;
//        }

//        public void ApplyEffect(AStarNode node, AccessCondition effect)
//        {
//            effect.ApplyPostconditions(node.VariableState, true);
//        }

        
//    }
//}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Ei.Ontology;
using Ei.Ontology.Actions;
using Ei.Runtime.Planning.Costs;
using Ei.Runtime.Planning.Heuristics;
using Ei.Runtime.Planning.Storage;
using Ei.Runtime.Planning.Strategies;

namespace Ei.Runtime.Planning
{
    /// <summary>
    /// AStar implementation from https://en.wikipedia.org/wiki/A*_seConnectionh_algorithm
    /// </summary>
    public class Planner
    {
        const string LogSource = "Planner";

        //  public static AStarNode empty;
        public event Action<Planner, AStarNode> StepPerformed;
        public event Action<Planner, List<AStarNode>> PlanGenerated;
        public event Action<Planner> PlanFailed;

        public IStorage Storage { get; set; }
        public AStarNode InitialNode { get; private set; }

        private bool reverse;
        private IStrategy strategy;
        private IHeuristics heuristics;
        private ICostManager costManager;

        private int checkedNodes;
        private Governor agent;
        private int visited;

        //public IHeuristics Heuristics { get; set; }

        private readonly AutoResetEvent waitEvent;


        // constructor

        public Planner(Governor agent, AStarNode initialNode = null, StringBuilder logBuilder = null)
        {
            //this.Heuristics = new StaticHeuristics();
            this.Message = logBuilder ?? new StringBuilder();
            this.Storage = new ListStorage();
            this.waitEvent = new AutoResetEvent(false);
            this.agent = agent;
            this.InitialNode = initialNode;
        }

        // properties

        public StringBuilder Message { get; private set; }

        // methods

        private void Log(string message)
        {
            this.Message.AppendLine(message);

            // Ei.Logs.if (Log.IsDebug) Log.Debug(LogSource, message);
        }

        internal List<AStarNode> Plan(IHeuristics heuristics, IStrategy strategy, ICostManager costManager, int maxPlanLegth = 20)
        {
            this.visited = 1;
            this.reverse = !(strategy is BackwardSearch); // we will reverse plan only if it is a forward search
            this.heuristics = heuristics;
            this.strategy = strategy;
            this.costManager = costManager;

            // init start node

            AStarNode currentNode = strategy.InitialNode;

            strategy.ApplyPostConditions(currentNode);
            currentNode.G_Score = 0;
            currentNode.F_Score = currentNode.G_Score + heuristics.Calculate(currentNode);

            // Add initial node to open list
            this.Storage.AddToOpenList(currentNode);

            if (this.StepPerformed != null)
            {
                this.StepPerformed(this, currentNode);
                //this.waitEvent.WaitOne();
            }

            Log("Starting a new plan with " + currentNode);
            Log("State: " + currentNode.VariableState);

            // apend to the initial node

            if (this.InitialNode == null)
            {
                this.InitialNode = currentNode;
            }
            else
            {
                this.InitialNode.Children.Add(currentNode);
            }

            while (this.Storage.HasOpened())
            {
                // the node in openset having the lowest f_score[] value
                currentNode = this.Storage.RemoveCheapestOpenNode();
                currentNode.Status = 1;
                currentNode.Visited = this.visited++;

                // break the execution once planning reaches treshold
                if (currentNode.Length > maxPlanLegth)
                {
                    Log("Reached maximum plan length: " + maxPlanLegth);
                    return null;
                }

                var neighbours = strategy.ViableConnections(currentNode);

                // if there is a postcondition on current Connection apply it
                Log("\n========================================");
                Log("CURRENT NODE: " + currentNode);
                Log("Neighbours: " + neighbours.Length);
                Log("State: " + currentNode.VariableState);
                Log("========================================\n");

                this.checkedNodes ++;

                this.Storage.AddToClosedList(currentNode);

                // if the node that we found is a goal node, reconstruct plan and finish
                if (heuristics.CheckGoal(currentNode))
                {
                    currentNode.Status = 3;

                    // Console.WriteLine ("Finished with plan");
                    var plan = ReconstructPlan(currentNode);
                    // invoke event
                    if (this.PlanGenerated != null) {
                        this.PlanGenerated(this, plan);
                    }
                    return plan;
                }

                foreach (var connection in neighbours)
                {
                    var opened = this.Storage.FindOpened(connection);
                    var closed = this.Storage.FindClosed(connection);

                    var costData = costManager.ComputeCost(this.agent, currentNode, connection);

                    // if the price is too high (probably there are no resources, drop this node)
                    if (float.IsPositiveInfinity(costData.Cost))
                    {
                        var errorNode = new AStarNode(connection);
                        errorNode.Status = 2;
                        currentNode.Children.Add(errorNode);
                        Log("[ERROR] Missing resources for " + connection);
                        continue;
                    }


                    var tentativeGScore = currentNode.G_Score + costData.Cost;

                    // if neighbor in OPEN and cost less than g(neighbor):
                    if (opened != null && tentativeGScore < opened.G_Score)
                    {
                        // remove neighbor from OPEN, because new path is better
                        this.Storage.RemoveOpened(opened);
                    }

                    // if neighbor in CLOSED and cost less than g(neighbor) - this allows us to revisit nodes if need be
                    if (closed != null && tentativeGScore < closed.G_Score)
                    {
                        // remove neighbor from CLOSED
                        this.Storage.RemoveClosed(closed);
                    }

                    // if neighbor not in OPEN and neighbor not in CLOSED:

                    //if (opened == null && closed == null)

//                    // in case that this is join workflow action we move the connection to the start state of the workflow
//                    if (connection.Action is ActionJoinWorkflow)
//                    {
//                        throw new NotImplementedException("Currently we do not support nested workflows in planning");
////                        var wa = connection.Action as ActionJoinWorkflow;
////                        Workflow w;
////                        if (wa.Workflows.Count == 0)
////                        {
////                            var dao = graph.GetWorkflow(wa.WorkflowId);
////                            if (!dao.Static) { throw new NotImplementedException("Currently we support only static workflows"); }
////                            // TODO: create a new workflow
////                            //       create a new arc that leads to the start node
////                            //       push to Context stack as in governor "EnterWorkflow"
////                            // 
////                            // w = graph.CreateWorkflow(wa.WorkflowId, )
////
////                        }
//                    }

                    // TODO: Deal with workflow exit
                    //       Detect that arc leads to the state in which we can exit
                    //       Replicate the Context stack functionality as in governor "ExitWorkflow"

                    AStarNode newNode = this.CreateNode(connection, 
                        currentNode.VariableState.Clone(),
                        currentNode, 
                        tentativeGScore,
                        costData.Data);

                    //Console.WriteLine("----------------------\nBefore: \n" + newNode.State);
                    strategy.ApplyPostConditions(newNode);
                    //Console.WriteLine("After: \n" + newNode.State);

                    newNode.Heuristic = heuristics.Calculate(newNode);
                    newNode.F_Score = newNode.G_Score + newNode.Heuristic;

                    if (float.IsPositiveInfinity(newNode.F_Score)) // there are probably no resources
                    {
                        throw new Exception("Infinity!!");   
                    }

                    Log("\n- ADDED: " + newNode);
                    Log("- State: " + currentNode.VariableState);

                    currentNode.Children.Add(newNode);

                    // we may also wish to expand effects (effects describe what is happening in the workflow)
                    if (strategy.ExpandEffects)
                    {
                        if (connection.ExpectedEffects != null)
                        {
                            foreach (var effect in connection.ExpectedEffects)
                            {
                                AStarNode effectNode = this.CreateNode(connection,
                                    currentNode.VariableState.Clone(),
                                    currentNode,
                                    tentativeGScore,
                                    costData.Data);

                                //Console.WriteLine("----------------------\nBefore: \n" + newNode.State);
                                effectNode.OriginalState = currentNode.VariableState.Clone();
                                
                                strategy.ApplyEffect(effectNode, effect);
                                effectNode.AppliedEffect = effect;

                                //Console.WriteLine("After: \n" + newNode.State);

                                effectNode.Heuristic = heuristics.Calculate(effectNode);
                                effectNode.F_Score = effectNode.G_Score + effectNode.Heuristic;

                                Log("\n- ADDED EFFECT " + effect + ": " + effectNode);
                                Log("- effectNode: " + currentNode.VariableState);

                                currentNode.Children.Add(effectNode);

                                //                                // if the node that we found is a goal node, reconstruct plan and finish
                                //                                if (heuristics.CheckGoal(effectNode))
                                //                                {
                                //                                    // Console.WriteLine ("Finished with plan");
                                //                                    var plan = ReconstructPlan(effectNode);
                                //
                                //                                    // invoke event
                                //                                    this.PlanGenerated?.Invoke(this, plan);
                                //                                    return plan;
                                //                                }
                            }
                        }
                    }

                    // if the node that we found is a goal node, reconstruct plan and finish
                    if (heuristics.CheckGoal(newNode))
                    {
                        newNode.Status = 3;
                        // Console.WriteLine ("Finished with plan");
                        var plan = ReconstructPlan(newNode);

                        // invoke event
                        if (this.PlanGenerated != null) {
                            this.PlanGenerated(this, plan);
                        }
                        return plan;
                    }
                }

                if (this.StepPerformed != null)
                {
                    this.StepPerformed(this, currentNode);
                    this.waitEvent.WaitOne();
                }

                //Console.ReadLine();
            }

            // invoke event on listeners
            if (this.PlanFailed != null) {
                this.PlanFailed(this);
            }
            return null;

        }

        private AStarNode CreateNode(Connection connection,
            Governor.GovernorVariableState state, 
            AStarNode currentNode, 
            float tentativeGScore,
            string costData)
        {
            var newNode = new AStarNode(connection);

            newNode.G_Score = tentativeGScore;
            newNode.Parent = currentNode;
            newNode.VariableState = state;
            newNode.CostData = costData;
            
            this.Storage.AddToOpenList(newNode);

            return newNode;
        }

        //!< Internal function to reconstruct the plan by tracing from last node to initial node.
        internal List<AStarNode> ReconstructPlan(AStarNode goalnode)
        {
            var plan = new List<AStarNode>();

            var currentNode = goalnode;
            while (currentNode != null && currentNode.Arc != null) // same as currentNode != null && currentNode?.Connection != null
            {
                // detect, if plan contains join workflow connections
                // if it does, plan actions contained within the workflow
                // the planning of sub workflow is done is a following way:
                // 1. We select a goal
                //   - If workflow node is the first one in plan, we use a global goal
                //   - Otherwise we use a precondition of the workflow node as a goal
                //   - TODO: Possibly each precondition should be dealth with separately
                var wa = currentNode.Arc.Action as ActionJoinWorkflow;
                if (wa != null)
                {
                    Log("\n\n------------------------\nCONSTRUCTING SUB PLAN FOR: " + wa.WorkflowId + "\n------------------------\n");

                    // switch to original state (unaffected by effects)
                    if (currentNode.OriginalState != null)
                    {
                        currentNode.VariableState = currentNode.OriginalState;
                    }

                    // mark this node as the successfull one
                    currentNode.Status = 3;

                    var newStrategy = this.strategy.CreateNested(currentNode, wa.TestWorkflow, currentNode.Arc);
                    var newHeuristics = newStrategy.CreateHeuristicsForNestedSearch(currentNode);

                    var planner = new Planner(this.agent, currentNode, this.Message);
                    var nestedPlan = planner.Plan(newHeuristics, newStrategy, costManager);

                    if (nestedPlan == null)
                    {
                        var node = currentNode;
                        while (node != null)
                        {
                            node.Status = 4;
                            node = node.Parent;
                        }                       
                        return null;
                    }

                    // first add the exit workflow action
                    plan.Add(new AStarNode(new Connection( null, null, null, new ActionExitWorkflow())));

                    // reverse this plan
                    nestedPlan.Reverse();

                    // insert new plan nodes into original plan
                    nestedPlan.ForEach(w => plan.Add(w));
                }

                // add the original node
                plan.Add(currentNode);

                currentNode = currentNode.Parent;
            }

            if (reverse)
            {
                plan.Reverse();
            }


            // reverse plan so that it goes from start to finish


            Log("\n********************************************");
            Log("FOUND PLAN: " + string.Join(" --> ", plan.Select(w => w.Arc.ToChainString()).ToArray()));
            Log("Checked: " + this.checkedNodes);
            Log("********************************************\n");

            return plan;
        }

        public void Continue()
        {
            this.waitEvent.Set();
        }
    }

    public class PlanException : Exception
    {
        public PlanException(string s) : base(s)
        {
        }
    }
}
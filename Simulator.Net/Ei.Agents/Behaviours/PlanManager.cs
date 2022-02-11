using Ei.Core.Ontology.Actions;
using Ei.Core.Runtime;
using Ei.Core.Runtime.Planning;
using Ei.Core.Runtime.Planning.Costs;
using Ei.Core.Runtime.Planning.Strategies;
using Ei.Logs;
using Ei.Simulation.Behaviours.Actuators;
using Ei.Simulation.Planning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ei.Simulation.Behaviours
{
    public class PlanManager
    {
        public struct PlannerSession
        {
            public SimulationAgent agent;
            public GoalState[] goal;
            public string goalType;
            public TaskCompletionSource<List<AStarNode>> task;
        }
        // we only plan for one agent at the time and this variable controls it
        private static bool planning = false;
        private static Queue<PlannerSession> sessions = new Queue<PlannerSession>();
        private Actuator actuator;

        public event Action<PlanManager, PlannerSession> PlanningStarted;
        public event Action<PlanManager> PlanNotFound;
        public event Action<PlanManager, List<AStarNode>> PlanFound;
        public event Action<PlanManager, PlanException> PlanFailed;

        public event Action<PlanManager, SimulationAgent, List<AStarNode>> PlanExecutionStarted;
        public event Action<PlanManager, SimulationAgent, string> PlanExecutionFailed;
        public event Action<PlanManager, SimulationAgent, List<AStarNode>, int> PlanItemStarted;
        public event Action<PlanManager, SimulationAgent, List<AStarNode>, int> PlanItemFinished;
        public event Action<PlanManager, SimulationAgent, List<AStarNode>> PlanExecutionFinished;

        public PlanManager(Actuator actuator)
        {
            this.actuator = actuator;
        }


        protected virtual void FindPlan(PlannerSession item, string logName)
        {
            var groupString = string.Join(",", item.agent.Governor.Groups.Select(g => g.Organisation.Id + "|" + g.Role.Id).ToArray());
            Log.Debug(logName, "Generating plan for: " + string.Join(";", item.goal.Select(w => w.ToString()).ToArray()));
            this.PlanningStarted?.Invoke(this, item);


            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<AStarNode> plan = null;
            try
            {
                plan = item.agent.Governor.PlanGoalState(item.goal,
                    PlanStrategy.ForwardSearch,
                    this.actuator.CreateCostManager(item));

                if (plan != null)
                {
                    Log.Info(logName, string.Format("Plan with length {0} generated in {1} seconds", plan == null ? 0 : plan.Count, (sw.ElapsedMilliseconds / 1000)));

                    this.PlanFound?.Invoke(this, plan);
                    sw.Stop();
                    item.task.TrySetResult(plan);
                }
                else
                {
                    this.PlanNotFound?.Invoke(this);
                    sw.Stop();
                    item.task.TrySetResult(null);
                }
            }
            catch (PlanException ex)
            {
                this.PlanFailed?.Invoke(this, ex);
                Log.Error(logName, "Plan search failed: " + ex.Message);
                item.task.TrySetResult(null);
            }
            finally
            {
                sw.Stop();
            }
        }


        private void ProcessQueues()
        {
            if (planning)
            {
                return;
            }

            while (sessions.Count > 0)
            {
                var item = sessions.Dequeue ();
                

                string logName = item.agent.Name + " Planner";

                // TODO: Dangerous! Should not happen ...
                if (item.agent.Governor.Workflow == null)
                {
                    return;
                }

                // make sure that agent is in the parent workflow
                while (item.agent.Governor.Workflow.Parent != null)
                {
                    Log.Info(logName, "Exiting workflow due to planning ...");
                    item.agent.Governor.ExitWorkflow();
                }

                this.FindPlan(item, logName);
            }

            planning = false;
        }

        private Task<List<AStarNode>> FindPlan(SimulationAgent agent, GoalState[] goal, string goalType)
        {
            var tsc = new TaskCompletionSource<List<AStarNode>>();
            sessions.Enqueue (new PlannerSession {  agent = agent, goal = goal, goalType = goalType, task = tsc });

            // synchronously process the queue
            Task.Run(ProcessQueues);

            return tsc.Task;
        }

        public async Task<ActionItem> CreateActionPlan(SimulationAgent agent, GoalState[] goal, string goalType)
        {
            var plan = await this.FindPlan(agent, goal, goalType);
            if (plan == null)
            {
                return null;
            }
            return new ActionItem(() => ExecutePlan(agent, plan));
            
        }

        private bool FailPlan(SimulationAgent agent, string logName, string message)
        {
            Log.Error(logName, "Plan Execution Failed: " + message);

            this.PlanExecutionFailed?.Invoke(this, agent, message);

            return false;
        }

        static object locker = new object();
       

        public async Task<bool> ExecutePlan(SimulationAgent agent, List<AStarNode> plan)
        {
            
            var logName = agent.Name + " PlanManager";
            var planFinished = false;

            //lock (locker)
            {
                // remove empty actions

                this.PlanExecutionStarted?.Invoke(this, agent, plan);


                for (var i=0; i<plan.Count; i++)
                {
                    // check if the engine stopped
                    if (!agent.gameObject.GameEngine.IsRunning)
                    {
                        Log.Warning(logName, "Stopping the plan as game engine is not running");
                        break;
                    }
                    var planItem = plan[i];

                    this.PlanItemStarted?.Invoke(this, agent, plan, i);
                    Log.Success(logName, $"Starting Plan Item {i} -  {planItem.Arc.Action}");

                    // skip plan items with no arcs
                    if (planItem.Arc == null) {
                        this.PlanItemFinished?.Invoke(this, agent, plan, i);
                        continue;
                    }

                    // move to new positions with arcs with no actions
                    if (planItem.Arc.Action == null)
                    {
                        if (planItem.Arc.From != null && planItem.Arc.To != null && planItem.Arc.From.Id != planItem.Arc.To.Id)
                        {
                            var actionInfo = agent.Governor.Move(planItem.Arc.To.Id);
                            if (actionInfo.IsNotOk)
                            {
                                return this.FailPlan(agent, logName, "Could not move to the new state");
                            }
                        }
                        this.PlanItemFinished?.Invoke(this, agent, plan, i);
                        continue;
                    }

                    // main body of the plan execution

                    if (planItem.Arc.Action is ActionExitWorkflow)
                    {
                        try
                        {
                            // workflow exit immediatelly continues the plan
                            var result = agent.Governor.ExitWorkflow();
                            if (result.IsNotOk)
                            {
                                return this.FailPlan(agent, logName, "Could not exit workflow");
                            }
                        }
                        catch (InstitutionException ex)
                        {
                            return this.FailPlan(agent, logName, ex.Message);
                        }
                    }
                    else if (planItem.Arc.Action is ActionMessage ||
                        planItem.Arc.Action is ActionJoinWorkflow)
                    {
                        

                        // first perform action in the institution
                        // use the object related to the action

                        try
                        {
                            var result = await this.actuator.ExecutePlanItem(agent, planItem); 
                            if (result == false)
                            {
                                this.FailPlan(agent, logName, "Action Failed");
                                return false;
                            }
                        } catch (Exception ex)
                        {
                            return this.FailPlan(agent, logName, "Action Failed - " + ex.Message);
                        }


                        //if (!string.IsNullOrEmpty(itemId))
                        //{
                        //    Log.Debug(agent.Name, "[WF] Find related ...");

                        //    float waitTime = 0;
                        //    //EnvironmentDataAction action;

                        //    // if it is a simple action that does not generate any interactio simply wait a defined interwal
                        //    if (this.environment.NoLocationInfo(itemId) != null)
                        //    {
                        //        if (this.PerformAction(agent, planItem))
                        //        {
                        //            waitTime = this.environment.NoLocationInfo(itemId).Duration;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        // this is environmental action
                        //        EnvironmentData obj;

                        //        //lock (project.Environment.Objects)
                        //        {
                        //            // object may have eventually disappeared, so we need to replan

                        //            if (!this.environment.TryGetValue(itemId, out obj))
                        //            {
                        //                this.FailPlan(agent, "Missing resource " + itemId);
                        //                return;
                        //            }

                        //            //var action = obj.Definition.Actions.First(w => w.Id == planItem.Arc.Action.Id);

                        //            VariableInstance[] pars = null;
                        //            if (obj.Parameters != null)
                        //            {
                        //                obj.Parameters.TryGetValue(planItem.Arc.Action.Id, out pars);
                        //            }

                        //            // object exists, so execute the object in the institution and use this object
                        //            if (this.PerformAction(agent, planItem, pars))
                        //            {
                        //                Log.Debug(agent.Name, "[WF] Using object and sleeping: " + waitTime);
                        //                waitTime = this.environment.UseObject(obj, planItem.Arc.Action.Id);
                        //            }
                        //        }
                        //    }

                        //    if (waitTime > 0)
                        //    {
                        //        waitTime = (float)this.timer.CalculateDuration(waitTime);

                        //        Log.Debug(agent.Name, "[WF] Continuing plan after: " + waitTime);
                        //        this.RunAfter(() => ContinuePlan(agent), waitTime);
                        //        return;
                        //    }
                        //}
                        //Log.Debug(agent.Name, "[WF] Continuing plan ...");
                        //this.RunAfter(() => ContinuePlan(agent), 0.1f); // continue after tenth of a second

                    }
                    else
                    {
                        throw new NotImplementedException("This is not implemented!");
                    }


                    // plan item finished successfully
                    Log.Debug(logName, $"Finished Plan Item {i} -  {planItem.Arc.Action}");
                    this.PlanItemFinished?.Invoke(this, agent, plan, i);

                    // we may have finished the plan
                    if (i == plan.Count - 1)
                    {
                        planFinished = true;
                    }
  
                }

                if (planFinished)
                {
                    this.PlanExecutionFinished?.Invoke(this, agent, plan);
                    return true;
                } else
                {
                    return this.FailPlan(agent, logName, "No reason given");
                }
                
            }
        }



    }
}

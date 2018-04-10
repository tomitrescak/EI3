using System;
using System.Globalization;
using Ei.Logs;
using Ei.Runtime;
using Ei.Runtime.Planning;
using Ei.Simulation.Simulator;

namespace Ei.Simulation.Physiology
{

    public class PhysiologyBasedAgent : SimulationAgent
    {
        // constructor
        private static PhysiologyProject physiologyProject;

        public PhysiologyBasedAgent(PhysiologyProject project, string role, string[] freeTimeGoals, string name = null) : base(project, role, freeTimeGoals, name) {
            physiologyProject = project;
            this.Callbacks = new PhysiologyAgentCallbacks(this);
        }

        // properties
        
        public static readonly byte[] KillColor = new byte[] { 220, 220, 220 };

        public Governor MainAgent { get; set; }

        public Governor PhysiologyAgent { get; set; }

        private IPhysiologyStore Store {
            get {
                var ei = project.Ei;
                return this.Governor.Resources.FindRole(typeof(IPhysiologyStore)) as IPhysiologyStore;
            }
        }


        // methods

        protected override bool Connected() {

            var hungerModifier = RandomInterval(project.PhysiologyDiversity[0], project.PhysiologyDiversity[1]);
            var thirstModifier = RandomInterval(project.PhysiologyDiversity[0], project.PhysiologyDiversity[1]);
            var fatigueModifier = RandomInterval(project.PhysiologyDiversity[0], project.PhysiologyDiversity[1]);

            var res = this.Governor.PerformAction("initAgent",
                VariableInstance.Create(
                    "Tick", (86400 / project.DayLengthInSeconds).ToString(CultureInfo.InvariantCulture),
                    "HungerModifier", hungerModifier.ToString(CultureInfo.InvariantCulture),
                    "ThirstModifier", thirstModifier.ToString(CultureInfo.InvariantCulture),
                    "FatigueModifier", fatigueModifier.ToString(CultureInfo.InvariantCulture)));

            if (res.IsNotOk) {
                Log.Error(this.Name, res.Code.ToString());
                return false;
            }

            this.MainAgent = this.Governor;

            this.Governor.Continue();
            return true;
        }

        private bool FailedInPlanHistory(int howManyPositionsFromStart, string type) {

            for (var i = 0; i < howManyPositionsFromStart && i < this.PlanHistory.Count; i++) {
                if (this.PlanHistory[i].GoalType == type && this.PlanHistory[i].Failed) {
                    return true;
                }
            }
            return false;
        }

        protected override void Reason() {
            // wait in case we pause
            if (project.Paused) {
                this.View.RunAfter(this.Reason, 1);
                return;
            }

            Log.Debug(this.MainAgent.Name, "Reasoning ...");

            // check if it is time for sleeping
            var time = project.SimulatedTime;
            time = time % 86400;

            //if (time > 64800)
            //{
            //    if (CurrentGoalAction != "Sleep")
            //    {
            //        this.PhysiologyAgent.PerformAction("sleep");
            //        CurrentGoalAction = "Sleep";
            //        this.View.Sleep();
            //    }

            //    // after one second check if agent can wake up
            //    this.View.RunAfter(this.Reason, 1);
            //    return;

            //}
            //else if (CurrentGoalAction == "Sleep")
            //{
            //    // wake agent up
            //    this.CurrentGoalAction = "Idle";
            //    this.PhysiologyAgent.PerformAction("wakeUp");
            //    this.View.WakeUp();
            //}



            // if agent is tired agent will rest
            var humanState = this.Store;

            if (humanState.Fatigue > physiologyProject.FatigueTreshold) {
                CurrentGoalAction = "Rest";

                this.SatifyFatigue();
                return;
            }

            // we either continue executing a current plan
            // or we generate a new goal

            if (this.State == AgentState.ExecutingPlan) {
                this.ExecuteNextPlanStep(this.MainAgent);
                return;
            }

            // if agent is thirsty agent will drink
            var thirst = humanState.Thirst;
            if (thirst > physiologyProject.KillThirstThreshold) {
                this.KillAgent("Thirst");
                return;
            }

            if (thirst > physiologyProject.ThirstThreshold) {
                CurrentGoalAction = "Thirst";

                // check if agent did try to satisfy the thirst and failed
                // thus we only try to satisfy thirst only after non failed attempt
                if (!this.FailedInPlanHistory(1, "Thirst")) {
                    this.SatifyThirst();
                    return;
                }
                this.CurrentGoalAction = "Recovery";

            }

            // we generate goal for hunger

            var hunger = humanState.Hunger;
            if (hunger > physiologyProject.KillHungerThreshold) {
                this.KillAgent("Hunger");
                return;
            }

            if (hunger > physiologyProject.HungerTreshold) {
                CurrentGoalAction = "Hunger";

                // check if agent did try to satisfy the hunger and failed
                // thus we only try to satisfy hunger only after non failed attempt
                if (!this.FailedInPlanHistory(1, "Hunger")) {
                    this.SatifyHunger();
                    return;
                }
                this.CurrentGoalAction = "Recovery";
            }

            // free time activity

            if (this.FreeTimeGoals != null && this.FreeTimeGoals.Length > 0) {
                if (this.CurrentGoalAction != "Recovery") {
                    CurrentGoalAction = "Free";
                }

                // we will either execute one of the planned actions or we execute free time
                // that is random walk

                var num = random.Next(0, this.FreeTimeGoals.Length + 1);

                if (num < this.FreeTimeGoals.Length) {
                    var goals = this.FreeTimeGoals[num];

                    // update goals to the current state (goals can be in form 'hunger = hunger + 1')
                    foreach (var goal in goals) {
                        goal.Update(this.MainAgent.Resources);
                    }

                    // randomly select on goal and execute it
                    this.CreatePlan(this.MainAgent, goals);
                    return;
                }
            }

            this.FreeTime();

        }

        public bool Dead { get; private set; }

        private void KillAgent(string type) {
            if (this.Dead) return;

            this.Dead = true;

            this.Name += " [DEAD] " + type;
            this.Color = KillColor;

            this.PhysiologyAgent.Move("exit");
            this.PhysiologyAgent.Move("join");
            this.MainAgent.Move("join");
        }

        protected virtual void SatifyFatigue() {
            Log.Debug(this.MainAgent.Name, "Fatigued ...");

            // calculate how long agent needs to rest

            var fatigue = this.Store.Fatigue;

            var finalFatigue = Math.Round(physiologyProject.FatigueTreshold * random.NextDouble());
            var difference = fatigue - finalFatigue; // agent can decide not to recuperate fully

            // notify institution about resting

            this.PhysiologyAgent.PerformAction("rest", VariableInstance.Create("Fatigue", finalFatigue.ToString("F1")));

            // agent recuperates 1 fatigue points per 20 minutes (1200 sec)
            difference = difference * 1200;

            var restingTime = difference / project.SimulatedSecond;

            // display on view
            this.View.Rest(restingTime);
        }

        protected virtual void SatifyThirst() {
            var thirst = this.Store.ThirstDecay;

            Log.Warning(this.MainAgent.Name, "Thirsty: " + thirst);

            // calculate how much we need to drink
            // var finalthirst = Math.Round(project.ThirstThreshold * random.NextDouble(), 2);

            var finalThirst = Math.Round((thirst - 0.1) * 100) / 100f;
            if (finalThirst < 0) finalThirst = 0.01;

            this.CreatePlan(this.MainAgent, new[]
            {
                new GoalState("ThirstDecay", finalThirst, StateGoalStrategy.Max)
            },
            "Thirst");
        }

        protected virtual void SatifyHunger() {
            var hunger = this.Store.HungerDecay;

            Log.Warning(this.MainAgent.Name, "Hungry: " + hunger);

            // calculate how much we need to drink
            // var finalthirst = Math.Round(project.ThirstThreshold * random.NextDouble(), 2);

            var finalHunger = Math.Round((hunger - 0.1) * 100) / 100f;
            this.CreatePlan(this.MainAgent, new[]
            {
                new GoalState("HungerDecay", finalHunger, StateGoalStrategy.Max)
            },
            "Hunger");
        }


        /// <summary>
        /// This action is executed when agent has a free time (is completely satisfied)
        /// </summary>

        protected virtual void FreeTime() {
            this.RandomWalk();
        }

        public void Rested() {
            this.PhysiologyAgent.PerformAction("rested");

            this.Reason();
        }

        protected override void MoveToDestination(float x, float y) {
            base.MoveToDestination(x, y);

            // turn off fatigue
            // this.PhysiologyAgent.PerformAction("moving");
        }

        public override void MovedToLocation() {
            base.MovedToLocation();

            // turn off fatigue but only if we are not going to sleep

            if (CurrentGoalAction != "Sleep") {
                //    this.PhysiologyAgent.PerformAction("stopped");
            }
        }
    }
}

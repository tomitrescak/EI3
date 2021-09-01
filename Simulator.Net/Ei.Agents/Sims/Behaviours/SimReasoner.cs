﻿using System.Collections.Generic;
using System.Linq;
using Ei.Simulation.Core;
using UnityEngine;

namespace Ei.Simulation.Sims.Behaviours
{
    public class SimReasoner: EiBehaviour, IUpdates
    {
        private interface ISelectionStartegy
        {
            SimAction FindGoal(Sim sim);
            PlanItem FindObject(Sim sim, string action);
        }

        private class Best : ISelectionStartegy {
            private SimAction[] uniqueActions;
            private SimAction[] UniqueActions {
                get {
                    if (uniqueActions == null) {
                        var list = new List<SimAction>();
                        var objects = GameObject.FindObjectsOfType<SimObject>();
                        foreach (var obj in objects) {
                            for (var i = 0; i < obj.Actions.Length; i++) {
                                if (list.All(w => w.Name != obj.Actions[i].Name)) {
                                    list.Add(obj.Actions[i]);
                                }
                            }
                        }
                        uniqueActions = list.ToArray();
                    }
                    return uniqueActions;
                }
            }

            public SimAction FindGoal(Sim sim) {
                var simObjects = GameObject.FindObjectsOfType<SimObject>();
                var happiness = 0f;
                SimAction bestAction = null;
                foreach (var a in this.UniqueActions) {
                    var obj = simObjects.FirstOrDefault(o => o.Actions.Any(b => b.Name == a.Name));
                    // if we have no such object with this action we refuse it
                    if (obj == null) {
                        continue;
                    }
                    // if we have no such object with this actions plan we refuse it
                    if (a.Plan != null && a.Plan.Length > 1) {
                        foreach (var planAction in a.Plan) {
                            if (simObjects.All(o => o.Actions.All(b => b.Name != planAction))) {
                                continue;
                            }
                        }
                    }
                    var h = sim.CheckDelta(a);
                    // Debug.WriteLine(a.Name + ": " + h);
                    if (happiness < h) {
                        happiness = h;
                        bestAction = a;
                    }
                };
                // Debug.WriteLine("====================");
                return bestAction;
            }

            public virtual PlanItem FindObject(Sim sim, string action) {
                var obj = GameObject.FindObjectsOfType<SimObject>().FirstOrDefault(o => o.Actions.Any(a => a.Name == action && a.Depleted == false));
                if (obj == null) {
                    return null;
                }
                return new PlanItem(obj, obj.Actions.First(a => a.Name == action));
            }
        }

        private class BestClosest : Best
        {
            public override PlanItem FindObject(Sim sim, string actionName) {
                var simObjects = GameObject.FindObjectsOfType<SimObject>();
                SimObject simObject = null;
                if (simObjects != null) {
                    var withSameAction = simObjects.Where(o => o.Actions.Any(a => a.Name == actionName && a.Depleted == false));
                    var distance = float.MaxValue;
                    foreach (var obj in withSameAction) {
                        var currentDistance = Vector3.Distance(obj.transform.position, sim.transform.position);
                        if (currentDistance < distance) {
                            simObject = obj;
                            distance = (float)currentDistance;
                        }
                    }
                    
                    if (simObject != null) {
                        return new PlanItem(simObject, simObject.Actions.First(a => a.Name == actionName));
                    }
                }
                return null;
            }

        }

        private class PlanItem
        {
            public SimObject simObject;
            public SimAction simAction;

            public PlanItem(SimObject obj, SimAction action) {
                this.simObject = obj;
                this.simAction = action;
            }

            public bool ActionDepleted() {
                if (this.simAction.Depleted) {
                    // object has depleted we replan the new destination in the next frame
                    return true;
                } else if (this.simAction.Uses >= 1) {
                    // decrese use count and check for depletion
                    this.simAction.Uses--;

                    if (this.simAction.Uses == 0) {
                        this.simAction.Depleted = true;
                        this.CheckDestroy();
                    }
                }
                return false;
            }

            private void CheckDestroy() {
                if (this.simObject.Actions.All(a => a.Depleted)) {
                    GameObject.Destroy(this.simObject.gameObject);
                }
            }
        }

        private ISelectionStartegy strategy = new BestClosest();
        private bool hasPlan;
        private bool executingPlan;
        private int successfullPlans;
        private int failedPlans;
        private SimAction finalPlanItem;
        private float continueAt = 0;
        private bool executingItem = false;

        private List<string> plan;
        
        private LinearNavigation navigation;
        private Sim sim;

        // current action in queue
        private PlanItem currentPlanItem;

        #region Properties

        public bool HasPlan {
            get { return hasPlan; }
            set {
                this.hasPlan = value;
                this.OnPropertyChanged("HasPlan");
            }
        }

        public bool ExecutingPlan {
            get { return executingPlan; }

            set {
                this.executingPlan = value;
                this.OnPropertyChanged("ExecutingPlan");
            }
        }

        public string Plan { get { return string.Join(", ", this.plan.ToArray()); } }

       
        #endregion

        public void Init() {
            this.navigation = this.GetComponent<LinearNavigation>();
            this.sim = this.GetComponent<Sim>();
        }

        public void Update() {
            if (this.sim.IsDead) {
                return;
            }

            // check if sim is dead
            if (this.sim.Hunger < -99) {
                this.sim.IsDead = true;
            }

            // if we are currently executing action we do nothing
            if (continueAt > Time.time) {
                return;
            }

            // if we are moving to a destination we do nothing else
            if (this.navigation.Navigating) {
                return;
            }

            // if we do not have a plan we reason and find the best object that gives us best value
            if (!this.HasPlan) {
                // find the best simObject
                this.finalPlanItem = this.strategy.FindGoal(sim);

                // NO MORE PLANS!
                if (finalPlanItem == null) {
                    return;
                }

                // with the best sim object 
                this.plan = this.finalPlanItem.Plan.ToList();
                this.plan.Add("end");

                this.OnPropertyChanged("Plan");

                this.HasPlan = true;
            }

            // if we have no curently planned action we plan it and start navigation
            if (this.currentPlanItem == null) {
                this.ContinuePlan();
                return;
            }

            // finish current item
            // if we could not finish it we retry in the next frame
            if (!this.FinishPlanItem()) {
                return;
            }

            // check if we have completed our plan
            this.ContinuePlan();
        }

        private bool FinishPlanItem() {
            // when we reach the destination we execute the action with a given object and then continue
            if (this.executingItem == false) {
                if (this.currentPlanItem.ActionDepleted()) {
                    if (this.currentPlanItem.simAction.Name == this.finalPlanItem.Name) {
                        this.PlanFailed();
                    }
                    // object has depleted we replan the new destination in the next frame
                    this.currentPlanItem = null;
                    return false;
                }
                this.executingItem = true;
                this.continueAt = Time.time + this.currentPlanItem.simAction.DurationInMinutes * BehaviourConfiguration.SimulatedMinutesToReal;
                //Debug.WriteLine(string.Format("Waiting {0} minutes from {1} till {2}",
                //    this.currentPlanItem.simAction.DurationInMinutes,
                //    TimeSpan.FromSeconds(Time.time * BehaviourConfiguration.RealSecondsToSimulated),
                //    TimeSpan.FromSeconds(this.continueAt * BehaviourConfiguration.RealSecondsToSimulated)));
                return false;
            }

            // set that we are done with execution
            this.executingItem = false;

            // apply modifiers of current action
            this.sim.UpdateModifiers(this.currentPlanItem.simAction);

            // remove last plan element
            this.plan.RemoveAt(0);
            this.OnPropertyChanged("Plan");

            // we set that we have no planned action
            this.currentPlanItem = null;
            

            return true;
        }

        private void ContinuePlan() {
            if (this.plan.Count == 0) {
                // UnityEngine.Debug.Log(TimeSpan.FromSeconds(Time.time * BehaviourConfiguration.RealSecondsToSimulated).ToString() + ": " + this.finalPlanItem.Name);
                this.sim.UpdateModifiers(this.finalPlanItem);
                this.PlanSucceeded();
                
            } else {
                // find closest object that provides this action
                this.currentPlanItem = this.CreatePlan();
                if (this.currentPlanItem == null) {
                    this.PlanFailed();
                    return;
                } else {
                    this.navigation.MoveToDestination(this.currentPlanItem.simObject.transform.position);
                }
            }
        }

        private PlanItem CreatePlan() {
            var actionName = this.plan[0];

            // if we are at final we retunr our final item
            if (actionName == "end") {
                return this.strategy.FindObject(this.sim, finalPlanItem.Name);
            }

            // find all objects wiht this name and that is not depleted
            return this.strategy.FindObject(this.sim, actionName);
        }

        private void PlanFailed() {
            this.failedPlans++;
            this.currentPlanItem = null;
            this.finalPlanItem = null;
            this.HasPlan = false;
        }

        private void PlanSucceeded() {
            this.successfullPlans++;
            this.currentPlanItem = null;
            this.finalPlanItem = null;
            this.HasPlan = false;
        }
    }
}

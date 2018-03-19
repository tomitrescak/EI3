using Ei.Agents.Core;
using Ei.Agents.Core.Behaviours;
using Ei.Simulator.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ei.Agents.Planning
{
    public class EiAgent : EiBehaviour, IAgentView, IUpdates
    {
        private struct ActionItem
        {
            public float startTime;
            public Action action;

            public ActionItem(float startTime, Action action) {
                this.startTime = startTime;
                this.action = action;
            }
        }

        private List<AgentProperty> properties;
        private LinearNavigation navigation;
        private ColorProvider colorProvider;
        private List<ActionItem> actionQueue;
        private Queue<Action> immediateActions;

        private bool navigating;

        public string AgentProperties { get { return string.Join("\n ", this.properties.Select(p => p.Label + ": " + p.Value).ToArray()); } }

        public PhysiologyBasedAgent agent;

        // ctor

        public EiAgent() {
            
        }

        // simulation methods

        public void Start() {
            this.properties = new List<AgentProperty>();
            this.actionQueue = new List<ActionItem>();
            this.immediateActions = new Queue<Action>();
            this.navigation = GetComponent<LinearNavigation>();
            this.colorProvider = GetComponent<ColorProvider>();
        }

        public void Update() {
            this.OnPropertyChanged("AgentProperties");

            if (this.navigation.Navigating) {
                return;
            }

            // check if we just ended navigation
            if (this.navigating) {
                // tell agent we just moved to our destination
                this.navigating = false;
                this.agent.MovedToLocation();
            }
            // execute immediate actions
            //if (this.immediateActions.Count > 0) {
            //    while (this.immediateActions.Count > 0) {
            //        this.immediateActions.Dequeue()();
            //    }
            //}
            // execute all actions in queue that can be executed at this time
            for (var i=actionQueue.Count - 1; i>= 0; i--) {
                var item = actionQueue[i];
                if (item.startTime <= Time.time) {
                    item.action();
                    actionQueue.RemoveAt(i);
                }
                
            }
            
        }


        // ei methods

        public void AddProperty(AgentProperty agentProperty) {
            // add in model TODO: remove this need
            this.agent.Properties.Add(agentProperty);
            // add in view
            this.properties.Add(agentProperty);
            this.OnPropertyChanged("AgentProperties");
        }

        public void MoveToLocation(double x, double y, double speedModifier) {
            this.navigation.SpeedPxPerSecond = (float) speedModifier;
            this.navigation.MoveToDestination((float) x, (float) y);
            this.navigating = true;
        }

        public void RunAfter(Action action, float waitTimeInSeconds) {
            this.actionQueue.Add(new ActionItem(Time.time + waitTimeInSeconds, action));
        }

        public void UpdateOnUi(Action action) {
            action();
        }

        public void WakeUp() {
            this.colorProvider.RestoreOriginal();
        }

        public void Rest(double restingTimeInSeconds) {
            this.colorProvider.MakeGreen();
            this.actionQueue.Add(new ActionItem((float)(Time.time + restingTimeInSeconds), () => {
                this.colorProvider.RestoreOriginal();
                agent.Rested();
            }));
        }

        public void Sleep() {
            this.colorProvider.MakeBlack();
        }


    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Ei.Ontology;
using Ei.Runtime;
using Ei.Runtime.Planning;
using Ei.Runtime.Planning.Heuristics;
using Ei.Runtime.Planning.Strategies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TechTalk.SpecFlow;

namespace Ei.Tests.Steps
{
    [Binding]
    public sealed class PlanningSteps
    {
        Dictionary<string, Mock<Governor>> Governors => this.scenarioContext.Get<Dictionary<string, Mock<Governor>>>();

        private List<AStarNode> plan;

        private readonly ScenarioContext scenarioContext;

        public PlanningSteps(ScenarioContext scenarioContext) {
            if (scenarioContext == null) throw new ArgumentNullException("scenarioContext");
            this.scenarioContext = scenarioContext;
        }

        [When(@"Agent '(.*)' requests plan to perform '(.*)' with strategy '(.*)'")]
        public void WhenAgentRequestsPlanToPerform(string agentName, string actionName, string strategyName) {
            var sw = new Stopwatch();
            sw.Start();
            var agent = Governors[agentName];
            plan = agent.Object.PlanAction(actionName, this.ParseStrategy(strategyName));
            Console.Write("ELAPSED: " + sw.ElapsedMilliseconds / 1000f);
            sw.Stop();
        }

        [Then(@"Plan length is '(.*)'")]
        public void ThenPlanLengthIs(int planLength) {
            var planString  = string.Join("\n", plan.Select(p => p.ToString()));
            Assert.AreEqual(plan.Count, planLength);
        }

        [When(@"Agent '(.*)' plans to change state to '(.*)' with strategy '(.*)'")]
        public void WhenAgentPlansToChangeStateToWithStrategy(string agentName, string stateDescription, string strategyName) {
            var sw = new Stopwatch();
            sw.Start();

            var agent = Governors[agentName];
            var goalState = GoalState.ParseStringGoals(agent.Object, stateDescription);
            // agent.Object.Properties[]
            plan = agent.Object.PlanGoalState(goalState, this.ParseStrategy(strategyName));
            Console.Write("ELAPSED: " + sw.ElapsedMilliseconds / 1000f);
            sw.Stop();
        }

        private PlanStrategy ParseStrategy(string name) {
            return (PlanStrategy)Enum.Parse(typeof(PlanStrategy), name);
        }

        [When(@"Agent '(.*)' backward plans action '(.*)' with strategy '(.*)'")]
        public void WhenAgentBackwardPlansActionWithStrategy(string agentName, string actionName, string strategyName) {
            var agent = Governors[agentName];
            plan = agent.Object.PlanAction(actionName, this.ParseStrategy(strategyName));
        }


        [When(@"Agent '(.*)' backward plans to change state to '(.*)' with strategy '(.*)'")]
        public void WhenAgentBackwardPlansToChangeStateToWithStrategy(string agentName, string stateDescription, string strategyName) {
            var agent = Governors[agentName];
            var goalState = GoalState.ParseStringGoals(agent.Object, stateDescription);

            // agent.Object.Properties[]
            plan = agent.Object.PlanGoalState(goalState, this.ParseStrategy(strategyName));
        }

        [When(@"Agents '(.*)' clone '(.*)' backward plans to change state to '(.*)' with strategy '(.*)'")]
        public void WhenAgentsCloneBackwardPlansToChangeStateToWithStrategy(string agentName, string cloneName, string stateDescription, string strategyName) {
            var agent = Governors[agentName];
            var clone = agent.Object.Clones.First(w => w.Name == cloneName);

            var goalState = GoalState.ParseStringGoals(agent.Object, stateDescription); // backwards, start is the desired state

            // agent.Object.Properties[]
            plan = clone.PlanGoalState(goalState, this.ParseStrategy(strategyName));
        }

        [Then(@"Agent '(.*)' has '(.*)' possibilitites to satisfy '(.*)'")]
        public void ThenAgentHasPossibilititesToSatisfy(string agentName, int possibilities, string stateDescription) {
            var agent = Governors[agentName].Object;
            var goalState = GoalState.ParseStringGoals(agent, stateDescription);

            var goals = Governor.FindGoals(agent.Workflow, agent.Resources, agent.Groups, goalState);
            Assert.AreEqual(possibilities, goals.Count);
        }
    }
}

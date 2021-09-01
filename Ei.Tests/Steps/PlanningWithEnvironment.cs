using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ei.Core.Ontology;
using Ei.Core.Runtime;
using Ei.Core.Runtime.Planning;
using Ei.Core.Runtime.Planning.Costs;
using Ei.Core.Runtime.Planning.Environment;
using Ei.Core.Runtime.Planning.Strategies;
using Moq;
using Xunit;

namespace Ei.Tests.Steps
{
    // [Binding]
    public sealed class PlanningWithEnvironment
    {
        Dictionary<string, Mock<Governor>> Governors => this.scenarioContext.Get<Dictionary<string, Mock<Governor>>>();

        private static Dictionary<string, AgentEnvironment> environments;

        private List<AStarNode> plan;

        private readonly ScenarioContext scenarioContext;

        public PlanningWithEnvironment(ScenarioContext scenarioContext) {
            if (scenarioContext == null) throw new ArgumentNullException("scenarioContext");
            this.scenarioContext = scenarioContext;
        }

        static PlanningWithEnvironment() {
            environments = new Dictionary<string, AgentEnvironment>();

            var basicData = new List<EnvironmentData>();
            basicData.Add(new EnvironmentData("bark", "findBark", 0, 10));
            basicData.Add(new EnvironmentData("wood", "findWood", 10, 20));
            basicData.Add(new EnvironmentData("rock", "findRock", 15, 20));
            basicData.Add(new EnvironmentData("flint", "findFlint", 25, 10));
            basicData.Add(new EnvironmentData("bone", "findBone", 25, 10));
            basicData.Add(new EnvironmentData("fire", "makeFire", 10, 0));
            basicData.Add(new EnvironmentData("knife", "makeKnife", 16, 50));
            basicData.Add(new EnvironmentData("spear", "makeSpear", 16, 50));
            basicData.Add(new EnvironmentData("table", "eatCookedFood", 16, 22));

            var fishData = new List<EnvironmentData>(basicData);
            fishData.Add(new EnvironmentData("river", "catchFish", 10, 15));
            fishData.Add(new EnvironmentData("fishPot", "cookFish", 20, 5));
            fishData.Add(new EnvironmentData("forrest", "huntKangaroo", 200, 180));
            fishData.Add(new EnvironmentData("kangaPot", "cookKangaroo", 200, 190));
            fishData.Add(new EnvironmentData("fruit", "eatFruit", 200, 165));
            var fishEnvironment = new AgentEnvironment(fishData.ToArray(), 200, 200, 10);

            fishEnvironment.AddNoLocationAction("makeSpearWorkflow");
            fishEnvironment.AddNoLocationAction("makeKnifeWorkflow");
            fishEnvironment.AddNoLocationAction("makeFireWorkflow");
            fishEnvironment.AddNoLocationAction("eatFishWorkflow");
            fishEnvironment.AddNoLocationAction("eatKangarooWorkflow");


            var kangaData = new List<EnvironmentData>(basicData);
            kangaData.Add(new EnvironmentData("river", "catchFish", 250, 150));
            kangaData.Add(new EnvironmentData("fishPot", "cookFish", 200, 180));
            kangaData.Add(new EnvironmentData("forrest", "huntKangaroo", 20, 22));
            kangaData.Add(new EnvironmentData("kangaPot", "cookKangaroo", 15, 10));
            kangaData.Add(new EnvironmentData("fruit", "eatFruit", 200, 170));
            var kangaEnvironment = new AgentEnvironment(kangaData.ToArray(), 200, 200, 10);

            kangaEnvironment.AddNoLocationAction("makeSpearWorkflow");
            kangaEnvironment.AddNoLocationAction("makeKnifeWorkflow");
            kangaEnvironment.AddNoLocationAction("makeFireWorkflow");
            kangaEnvironment.AddNoLocationAction("eatFishWorkflow");
            kangaEnvironment.AddNoLocationAction("eatKangarooWorkflow");

            environments.Add("PreferFish", fishEnvironment);
            environments.Add("PreferKangaroo", kangaEnvironment);


        }


        // [When(@"Agent '(.*)' plans state '(.*)' with environment '(.*)'")]
        public void WhenAgentPlansStateWithEnvironment(string agentName, string stateDescription, string environment) {
            var agent = Governors[agentName];
            var goalState = GoalState.ParseStringGoals(agent.Object, stateDescription);

            plan = agent.Object.PlanGoalState(goalState, PlanStrategy.ForwardSearch, new TravelCostManager(agent.Object, environments[environment], 0, 0));

        }

        // [Then(@"Then the plan contains '(.*)'")]
        public void ThenThenThePlanContains(string actionId) {
            Assert.True(plan.Exists(w => w.Arc.Action != null && w.Arc.Action.Id == actionId));
        }

    }
}

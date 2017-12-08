using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ei.Logs;
using Ei.Ontology;
using Ei.Ontology.Actions;
using Ei.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TechTalk.SpecFlow;

namespace Ei.Tests.Steps
{
    [Binding]
    public class ActivitySteps
    {
        Dictionary<string, Mock<Governor>> Governors => this.scenarioContext.Get<Dictionary<string, Mock<Governor>>>();
        Dictionary<string, Mock<IGovernorCallbacks>> Callbacks => this.scenarioContext.Get<Dictionary<string, Mock<IGovernorCallbacks>>>();
        Mock<ILog> Logger => this.scenarioContext.Get<Mock<ILog>>();

        private readonly ScenarioContext scenarioContext;

        public ActivitySteps(ScenarioContext scenarioContext) {
            if (scenarioContext == null) throw new ArgumentNullException("scenarioContext");
            this.scenarioContext = scenarioContext;
        }


        [Then(@"Agent '(.*)' plays role '(.*)' and belong to the started institution")]
        public void ThenAgentPlaysRoleInInstitution(string agentName, string roles)
        {
            var ei = this.scenarioContext.Get<Mock<InstitutionManager>>().Object.Ei;
            var agent = Governors[agentName];

            // check name
            Assert.AreEqual(agentName, agent.Object.Name);

            // check roles
            var orgRoles = roles.Split(';').Select(roleSplit => roleSplit.Split(',')).ToArray();
            for (var i = 0; i < orgRoles.Length; i++)
            {
                if (orgRoles[i].Length == 2)
                {
                    Assert.AreEqual(orgRoles[i][0], agent.Object.Groups[0].Organisation.Name);
                    Assert.AreEqual(orgRoles[i][1], agent.Object.Groups[1].Role.Name);
                }
                else
                {
                    Assert.AreEqual(orgRoles[i][0], agent.Object.Groups[0].Role.Name);
                }

            }
        }

        [When(@"Agent '(.*)' joins '(.*)'")]
        [When(@"Agent '(.*)' performs '(.*)'")]
        public void ThenAgentPerformsActivity(string agentName, string activityName)
        {
            var agent = Governors[agentName];
            var result = agent.Object.PerformAction(activityName);

            Assert.AreEqual(InstitutionCodes.Ok, result.Code);

            // continue execution
            // agent.Object.Continue();
        }

        [When(@"Agent '(.*)' joins workflow '(.*)' with '(.*)'")]
        [When(@"Agent '(.*)' performs action '(.*)' with '(.*)'")]
        public void ThenAgentPerformsActivityWith(string agentName, string activityName, string activityParameters)
        { 
            this.WhenCloneOfPerformsActivityWith(null, agentName, activityName, activityParameters);
        }

        [Then(@"Agent '(.*)' cannot join workflow '(.*)' with '(.*)'")]
        public void ThenAgentCannotJoinWorkflowWith(string agentName, string activityName, string activityParameters)
        {
            this.ThenCloneOfAgentCannotJoinWorkflowWith(null, agentName, activityName, activityParameters);
        }

        [Then(@"Clone '(.*)' of '(.*)' cannot join workflow '(.*)' with '(.*)'")]
        public void ThenCloneOfAgentCannotJoinWorkflowWith(string cloneName, string agentName, string activityName, string activityParameters)
        {
            var ei = this.scenarioContext.Get<Mock<InstitutionManager>>().Object.Ei;
            var agent = Governors[agentName];

            // build parameters
            var parsplt = string.IsNullOrEmpty(activityParameters) ? new string[0] : activityParameters.Split(';');
            var parameters = parsplt.Select(pair => new VariableInstance(pair.Split('=')[0], pair.Split('=')[1])).ToArray();

            var result = string.IsNullOrEmpty(cloneName) ?
                agent.Object.PerformAction(activityName, parameters) :
                agent.Object.PerformAction(cloneName, activityName, parameters);

            Assert.AreEqual(result.Code, InstitutionCodes.Failed);
        }


        [When(@"Clone '(.*)' of '(.*)' performs action '(.*)' with '(.*)'")]
        public void WhenCloneOfPerformsActivityWith(string cloneName, string agentName, string activityName, string activityParameters)
        {
            var agent = Governors[agentName]; 

            // build parameters
            var parsplt = string.IsNullOrEmpty(activityParameters) ? new string[0] : activityParameters.Split(';');
            var parameters = parsplt.Select(pair => new VariableInstance(pair.Split('=')[0], pair.Split('=')[1])).ToArray();

            var result = string.IsNullOrEmpty(cloneName) ?
                agent.Object.PerformAction(activityName, parameters) :
                agent.Object.PerformAction(cloneName, activityName, parameters);
             
            Assert.IsTrue(result.IsAcceptable);  
            //Assert.AreEqual(InstitutionCodes.Ok, result.Code);

            // continue execution
            if (result.IsOk)
            {
                agent.Object.Continue(cloneName); 
            }
        }

        [Then(@"Agent '(.*)' fails action '(.*)' with '(.*)'")]
        public void ThenAgentFailsActivityWith(string agentName, string activityName, string activityParameters)
        {
            var agent = Governors[agentName];

            // build parameters
            var parsplt = activityParameters.Split(';');
            var parameters = parsplt.Select(pair => new VariableInstance(pair.Split('=')[0], pair.Split('=')[1])).ToArray();

            var result = agent.Object.PerformAction(activityName, parameters);

            Assert.AreNotEqual(InstitutionCodes.Ok, result.Code);

            // continue execution
            agent.Object.Continue();
        }
         

        [Then(@"Agent '(.*)' parameter '(.*)' is equal to '(.*)'")]
        public void ThenWorkflowHasParameterWithParameterEqualTo(string agentName, string paramName, string propertyValue)
        {
            var param = GetParameter(agentName, paramName);
            Assert.AreEqual(propertyValue, param);
        }

        [Then(@"Agent '(.*)' int parameter '(.*)' is equal to '(.*)'")]
        public void ThenWorkflowHasIntParameterWithParameterEqualTo(string agentName, string paramName, string propertyValue)
        {
            var param = GetParameter(agentName, paramName);
            Assert.AreEqual(int.Parse(propertyValue), param);
        }

        [Then(@"Agent '(.*)' float parameter '(.*)' is equal to '(.*)'")]
        public void ThenWorkflowHasFloatParameterWithParameterEqualTo(string agentName, string paramName, string propertyValue)
        {
            var param = GetParameter(agentName, paramName);
            Assert.AreEqual(float.Parse(propertyValue), param);
        }

        [Then(@"Agent '(.*)' bool parameter '(.*)' is equal to '(.*)'")]
        public void ThenWorkflowHasBoolParameterWithParameterEqualTo(string agentName, string paramName, string propertyValue)
        {
            var param = GetParameter(agentName, paramName);
            Assert.AreEqual(bool.Parse(propertyValue), param);
        }

        private object GetParameter(string agentName, string paramName)
        {
            var agent = Governors[agentName];
            var provider = agent.Object.VariableState.FindProvider(paramName);
            Assert.IsNotNull(provider);

            var obj = provider.FindByName(paramName);
            
            Assert.IsNotNull(obj);
            return obj.Value(provider);
        }

        [When(@"Agent '(.*)' automatically continues")]
        public void WhenAgentAutomaticallyContinues(string agentName)
        {
            var agent = Governors[agentName];
            agent.Object.Continue();
        }


        [Then(@"Logs '(.*)' with '(.*)'")]
        public void ThenAgentLogsActionWithParameters(string actionName, string message)
        {
            Logger.Verify(
                w => w.Log(It.Is<ILogMessage>(i => i.Code == actionName && string.Join(";", i.Parameters) == message)), string.Format("Failed for {0} and parameters '{1}'", actionName, message));
        }

        [Then(@"Agent '(.*)' cannot move to '(.*)'")]
        public void AgentCannotMoveTo(string agentName, string positionId) {
            var agent = Governors[agentName];
            var result = agent.Object.Move(positionId);

            Assert.AreEqual(result.Code, ActionInfo.StateNotReachable.Code);
        }

        [When(@"Agent '(.*)' moves to '(.*)'")]
        [Then(@"Agent '(.*)' moves to '(.*)'")]
        public void ThenAgentMovesTo(string agentName, string positionId)
        {
            var agent = Governors[agentName];
            var result = agent.Object.Move(positionId);  

            Assert.AreEqual(result.Code, ActionInfo.Ok.Code);
        }

        [Then(@"Agent '(.*)' has '(.*)' possibilities")]
        public void ThenAgentHasPossibilities(string agentName, int numPossibilities)
        {
            var agent = Governors[agentName];
            Assert.AreEqual(numPossibilities, agent.Object.FeasibleActions().Length);
        }

        [Then(@"Agent '(.*)' possible action id is '(.*)'")]
        public void ThenAgentPossibleActionIdIs(string agentName, string actionId)
        {
            var agent = Governors[agentName];
            Assert.IsTrue(agent.Object.FeasibleActions().Any(w => w != null && w.Id == actionId), 
                string.Format("Agent has no possibility for '{0}'. Possible actions are '{1}'.", actionId, 
                    string.Join(",", agent.Object.FeasibleActions().Select(w => w == null ? "<none>" : w.Id).ToArray())));
        }


        [Then(@"Agent '(.*)' exits workflow from '(.*)' to '(.*)'")]
        public void ThenAgentExitsWorkflowVia(string agentName, string fromId, string toId)
        {
            var callback = Callbacks[agentName];
            callback.Verify(w => w.ExitedWorkflow(fromId, It.IsAny<string>(), toId, It.IsAny<string>()));
        }

        [Then(@"Agent '(.*)' exits institution")]
        public void ThenAgentExitsInstitution(string agentName)
        {
            var callback = Callbacks[agentName];
            callback.Verify(w => w.ExitedInstitution(It.IsAny<string>(), It.IsAny<string>()));
        }

        [Then(@"Agent '(.*)' can join '(.*)' workflows in '(.*)'")]
        public void ThenAgentCanJoinWorkflowsIn(string agentName, int workflowCount, string workflowId)
        {
            var agent = Governors[agentName];
            var workflows = agent.Object.GetWorkflowInfos(workflowId);

            Assert.AreEqual(workflowCount, workflows.Length);
        }

        [Then(@"Agent '(.*)' wait move to '(.*)' in workflow '(.*)'")]
        public void ThenAgentWaitMoveToPortInWorkflow(string agentName, string positionId, string workflowId)
        {
            var agent = Governors[agentName];

            //Console.WriteLine("[AGENT POSITION 1] " + agent.Object.Name + " - " + agent.Object.Workflow.Id + "." + agent.Object.Port.Action.Id + "." + agent.Object.Port.Id);
             
            if (agent.Object.Workflow.Id == workflowId && agent.Object.Position.Id == positionId)
            {
                return;
            }
            
            // wait for callback
            var callback = Callbacks[agentName];
            var waiter = new AutoResetEvent(false);

            callback.Setup(w => w.ChangedPosition(agentName, workflowId, It.IsAny<int>(), positionId)).Callback(() =>
            {
                if (Log.IsDebug) Log.Debug("[AGENT ARRIVED] " + agent.Object.Name + " - " + agent.Object.Workflow.Id + "." + agent.Object.Position.Id);
                waiter.Set();
            }); 

            try
            {
                // we may have actually called this before we setup so we check
                callback.Verify(w => w.ChangedPosition(agentName, workflowId, It.IsAny<int>(), positionId));
                if (Log.IsDebug) Log.Debug("[AGENT ALREADY THERE] " + agent.Object.Name + " - " + agent.Object.Workflow.Id + "." + agent.Object.Position.Id);
                waiter.Set();
            }
            catch 
            {
                if (Log.IsDebug) Log.Debug("[NO CALL]");
            }

            //Console.WriteLine("[WAITING]"); 
            var result = waiter.WaitOne(2000);

            //Console.WriteLine("[AGENT POSITION 2] " + agent.Object.Name + " - " + agent.Object.Workflow.Id + "." + agent.Object.Port.Action.Id + "." + agent.Object.Port.Id);

            Assert.IsTrue(result, "Wait timeout: Agent has never arrived at the desired destination"); 
        }

        [Then(@"Agent '(.*)' is notified of '(.*)' by '(.*)' with '(.*)' in '(.*)'")]
        public void ThenAgentIsNotifiedOfByWithIn(string toAgentName, string activityId, string fromAgentName, string parameters, string workflowId)
        {
            var callback = Callbacks[toAgentName];
           
            callback.Verify(w => w.NotifyActivity(
                toAgentName,
                workflowId, 
                It.IsAny<int>(), 
                fromAgentName, 
                activityId,
                It.IsAny<VariableState>()
            // It.Is<VariableState>(z => parameters == string.Join(";", z.Select(i => i.ToString())))
            ));
        }  

        [Then(@"Agent '(.*)' fails exit workflow")]
        public void ThenAgentFailsExitWorkflow(string agentName)
        {
            var agent = Governors[agentName];
            var result = agent.Object.ExitWorkflow();

            Assert.AreEqual(InstitutionCodes.Failed, result.Code);
        }

        [Then(@"Agent '(.*)' splits to '(.*)' '(.*)' clones")]
        public void ThenAgentSplitsToClones(string agentName, int cloneCount, string shallow)
        {
            var callback = Callbacks[agentName];
            callback.Verify(w => w.Split(It.Is<Governor[]>(z => z.Length == cloneCount), shallow == "shallow"));
        }

        [Then(@"Agent '(.*)' does not yet join")]
        public void ThenAgentDoesNotYetJoin(string agentName)
        {
            var callback = Callbacks[agentName];
            callback.Verify(w => w.Joined(), Times.Never);
        }

        [Then(@"Agent '(.*)' is in position '(.*)'")]
        public void ThenAgentIsInPosition(string agentName, string positionId)
        {
            var agent = Governors[agentName];
            Assert.AreEqual(positionId, agent.Object.Position.Id);
        }

        [Then(@"Agent '(.*)' is in workflow '(.*)' position '(.*)'")]
        public void ThenAgentIsinActivityPortInWorkflow(string agentName, string workflowId, string positionId)
        {
            var agent = Governors[agentName];

            Assert.AreEqual(workflowId, agent.Object.Workflow.Id);
            Assert.AreEqual(positionId, agent.Object.Position.Id);
        }

        // notifications

        [Then(@"Agent '(.*)' notifies institution entry with id '(.*)' and name '(.*)'")]
        public void ThenGovernorEntersInstitutionWithIdAndName(string agentName, string id, string name)
        {
            var callback = Callbacks[agentName];
            callback.Verify(w => w.EnteredInstitution(id, name));
        }

        [Then(@"Agent '(.*)' notifies workflow entry with id '(.*)' and name '(.*)'")]
        public void ThenGovernorEntersWorkflowWithIdAndName(string agentName, string id, string name)
        {
            var callback = Callbacks[agentName];
            callback.Verify(w => w.EnteredWorkflow(It.IsAny<string>(), id, name));
        }

        [Then(@"Agent '(.*)' notifies change position to '(.*)' in workflow '(.*)'")]
        public void ThenAgentNotifiesChangePositionToId(string agentName, string positionId, string workflowId)
        {
            var callback = Callbacks[agentName];
            callback.Verify(w => w.ChangedPosition(agentName, workflowId, It.IsAny<int>(), positionId));
        }

        [Then(@"Agent '(.*)' notifies workflow exit '(.*)' id '(.*)' to '(.*)' id '(.*)'")]
        public void ThenGovernorExitsFromWorkflowIdToId(string agentName, string fromName, string fromId, string toName, string toId)
        {
            var callback = Callbacks[agentName];

            toName = toName == "null" ? null : toName;
            toId = toId == "null" ? null : toId;

            callback.Verify(w => w.ExitedWorkflow(fromId, fromName, toId, toName));
        }

        [Then(@"Agent '(.*)' notifies institution exit '(.*)' id '(.*)'")]
        public void ThenGovernorExitsInstitutionId(string agentName, string name, string id)
        {
            var callback = Callbacks[agentName];
            callback.Verify(w => w.ExitedInstitution(id, name));
        }

        [When(@"Agent '(.*)' int parameter '(.*)' is set to '(.*)'")]
        public void WhenAgentParameterIsSetTo(string agentName, string parameterName, string parameterValue)
        {
            var agent = Governors[agentName];
            var provider = agent.Object.VariableState.FindProvider(parameterName);
            provider.Descriptors.First(d => d.Name == parameterName).Update(provider, int.Parse(parameterValue));
        }


        [Then(@"Agent '(.*)' has parameter '(.*)'")]
        public void ThenAgentHasParameter(string agentName, string parameterName)
        {
            var agent = Governors[agentName];
            var provider = agent.Object.VariableState.FindProvider(parameterName);
            Assert.IsTrue(provider.Descriptors.Any(d => d.Name == parameterName), "Agent does not have parameter: " + parameterName);
        }


    }
}

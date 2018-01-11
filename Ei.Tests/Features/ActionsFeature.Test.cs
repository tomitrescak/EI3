using Ei.Tests.Steps;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Ei.Tests.Features
{
    public class ActionsFeatureTest
    {
        [Fact]
        public void StartJoinWorkflow() {
            var scenarioContext = new ScenarioContext();
            var c = new InstitutionConnectionSteps(scenarioContext);
            var a = new ActivitySteps(scenarioContext);

            // Given That institution 'InstitutionStart' is launched
            c.GivenThatInstitutionIsRunning("InstitutionStart");
            // When Agent 'user1' connects with '123' to organisation 'Default'
            c.WhenAgentConnectsWithPasswordInOrganisation("user1", "123", "Default");
            // When Agent 'user2' connects with '123' to organisation 'Default'
            c.WhenAgentConnectsWithPasswordInOrganisation("user2", "123", "Default");
            // When Agent 'user3' connects with '123' to organisation 'Default'
            c.WhenAgentConnectsWithPasswordInOrganisation("user3", "123", "Default");


            // # start a new workflow


            // Then Agent 'user1' cannot perform 'joinSubWorkflow' with 'WorkflowInstanceNotRunning'
            a.ThenAgentCannotPerformWith("user1", "joinSubWorkflow", "WorkflowInstanceNotRunning");

            // When Agent 'user1' performs 'startSubWorkflow'
            a.ThenAgentPerformsActivity("user1", "startSubWorkflow");

            // When Agent 'user1' joins workflow 'joinSubWorkflow' with 'InstanceId=0'
            a.ThenAgentPerformsActivityWith("user1", "joinSubWorkflow", "InstanceId=0");

            // Then Agent 'user1' is in workflow 'subWorkflow' position 'start'
            a.ThenAgentIsInWorkflowPosition("user1", "subWorkflow", "start");

            // When Agent 'user2' performs 'joinSubWorkflow'
            a.ThenAgentPerformsActivity("user2", "joinSubWorkflow");

            // Then Agent 'user2' is in workflow 'subWorkflow' position 'start'
            a.ThenAgentIsInWorkflowPosition("user2", "subWorkflow", "start");


            // # say message, test validations


            // Then Agent 'user2' cannot perform 'send' with 'InvalidParameters'
            a.ThenAgentCannotPerformWith("user2", "send", "InvalidParameters");

            // Then Agent 'user2' fails 'send' with 'Stones=0' and message 'Stones: Value Required'
            a.ThenAgentFailsActivityWithMessage("user2", "send", "Stones=0", "Stones: Value Required");

            // Then Agent 'user2' fails 'send' with 'Stones=2;Weight=100' and message 'Weight needs to be max 10'
            a.ThenAgentFailsActivityWithMessage("user2", "send", "Stones=2;Weight=100", "Weight needs to be max 10");

            // When Agent 'user2' performs action 'send' with 'Stones=3;Weight=3'
            a.ThenAgentPerformsActivityWith("user2", "send", "Stones=3;Weight=3");

            // Then Agent 'user1' is notified of 'send' by 'user2' with 'c' in 'subWorkflow'
            a.ThenAgentIsNotifiedOfByWithIn("user1", "send", "user2", "c", "subWorkflow");

            // Then Agent 'user2' is notified of 'send' by 'user2' with 'c' in 'subWorkflow'
            a.ThenAgentIsNotifiedOfByWithIn("user2", "send", "user2", "c", "subWorkflow");


            // # test timeouts


            // When Agent 'user1' moves to 'wait'
            a.ThenAgentMovesTo("user1", "wait");

            // Then Agent 'user1' wait move to 'yield' in workflow 'subWorkflow'
            a.ThenAgentWaitMoveToPortInWorkflow("user1", "yield", "subWorkflow");


            // # user 3 can now join workflow as number of stones is bigger


            // Then Agent 'user3' can join '1' workflows in 'joinSubWorkflow'
            a.ThenAgentCanJoinWorkflowsIn("user3", 1, "joinSubWorkflow");

            // When Agent 'user3' performs 'joinSubWorkflow'
            a.ThenAgentPerformsActivity("user3", "joinSubWorkflow");

            // Then Agent 'user3' is in workflow 'subWorkflow' position 'yield'
            a.ThenAgentIsInWorkflowPosition("user3", "subWorkflow", "yield");


            // # exit workflow, move agents to final state and try to re-enter


            // Agent 'user3' exits workflow
            a.ThenAgentExitsWorkflow("user3");

            // When Agent 'user1' moves to 'end'
            a.ThenAgentMovesTo("user1", "end");

            // Then Agent 'user1' exits workflow from 'subWorkflow' to 'main
            a.ThenAgentExitsWorkflowTo("user1", "subWorkflow", "main");

            // Then Agent 'user2' exits workflow from 'subWorkflow' to 'main'
            a.ThenAgentExitsWorkflowTo("user2", "subWorkflow", "main");


            // # when agent tries to re-enter this instance=0 is no longer running


            // Agent 'user1' is in position 'inc'
            a.ThenAgentIsInPosition("user1", "inc");

            // When Agent 'user3' moves to 'start'
            a.ThenAgentMovesTo("user3", "start");

            // Then Agent 'user3' cannot perform 'joinSubWorkflow' with 'WorkflowInstanceNotRunning
            a.ThenAgentCannotPerformWith("user3", "joinSubWorkflow", "WorkflowInstanceNotRunning");
        }

        [Fact]
        public void SplitandJoin() {
            var scenarioContext = new ScenarioContext();
            var c = new InstitutionConnectionSteps(scenarioContext);
            var a = new ActivitySteps(scenarioContext);

            // Given That institution 'InstitutionStart' is launched
            c.GivenThatInstitutionIsRunning("InstitutionStart");
            // When Agent 'user1' connects with '123' to organisation 'Default'
            c.WhenAgentConnectsWithPasswordInOrganisation("user", "123", "Default");

            // When Agent 'user' moves to 'inc'
            a.ThenAgentMovesTo("user", "inc");

            // When Agent 'user' moves to 'split'
            a.ThenAgentMovesTo("user", "split");

            // Then Agent 'user' splits to '2' 'shallow' clones
            a.ThenAgentSplitsToClones("user", 2, "shallow");

            // When Clone 'user_Left' of 'user' moves to 'join
            a.ThenAgentMovesTo("user_Left", "user", "join");

            // Then Agent 'user' does not yet join
            a.ThenAgentDoesNotYetJoin("user");

            // When Clone 'user_Right' of 'user' moves to 'join'
            a.ThenAgentMovesTo("user_Right", "user", "join");

            // Then Agent 'user' is in position 'joined'
            a.ThenAgentIsInPosition("user", "joined");
        }

        [Fact]
        public void PlanningWithoutCycles() {
            var scenarioContext = new ScenarioContext();
            var c = new InstitutionConnectionSteps(scenarioContext);
            var a = new ActivitySteps(scenarioContext);
            var p = new PlanningSteps(scenarioContext);

            // Given That institution 'InstitutionStart' is launched
            c.GivenThatInstitutionIsRunning("InstitutionStart");
            // When Agent 'user1' connects with '123' to organisation 'Default'
            c.WhenAgentConnectsWithPasswordInOrganisation("user", "123", "Default");
            // And Agent 'user' plans to change state to 'ParentParameter=1' with strategy 'ForwardSearch'
            p.WhenAgentPlansToChangeStateToWithStrategy("user", "ParentParameter=1", "ForwardSearch");
            // Then Plan length is '2'
            p.ThenPlanLengthIs(2);
        }

        [Fact]
        public void PlanningWithCycles() {
            var scenarioContext = new ScenarioContext();
            var c = new InstitutionConnectionSteps(scenarioContext);
            var a = new ActivitySteps(scenarioContext);
            var p = new PlanningSteps(scenarioContext);

            // Given That institution 'InstitutionStart' is launched
            c.GivenThatInstitutionIsRunning("InstitutionStart");
            // When Agent 'user1' connects with '123' to organisation 'Default'
            c.WhenAgentConnectsWithPasswordInOrganisation("user", "123", "Default");
            // And Agent 'user' plans to change state to 'ParentParameter=1' with strategy 'ForwardSearch'
            p.WhenAgentPlansToChangeStateToWithStrategy("user", "ParentParameter=2", "ForwardSearch");
            // Then Plan length is '4'
            p.ThenPlanLengthIs(4);
        }

    }
}

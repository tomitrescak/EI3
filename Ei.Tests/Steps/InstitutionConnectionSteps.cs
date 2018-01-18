using Ei.Compilation;
using Ei.Persistence.Json;
using Microsoft.VisualStudio.TestPlatform.Common.DataCollection;

namespace Ei.Tests.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Moq;

    using Logs;
    using Ontology;
    using Runtime;
    using Ei.Tests.Bdd.Institutions;
    using Xunit;

    // [Binding]
    public class InstitutionConnectionSteps
    {
        Mock<Governor> Governor => this.scenarioContext.Get<Mock<Governor>>();
        Mock<IGovernorCallbacks> Callback => this.scenarioContext.Get<Mock<IGovernorCallbacks>>();
        Mock<ILog> Logger => this.scenarioContext.Get<Mock<ILog>>();
        Mock<InstitutionManager> Manager => this.scenarioContext.Get<Mock<InstitutionManager>>();

        Dictionary<string, Mock<Governor>> Governors => this.scenarioContext.Get<Dictionary<string, Mock<Governor>>>();
        Dictionary<string, Mock<IGovernorCallbacks>> Callbacks => this.scenarioContext.Get<Dictionary<string, Mock<IGovernorCallbacks>>>();

        private readonly ScenarioContext scenarioContext;

        public InstitutionConnectionSteps(ScenarioContext scenarioContext) {
            if (scenarioContext == null) throw new ArgumentNullException("scenarioContext");
            this.scenarioContext = scenarioContext;
        }


        // [Given(@"That institution '(.*)' is launched")]
        public void GivenThatInstitutionIsRunning(string path) {
            var managerMock = new Mock<InstitutionManager> { CallBase = true };
            var logMock = new Mock<ILog>();

            Log.Register(new ConsoleLog());
            Log.Register(logMock.Object);

            // start institution
            Institution ei;
            switch (path) {
                case "InstitutionStart":
                    ei = new ConnectionTestEi();
                    break;
                case "ConnectionTest.json":
                    var dao = JsonInstitutionLoader.Instance.LoadFromFile("Files/" + path);
                    var code = dao.GenerateAll();
                    var result = Compiler.Compile(code, "DefaultInstitution", out ei);
                    Assert.Null(result);
                    break;
                default:
                    throw new NotImplementedException($"Institution '{path}' is not implemented");
            }
            managerMock.Object.Start(ei);

            this.scenarioContext.Set<Mock<ILog>>(logMock);
            this.scenarioContext.Set<Mock<InstitutionManager>>(managerMock);

            // add also collections in which we will store all agents and their callbacks
            this.scenarioContext.Set(new Dictionary<string, Mock<Governor>>());
            this.scenarioContext.Set(new Dictionary<string, Mock<IGovernorCallbacks>>());
        }

        // [When(@"Agent '(.*)' connects to organisation '(.*)'")]
        public void WhenAgentConnectsWithOrganisation(string name, string organisation) {
            this.WhenAgentConnectsWithPasswordInOrganisation(name, null, organisation);
        }


        // [When(@"Agent '(.*)' connects with '(.*)' to organisation '(.*)'")]
        public void WhenAgentConnectsWithPasswordInOrganisation(string name, string password, string organisation) {
            var governor = new Mock<Governor>() { CallBase = true };
            var callback = new Mock<IGovernorCallbacks>();

            // we return our mocked governor so that we can observe changes on him
            Manager.Setup(x => x.CreateGovernor()).Returns(governor.Object);
            Governors.Add(name, governor);
            Callbacks.Add(name, callback);

            Governor gov;
            var code = Manager.Object.Connect(callback.Object, organisation, name, password, null, out gov);

            Assert.Equal(InstitutionCodes.Ok, code);

            // automatically continue
            //governor.Object.Continue();   
        }

        // [Then(@"Agent '(.*)' plays role '(.*)'")]
        public void ThenAgentPlayesRole(string agentName, string role) {
            var agent = Governors[agentName];
            Assert.Contains(agent.Object.Groups, w => w.Role.Id == role);
        }

        // [When(@"Agent '(.*)' connects with credentials '(.*)' and role '(.*)'")]
        public void WhenAgentConnectsWithCredentialsAndRole(string user, string password, string roles) {
            var managerMock = this.scenarioContext.Get<Mock<InstitutionManager>>();

            // create new governor mock
            var governor = new Mock<Governor>();
            governor.CallBase = true;
            Governors.Add(user, governor);
            var callbackMock = new Mock<IGovernorCallbacks>();
            Callbacks.Add(user, callbackMock);

            this.scenarioContext.Set<Mock<IGovernorCallbacks>>(callbackMock);
            this.scenarioContext.Set<Mock<Governor>>(governor);

            //            governor.CallBase = true;
            //            governor.Setup(
            //                w => w.LogAction(It.IsAny<InstitutionCodes>(), It.IsAny<string[]>()))
            //                    .Callback<InstitutionCodes, object[]>(
            //                        (w, p) =>
            //                        {
            //                            Console.WriteLine($"Action called with '{w}' and '{ string.Join(",", p)}'");
            //                        });

            // we return our mocked governor so that we can observe changes on him
            managerMock.Setup(x => x.CreateGovernor()).Returns(governor.Object);

            // parse roles
            var roleSplits = roles.Split(';');
            var orgRoles = roleSplits.Select(roleSplit => roleSplit.Split(',')).ToArray();
            Governor gov;
            var code = managerMock.Object.Connect(callbackMock.Object, null, "user", "pass", orgRoles, out gov);

            // perform first actions
            governor.Object.Continue();

            Assert.Equal(InstitutionCodes.Ok, code);
        }

        // [Then(@"Institution has '(.*)' agents")]
        public void ThenInstitutionHasAgents(int count) {
            Assert.Equal(count, Manager.Object.ConnectedAgents);
        }
    }
}

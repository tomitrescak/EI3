﻿namespace Ei.Tests.Steps
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using Moq;
  using TechTalk.SpecFlow;

  using Logs;
  using Ontology;
  using Runtime;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using Ei.Tests.Bdd.Institutions;

  [Binding]
  public class InstitutionConnectionSteps
  {
    Mock<Governor> Governor => ScenarioContext.Current.Get<Mock<Governor>>();
    Mock<IGovernorCallbacks> Callback => ScenarioContext.Current.Get<Mock<IGovernorCallbacks>>();
    Mock<ILog> Logger => ScenarioContext.Current.Get<Mock<ILog>>();
    Mock<InstitutionManager> Manager => ScenarioContext.Current.Get<Mock<InstitutionManager>>();

    Dictionary<string, Mock<Governor>> Governors => ScenarioContext.Current.Get<Dictionary<string, Mock<Governor>>>();
    Dictionary<string, Mock<IGovernorCallbacks>> Callbacks => ScenarioContext.Current.Get<Dictionary<string, Mock<IGovernorCallbacks>>>();

    [Given(@"That institution '(.*)' is launched")]
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
        default:
          throw new NotImplementedException($"Institution '{path}' is not implemented");
      }
      managerMock.Object.Start(ei);

      ScenarioContext.Current.Set<Mock<ILog>>(logMock);
      ScenarioContext.Current.Set<Mock<InstitutionManager>>(managerMock);

      // add also collections in which we will store all agents and their callbacks
      ScenarioContext.Current.Set(new Dictionary<string, Mock<Governor>>());
      ScenarioContext.Current.Set(new Dictionary<string, Mock<IGovernorCallbacks>>());
    }

    [When(@"Agent '(.*)' connects to organisation '(.*)'")]
    public void WhenAgentConnectsWithOrganisation(string name, string organisation) {
      this.WhenAgentConnectsWithOrganisationPassword(name, organisation, null);
    }


    [When(@"Agent '(.*)' connects with organisation '(.*)' password '(.*)'")]
    public void WhenAgentConnectsWithOrganisationPassword(string name, string organisation, string password) {
      var governor = new Mock<Governor>() { CallBase = true };
      var callback = new Mock<IGovernorCallbacks>();

      // we return our mocked governor so that we can observe changes on him
      Manager.Setup(x => x.CreateGovernor()).Returns(governor.Object);
      Governors.Add(name, governor);
      Callbacks.Add(name, callback);

      Governor gov;
      var code = Manager.Object.Connect(callback.Object, organisation, name, password, null, out gov);

      Assert.AreEqual(InstitutionCodes.Ok, code);

      // automatically continue
      //governor.Object.Continue();   
    }

    [Then(@"Agent '(.*)' plays role '(.*)'")]
    public void ThenAgentPlayesRole(string agentName, string role) {
      var agent = Governors[agentName];
      Assert.IsTrue(agent.Object.Groups.Any(w => w.Role.Id == role));
    }

    [When(@"Agent '(.*)' connects with credentials '(.*)' and role '(.*)'")]
    public void WhenAgentConnectsWithCredentialsAndRole(string user, string password, string roles) {
      var managerMock = ScenarioContext.Current.Get<Mock<InstitutionManager>>();

      // create new governor mock
      var governor = new Mock<Governor>();
      governor.CallBase = true;
      Governors.Add(user, governor);
      var callbackMock = new Mock<IGovernorCallbacks>();
      Callbacks.Add(user, callbackMock);

      ScenarioContext.Current.Set<Mock<IGovernorCallbacks>>(callbackMock);
      ScenarioContext.Current.Set<Mock<Governor>>(governor);

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

      Assert.AreEqual(code, InstitutionCodes.Ok);
    }

    [Then(@"Institution has '(.*)' agents")]
    public void ThenInstitutionHasAgents(int count) {
      Assert.AreEqual(count, Manager.Object.ConnectedAgents);
    }
  }
}

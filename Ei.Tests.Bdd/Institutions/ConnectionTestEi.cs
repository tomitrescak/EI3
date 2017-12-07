using Ei.Ontology;
using Ei.Ontology.Actions;
using Ei.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ei.Tests.Bdd.Institutions
{
  public class ConnectionTestEi : Institution
  {
    public ConnectionTestEi() : base("ConnectionTest") {

      // init basic properties
      this.Name = "Connection Test";
      this.Description = "Connection Test Description";

      // init components
      this.InitRoles();

      // init workflows
      this.InitWorkflows();
    }

    // init institutional parts

    private void InitRoles() {
      var citizenRole = new Role("1");
    }

    private void InitWorkflows() {
      this.MainWorkflowId = "1";
      this.Workflows = new ReadOnlyCollection<Workflow>(new[] { });
    }

    // abstract implementation 

    public override Institution Instance {
      get {
        return new ConnectionTestEi();
      }
    }
  }

  public class CitizenRole : Role
  {
    public CitizenRole() : base("1") {
      this.Name = "Citizen";
      this.Description = null;
    }

    public class Properties : VariableState
    {
      public int ParentParameter { get; set; }
    }
  }

  public class MainWorkflow : Workflow
  {
    public MainWorkflow(Institution institution, Workflow parent, int instanceId) : base(institution, parent, instanceId) {
      this.Id = "1";
      this.Name = "Main Workflow";
    }

    public override List<Connection> Connections => throw new NotImplementedException();

    public override WorkflowVariableState VariableState => throw new NotImplementedException();

    public override ReadOnlyCollection<Governor> Agents => throw new NotImplementedException();

    public override bool Stateless => throw new NotImplementedException();

    public override bool Static => throw new NotImplementedException();

    public override ReadOnlyCollection<State> States => throw new NotImplementedException();

    public override ReadOnlyCollection<ActionBase> Actions => throw new NotImplementedException();

    public override Access CreatePermissions => throw new NotImplementedException();

    public override Workflow CreateInstance(Institution ei, Workflow parentWorkflow, int instanceId) {
      throw new NotImplementedException();
    }
  }
}

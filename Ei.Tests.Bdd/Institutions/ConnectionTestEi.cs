using Ei.Ontology;
using Ei.Ontology.Actions;
using Ei.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ei.Ontology.Institution;

namespace Ei.Tests.Bdd.Institutions
{
    #region class ConnectionTestEi
    public class ConnectionTestEi : Institution<Institution.ResourceState>
    {
        private ResourceState state;

        public ConnectionTestEi() : base("ConnectionTest") {
            // init basic properties
            this.Name = "Connection Test";
            this.Description = "Connection Test Description";

            // init state
            this.state = new ResourceState(this);

            // init organisations
            this.AddOrganisations(
                new DefaultOrganisation()
            );

            // init components
            this.AddRoles(
                new CitizenRole()
            );

            // init workflows
            this.AddWorkflows(
                new MainWorkflow(this)
            );
            this.MainWorkflowId = this.Workflows[0].Id;

            // init security
            this.AuthenticationPermissions.Init(new List<AuthorisationInfo> {
                new AuthorisationInfo(this, "user", null, null, new [] { this.GroupByName(new [] { "Citizen" } )}),
                new AuthorisationInfo(this, "user", "org", "Default", new [] { this.GroupByName(new [] { "Citizen" } )})
            }, this);
        }

        // abstract implementation 

        public override Institution Instance {
            get {
                return new ConnectionTestEi();
            }
        }

        public override Institution.ResourceState Resources { get { return new ResourceState(this); } }
    }
    #endregion

    #region class DefaultOrganisation
    public class DefaultOrganisation : Organisation
    {
        public DefaultOrganisation() : base("default") {
            this.Name = "Default";
        }

        public class Resources : Ei.Runtime.ResourceState { }

        public override Ei.Runtime.ResourceState CreateState() {
            return new Resources();
        }
    }
    #endregion

    #region class CitizenRole
    public class CitizenRole : Role
    {
        public CitizenRole() : base("1") {
            this.Name = "Citizen";
            this.Description = null;
        }

        public override Ei.Runtime.ResourceState CreateState() {
            return new Resources();
        }

        public class Resources : Runtime.ResourceState
        {
            public int ParentParameter { get; set; }
        }
    }
    #endregion

    #region class MainWorkflow
    public class MainWorkflow : Workflow
    {
        // 
        public class Resources : ResourceState
        {
            public Resources(Workflow workflow) : base(workflow) {
            }
        }

        // variables must be initialised here as readonly values are done in constructor

        List<Connection> connections = new List<Connection>();
        List<State> states = new List<State>();
        List<ActionBase> actions = new List<ActionBase>();

        // properties

        protected override List<Connection> WorkflowConnections { get { return this.connections; } }

        protected override List<State> WorkflowStates { get { return this.states; } }

        protected override List<ActionBase> WorkflowActions { get { return this.actions; } }

        public override bool Stateless { get { return true; } }

        public override bool Static { get { return true; } }

        public override Access CreatePermissions { get { return null; } }

        public override ResourceState CreateState() {
            return new Resources(this);
        }

        // constructor

        public MainWorkflow(Institution ei, string id = "main") : base(ei, id) {
            this.Name = "Main";

            // add states
            var startState = new State("start", "Start", "", this, false, 0, null, null, true, false);
            var endState = new State("end", "End", "", this, false, 0, null, null, false, true);
            var incState = new State("inc", "Inc", "", this);

            this.states.AddRange(new[] {
                startState,
                endState
            });

            this.Connect(startState, endState)
                .Condition(new AccessCondition<Institution.ResourceState, Resources, DefaultOrganisation.Resources, CitizenRole.Resources, Runtime.ResourceState>()
                    .Allow(
                        (i, w, o, r, a) => {
                            return r.ParentParameter > 0;
                        }
                     ));

            this.Connect(startState, incState)
                .Condition(new AccessCondition<Institution.ResourceState, Resources, DefaultOrganisation.Resources, CitizenRole.Resources, Runtime.ResourceState>()
                    .Action((i, w, o, r, a) => {
                        r.ParentParameter++;
                 }));

            this.Connect(incState, startState);



            // IMPORTANT: this needs to be called to initialise connections
            this.Init();
        }
    }
    #endregion
}

﻿using Ei.Ontology;
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
    public class ConnectionTestEi : Institution<InstitutionState>
    {
        private InstitutionState state;
   
        public ConnectionTestEi() : base("ConnectionTest") {
            // init basic properties
            this.Name = "Connection Test";
            this.Description = "Connection Test Description";

            // init state
            this.state = new InstitutionState(this);

            // init organisations
            this.InitOrganisations();

            // init components
            this.InitRoles();

            // init workflows
            this.InitWorkflows();

            // init security
            this.AuthenticationPermissions.Init(new List<AuthorisationInfo> {
                new AuthorisationInfo(this, "user", null, null, new [] { this.GroupByName(new [] { "Citizen" } )}),
                new AuthorisationInfo(this, "user", "org", "Default", new [] { this.GroupByName(new [] { "Citizen" } )})
            }, this);
        }

        // init institutional parts

        private void InitRoles() {
            var citizenRole = new CitizenRole();
            this.Roles = new ReadOnlyCollection<Role>(new[] { citizenRole });
        }

        private void InitOrganisations() {
            var defaultOrganisation = new Organisation("1", "Default", "Default Organisation");
            this.Organisations = new ReadOnlyCollection<Organisation>(new[] { defaultOrganisation });
        }

        private void InitWorkflows() {
            this.MainWorkflowId = "main";
            this.Workflows = new ReadOnlyCollection<Workflow>(new[] {
                new MainWorkflow(this)
            });
        }

        // abstract implementation 

        public override Institution Instance {
            get {
                return new ConnectionTestEi();
            }
        }

        public override InstitutionState VariableState { get { return this.state; } }
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

    public class StartStopAccessCondition : AccessCondition<InstitutionState, MainWorkflow.Properties, CitizenRole.Properties>
    {
        public override bool CheckConditions(InstitutionState institutionState, MainWorkflow.Properties workflowState, CitizenRole.Properties agentState) {
            return agentState.ParentParameter > 0;
        }
    }

    public class MainWorkflow : Workflow
    {
        // 
        public class Properties : WorkflowVariableState
        {
            public Properties(Workflow workflow) : base(workflow) {
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

        public override WorkflowVariableState CreateState() {
            return new Properties(this);
        }

        // constructor

        public MainWorkflow(Institution ei, string id = "main") : base(ei, id) {
            this.Name = "Main Workflow";

            // add states
            var startState = new State("start", "Start", "", this, false, 0, null, null, true, false);
            var endState = new State("end", "End", "", this, false, 0, null, null, false, true);

            this.states.AddRange(new[] {
                startState,
                endState
            });

            var connection = new Connection(ei, startState, endState, null) {
                Preconditions = new Access(ei,
                    new[] {
                        new StartStopAccessCondition()
                    }, null)
            };

            // IMPORTANT: this needs to be called to initialise connections
            this.Init();
        }
    }
}

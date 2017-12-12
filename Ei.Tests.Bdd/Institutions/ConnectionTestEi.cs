using Ei.Ontology;
using Ei.Ontology.Actions;
using Ei.Ontology.Transitions;
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
    public class ConnectionTestEi : Institution<Institution.InstitutionState>
    {
        private InstitutionState state;

        public ConnectionTestEi() : base("ConnectionTest") {
            // init basic properties
            this.Name = "Connection Test";
            this.Description = "Connection Test Description";  

            // init state
            this.state = new InstitutionState(this);

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
                new MainWorkflow(this),
                new SubWorkflow(this)
            );
            this.MainWorkflowId = this.Workflows[0].Id;

            // init security
            this.AuthenticationPermissions.Init(new List<AuthorisationInfo> {
                new AuthorisationInfo(this, "user", null, null, new [] { this.GroupByName(new [] { "Citizen" } )}),
                new AuthorisationInfo(this, null, "123", "Default", new [] { this.GroupByName(new [] { "Citizen" } )})
            }, this);
        }

        // abstract implementation 

        public override Institution Instance {
            get {
                return new ConnectionTestEi();
            }
        }

        public override Institution.InstitutionState Resources { get { return new InstitutionState(this); } }
    }
    #endregion

    #region class DefaultOrganisation
    public class DefaultOrganisation : Organisation
    {
        public DefaultOrganisation() : base("default") {
            this.Name = "Default";
        }

        public class DefaultResources : Ei.Runtime.ResourceState { }

        protected override Ei.Runtime.ResourceState CreateState() {
            return new DefaultResources();
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
            return new CitizenResources();
        }

        public class CitizenResources : Runtime.ResourceState
        {
            public int ParentParameter { get; set; }
        }
    }
    #endregion

    #region AccessFactory
    static class AccessFactory
    {
        public static AccessCondition<Institution.InstitutionState, MainWorkflow.WorkflowState, DefaultOrganisation.DefaultResources, CitizenRole.CitizenResources, Runtime.ResourceState> MainDefaultCitizen {
            get { return new AccessCondition<Institution.InstitutionState, MainWorkflow.WorkflowState, DefaultOrganisation.DefaultResources, CitizenRole.CitizenResources, Runtime.ResourceState>(); }
        }
        public static AccessCondition<Institution.InstitutionState, SubWorkflow.Resources, DefaultOrganisation.DefaultResources, CitizenRole.CitizenResources, Runtime.ResourceState> SubWorkflowDefaultCitizen {
            get { return new AccessCondition<Institution.InstitutionState, SubWorkflow.Resources, DefaultOrganisation.DefaultResources, CitizenRole.CitizenResources, Runtime.ResourceState>(); }
        }
    }
    #endregion

    #region class MainWorkflow
    public class MainWorkflow : Workflow
    {


        // action parameters

        class SubWorkflowParameters : ResourceState
        {
            int Granite { get; set; }

            public override string Validate() {
                if (Granite < 0) {
                    return "Granite needs to be bigger then 0";
                }
                return null;
            }

            public override ResourceState Clone(ResourceState state = null) {
                // improving performance
                return new SubWorkflowParameters().Merge(this, true);
            }
        } 

        // properties

        public override bool Stateless { get { return true; } }

        public override bool Static { get { return true; } }

        // constructor

        public MainWorkflow(Institution ei, string id = "main") : base(ei, id) {
            this.Name = "Main";

            // add actions
            var joinSubworkflow = new ActionJoinWorkflow("joinSubWorkflow", ei, SubWorkflow.ID);
            var startSubWorkflow = new ActionStartWorkflow("startSubWorkflow", ei, joinSubworkflow, new SubWorkflowParameters());

            this.AddActions(
                joinSubworkflow,
                startSubWorkflow
            );

            // add states
            var startState = new State("start", "Start", "", this, false, 0, null, null, true, false);
            var endState = new State("end", "End", "", this, false, 0, null, null, false, true);
            var incState = new State("inc", "Inc", "", this);

            var s1State = new State("s1", "Left", "", this);
            var s2State = new State("s2", "Right", "", this);
            var joinedState = new State("joined", "Joined", "", this);

            var multiState = new State("actions", "Actions", "", this);
            var openState = new State("open", "Open", "", this, true);
            var workflowState = new State("workflow", "Workflow", "", this);
            var split = new TransitionSplit("split", "Split", "", true, new[] { new[] { "s1", "Left" }, new[] { "s2", "Right" } }, this);
            var join = new TransitionJoin("join", "Join", "", this);
      
            this.AddStates(
                startState,
                endState,
                incState,
                multiState,
                openState
            );

            // add connections

            this.Connect(startState, endState)
                .Condition(AccessFactory.MainDefaultCitizen
                    .Allow(
                        (i, w, o, r, a) => {
                            return r.ParentParameter > 0;
                        }
                     ));

            this.Connect(startState, incState)
                .Condition(AccessFactory.MainDefaultCitizen
                    .Action((i, w, o, r, a) => {
                        r.ParentParameter++;
                    }));

            this.Connect(incState, incState)
                .Condition(AccessFactory.MainDefaultCitizen
                    .Action(
                        (i, w, o, r, a) => {
                            return r.ParentParameter == 0;
                        },
                        (i, w, o, r, a) => {
                            r.ParentParameter += 10;
                        })
                    .Action((i, w, o, r, a) => {
                        return r.ParentParameter > 0;
                    }, (i, w, o, r, a) => {
                        r.ParentParameter = 0;
                    }));

            this.Connect(incState, startState);
            this.Connect(incState, null);
            this.Connect(null, incState)
                .Condition(AccessFactory.MainDefaultCitizen
                    .Action((i, w, o, r, a) => {
                        r.ParentParameter=3;
                    })
                );

            this.Connect(startState, startState, startSubWorkflow);
            this.Connect(startState, incState, joinSubworkflow);

            // check joins
            this.Connect(incState, split);
            this.Connect(split, s1State);
            this.Connect(split, s2State);
            this.Connect(s1State, join);
            this.Connect(s2State, join);
            this.Connect(join, joinedState);

            // IMPORTANT: this needs to be called to initialise connections
            this.Init();
        }
    }
    #endregion

    #region class SubWorkflow
    public class SubWorkflow : Workflow {
        public const string ID = "subWorkflow";

        #region class Resources
        public class Resources : WorkflowState {
            public int Stones { get; set; }
        }
        private Resources resources = new Resources();
        #endregion

        class SendActionParameters : WorkflowState {
            public int Stones { get; set; }
            public int Weight { get; set; }

            public override string Validate() {
                if (Stones == 0) {
                    return string.Format("Stones: Value Required", this.Stones);
                }
                if (Weight > 10) {
                    return string.Format("Weight needs to be max 10", this.Weight);
                }
                return null;
            }
        }

        // properties

        public override bool Stateless { get { return false; } }

        public override bool Static { get { return false; } }

        public override WorkflowState State { get { return this.resources; } }

        // constructor

        public SubWorkflow(Institution ei) : base(ei, SubWorkflow.ID) {
            this.Name = "Main";

            // define actions

            var sendAction = new ActionMessage("send", ei, new SendActionParameters(), ei.RolesByName("Default", "Citizen"), null);
            var timeout = new ActionTimeout("timeout", ei);

            this.AddActions(
                sendAction
            );

            // add states

            var startState = new State("start", "Start", "", this, false, 0, null, null, true, false);
            var waitState = new State("wait", "Wait", "", this, false, 1);
            var midState = new State("mid", "Mid", "", this);
            var yieldState = new State("yield", "Yield", "", this);
            var endState = new State("end", "End", "", this, false, 0, null, null, false, true);

            this.AddStates(
                startState,
                endState
            );

            // define connections

            this.Connect(startState, midState, sendAction)
                .Condition(new AccessCondition<Institution.InstitutionState, SubWorkflow.Resources, DefaultOrganisation.DefaultResources, CitizenRole.CitizenResources, SendActionParameters>()
                    .Action((i, w, o, r, a) => {
                        w.Stones = a.Stones;
                    }));

            this.Connect(midState, waitState);
            this.Connect(waitState, yieldState, timeout);
            this.Connect(yieldState, endState);

            // define constraints

            this.AddJoinPermissions(AccessFactory.SubWorkflowDefaultCitizen
                .Allow((i, w, o, r, a) => {
                    return w.AgentCount < 2 || w.Stones > 2;
                })    
            );
                

            // IMPORTANT: this needs to be called to initialise connections
            this.Init();
        }
    }
    #endregion
}
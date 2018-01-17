using Ei.Ontology;
using Ei.Ontology.Actions;
using Ei.Ontology.Transitions;
using Ei.Runtime;
using Ei.Runtime.Planning;
using System;
using System.Collections.Generic;

namespace Ei.Tests.Bdd.Institutions
{
    #region class ConnectionTestEi
    public class ConnectionTestEi : Institution<Institution.InstitutionState> {



        public ConnectionTestEi() : base("ConnectionTest") {
            // init basic properties
            this.Name = "Connection Test";
            this.Description = "Connection Test Description";

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
            this.AuthenticationPermissions.Add(
                new AuthorisationInfo("user", null, null, this.GroupByName("Citizen")),
                new AuthorisationInfo(null, "123", "Default", this.GroupByName("Citizen"))
           );
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
    public class DefaultOrganisation : Organisation {
        public DefaultOrganisation() : base("default") {
            this.Name = "Default";
        }

        public class DefaultResources : SearchableState {
            private List<object> defaultValues = new List<object>();

            public override SearchableState Clone(SearchableState cloneTo = null) {
                var clone = new DefaultResources();
                return clone;
            }

            public override bool Contains(string variable) {
                return false;
            }

            public override List<VariableInstance> FilterByAccess(Governor governor) {
                var list = new List<VariableInstance>();
                return list;
            }

            public override object GetValue(string name) {
                throw new Exception("Key does not exists: " + name);
            }

            public override void SetValue(string name, object value) {
                throw new Exception("Key does not exists: " + name);
            }

            public override ResourceState Merge(BaseState state) {
                return this;
            }

            public override ResourceState NewInstance() {
                return new DefaultResources();
            }

            public override void ResetDirty() {
            }

            public override GoalState[] ToGoalState() {

                return null;
            }
        }

        protected override SearchableState CreateState() {
            return new DefaultResources();
        }
    }
    #endregion

    #region class CitizenRole
    public class CitizenRole : Role {
        public CitizenRole() : base("1") {
            this.Name = "Citizen";
            this.Description = null;
        }

        public override SearchableState CreateState() {
            return new CitizenResources();
        }

        public class CitizenResources : SearchableState {
            public int ParentParameter { get; set; }

            // implementation

            private List<object> defaultValues = new List<object> {
                0
            };

            public override SearchableState Clone(SearchableState cloneTo = null) {
                var clone = new CitizenResources {
                    ParentParameter = this.ParentParameter
                };
                return clone;
            }

            private static List<string> Names = new List<string>() { "ParentParameter" };
            public override bool Contains(string variable) {
                return Names.Contains(variable);
            }

            public override List<VariableInstance> FilterByAccess(Governor governor) {
                var list = new List<VariableInstance>() {
                    new VariableInstance("ParentParameter", this.ParentParameter.ToString())
                };
                return list;
            }

            public override object GetValue(string name) {
                switch (name) {
                    case "ParentParameter":
                        return this.ParentParameter;
                    default:
                        throw new Exception("Key not found: " + name);
                }
            }

            public override void SetValue(string name, object value) {
                throw new Exception("Key does not exists: " + name);
            }

            public override ResourceState Merge(BaseState state) {

                var typedState = (CitizenResources)state;
                if (!typedState.ParentParameter.Equals(typedState.defaultValues[0])) {
                    this.ParentParameter = typedState.ParentParameter;
                }
                return this;
            }

            public override ResourceState NewInstance() {
                return new CitizenResources();
            }

            public override void ResetDirty() {
                this.defaultValues[0] = this.ParentParameter;
            }

            public override GoalState[] ToGoalState() {
                var list = new List<GoalState>();
                // we either return dirty ones or all of them
                if (!this.ParentParameter.Equals(this.defaultValues[0])) {
                    list.Add(new GoalState("ParentParameter", this.ParentParameter, StateGoalStrategy.Equal));
                }

                return list.ToArray();
            }
        }
    }
    #endregion

    #region AccessFactory
    static class AccessFactory {
        public static AccessCondition<Institution.InstitutionState, MainWorkflow.WorkflowState, DefaultOrganisation.DefaultResources, CitizenRole.CitizenResources, ParameterState> MainDefaultCitizen {
            get { return new AccessCondition<Institution.InstitutionState, MainWorkflow.WorkflowState, DefaultOrganisation.DefaultResources, CitizenRole.CitizenResources, ParameterState>(); }
        }
        public static AccessCondition<Institution.InstitutionState, SubWorkflow.Resources, DefaultOrganisation.DefaultResources, CitizenRole.CitizenResources, ParameterState> SubWorkflowDefaultCitizen {
            get { return new AccessCondition<Institution.InstitutionState, SubWorkflow.Resources, DefaultOrganisation.DefaultResources, CitizenRole.CitizenResources, ParameterState>(); }
        }
    }
    #endregion

    #region class MainWorkflow
    public class MainWorkflow : Workflow {
        // action parameters

        class SubWorkflowParameters : ParameterState {

            int Granite { get; set; }

            public override string Validate() {
                if (Granite < 0) {
                    return "Granite needs to be bigger then 0";
                }
                return null;
            }

            public override ParameterState Parse(VariableInstance[] properties) {
                foreach (var property in properties) {
                    switch (property.Name) {
                        case "Granite":
                            this.Granite = int.Parse(property.Value);
                            break;
                        default:
                            throw new Exception("Key not found: " + property.Name);
                    }
                }
                return this;
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
            // var startSubWorkflow = new ActionStartWorkflow("startSubWorkflow", ei, joinSubworkflow, new SubWorkflowParameters());

            this.AddActions(new ActionBase [] {
                joinSubworkflow,
                // startSubWorkflow,
                }
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
                        r.ParentParameter = 3;
                    })
                );

            // this.Connect(startState, startState, startSubWorkflow);
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

            public override ResourceState NewInstance() {
                return new Resources();
            }
        }
        private Resources resources = new Resources();
        #endregion

        class SendActionParameters : ParameterState {
            public int Stones { get; set; }
            public int Weight { get; set; }

            public override ParameterState Parse(VariableInstance[] properties) {
                foreach (var property in properties) {
                    switch (property.Name) {
                        case "Stones":
                            this.Stones = int.Parse(property.Value);
                            break;
                        case "Weight":
                            this.Weight = int.Parse(property.Value);
                            break;
                        default:
                            throw new Exception("Key not found: " + property.Name);
                    }
                }
                return this;
            }

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

            var sendAction = new ActionMessage("send", ei, new SendActionParameters(), ei.GroupsByName("Default", "Citizen"), null);
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

namespace R
{
}


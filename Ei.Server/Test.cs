//using Ei;
//using Ei.Core;
//using Ei.Core.Ontology;
//using Ei.Core.Ontology.Actions;
//using Ei.Core.Ontology.Transitions;
//using Ei.Core.Runtime;
//using Ei.Core.Runtime.Planning;
//using System;
//using System.Collections.Generic;

//#region class DefaultInstitution
//public class DefaultInstitution : Institution<DefaultInstitution.Store>
//{

//    #region class Store
//    public class Store : Institution.InstitutionState
//    {

//        public Store(Institution ei) : base(ei) { }
//    }
//    #endregion

//    // constructor 

//    public DefaultInstitution() : base("din")
//    {
//        this.Name = "DIN";
//        this.Description = "";

//        // init organisations 
//        this.AddOrganisations(new Organisation[] {
//            new DefaultOrganisation(),
//        });


//        // init roles
//        this.AddRoles(new Role[] {
//            new CitizenRole(),
//            new SlaveRole(),
//        });

//        // init workflows
//        this.AddWorkflows(new Workflow[] {
//            new MainWorkflow(this),
//        });

//        // set main workfloe
//        if (this.Workflows == null || this.Workflows.Count == 0)
//        {
//            throw new Exception("You need to define at least one workflow!");
//        }
//        this.MainWorkflowId = this.Workflows[0].Id;

//        // init security

//        this.AuthenticationPermissions.Add(new AuthorisationInfo[] {
//        });


//        // init expressions

//    }

//    // abstract implementation

//    public override Institution Instance => new DefaultInstitution();

//    public override Institution.InstitutionState Resources => new DefaultInstitution.Store(this);
//}
//#endregion

//#region class CitizenRole
//public class CitizenRole : Role
//{


//    #region class Store
//    public class Store : SearchableState
//    {
//        // Fields

//        private List<object> defaultValues = new List<object>() {
//            0,
//            0,
//        };

//        // Properties

//        public int Age { get; set; }
//        public int Food { get; set; }

//        // Ctor
//        public Store()
//        {
//            this.Age = 0;
//            this.Food = 0;
//        }

//        // Methods

//        public override SearchableState Clone(SearchableState cloneTo = null)
//        {
//            var clone = cloneTo == null
//                ? new CitizenRole.Store()
//                : (CitizenRole.Store)cloneTo;

//            clone.Age = this.Age;
//            clone.Food = this.Food;

//            return clone;
//        }

//        public override bool Contains(string name)
//        {
//            if (name == "Age") return true;
//            if (name == "Food") return true;

//            return false;
//        }

//        public override List<VariableInstance> FilterByAccess(Governor governor)
//        {
//            return new List<VariableInstance> {
//                new VariableInstance("Age", this.Age.ToString()),
//                new VariableInstance("Food", this.Food.ToString()),
//            };
//        }

//        public override object GetValue(string name)
//        {
//            if (name == "Age") return this.Age;
//            if (name == "Food") return this.Food;

//            throw new Exception("Key does not exists: " + name);
//        }

//        public override void SetValue(string name, object value)
//        {
//            if (name == "Age") { this.Age = (int)value; return; }
//            if (name == "Food") { this.Food = (int)value; return; }

//            throw new Exception("Key does not exists: " + name);
//        }

//        public override void ResetDirty()
//        {
//            this.defaultValues[0] = this.Age;
//            this.defaultValues[1] = this.Food;
//        }

//        public override GoalState[] ToGoalState()
//        {
//            var list = new List<GoalState>();
//            if (!this.Age.Equals(this.defaultValues[0]))
//            {
//                list.Add(new GoalState("Age", this.Age, StateGoalStrategy.Equal));
//            }
//            if (!this.Food.Equals(this.defaultValues[1]))
//            {
//                list.Add(new GoalState("Food", this.Food, StateGoalStrategy.Equal));
//            }

//            return list.ToArray();
//        }
//    }
//    #endregion


//    public CitizenRole() : base("default")
//    {
//        this.Name = "Citizen";
//    }

//    public CitizenRole(string id) : base(id) { }

//    public override SearchableState CreateState()
//    {
//        return new CitizenRole.Store();
//    }
//}
//#endregion
//#region class SlaveRole
//public class SlaveRole : Role
//{


//    #region class Store
//    public class Store : SearchableState
//    {
//        // Fields

//        private List<object> defaultValues = new List<object>()
//        {
//        };

//        // Properties


//        // Ctor
//        public Store()
//        {
//        }

//        // Methods

//        public override SearchableState Clone(SearchableState cloneTo = null)
//        {
//            var clone = cloneTo == null
//                ? new SlaveRole.Store()
//                : (SlaveRole.Store)cloneTo;


//            return clone;
//        }

//        public override bool Contains(string name)
//        {

//            return false;
//        }

//        public override List<VariableInstance> FilterByAccess(Governor governor)
//        {
//            return new List<VariableInstance>
//            {
//            };
//        }

//        public override object GetValue(string name)
//        {

//            throw new Exception("Key does not exists: " + name);
//        }

//        public override void SetValue(string name, object value)
//        {

//            throw new Exception("Key does not exists: " + name);
//        }

//        public override void ResetDirty()
//        {
//        }

//        public override GoalState[] ToGoalState()
//        {
//            var list = new List<GoalState>();

//            return list.ToArray();
//        }
//    }
//    #endregion


//    public SlaveRole() : base("Slave")
//    {
//        this.Name = "Slave";
//    }

//    public SlaveRole(string id) : base(id) { }

//    public override SearchableState CreateState()
//    {
//        return new SlaveRole.Store();
//    }
//}
//#endregion
//#region class DefaultOrganisation
//public class DefaultOrganisation : Organisation
//{


//    #region class Store
//    public class Store : SearchableState
//    {
//        // Fields

//        private List<object> defaultValues = new List<object>()
//        {
//        };

//        // Properties


//        // Ctor
//        public Store()
//        {
//        }

//        // Methods

//        public override SearchableState Clone(SearchableState cloneTo = null)
//        {
//            var clone = cloneTo == null
//                ? new DefaultOrganisation.Store()
//                : (DefaultOrganisation.Store)cloneTo;


//            return clone;
//        }

//        public override bool Contains(string name)
//        {

//            return false;
//        }

//        public override List<VariableInstance> FilterByAccess(Governor governor)
//        {
//            return new List<VariableInstance>
//            {
//            };
//        }

//        public override object GetValue(string name)
//        {

//            throw new Exception("Key does not exists: " + name);
//        }

//        public override void SetValue(string name, object value)
//        {

//            throw new Exception("Key does not exists: " + name);
//        }

//        public override void ResetDirty()
//        {
//        }

//        public override GoalState[] ToGoalState()
//        {
//            var list = new List<GoalState>();

//            return list.ToArray();
//        }
//    }
//    #endregion


//    public DefaultOrganisation() : base("default")
//    {
//        this.Name = "Default";
//    }

//    public DefaultOrganisation(string id) : base(id) { }

//    protected override SearchableState CreateState()
//    {
//        return new DefaultOrganisation.Store();
//    }
//}
//#endregion
//#region class MainWorkflow
//public class MainWorkflow : Workflow
//{

//    // store
//    #region Store
//    public new class Store : Workflow.Store
//    {

//        public override Workflow.Store Clone(Workflow.Store store = null)
//        {
//            var current = new MainWorkflow.Store();

//            // clone parent properties
//            base.Clone(current);

//            // add workflow properties

//            return current;
//        }
//    }
//    #endregion

//    // workflow actions

//    #region Action Parameters

//    #region FeedActionParameters
//    public class FeedActionParameters : ParameterState
//    {

//        // Properties


//        // Abstract implementation

//        public override string Validate()
//        {
//            var result = base.Validate();
//            if (result != null)
//            {
//                return result;
//            }

//            return null;
//        }

//        public override void Parse(VariableInstance[] properties)
//        {
//            base.Parse(properties);
//            foreach (var property in properties)
//            {
//                switch (property.Name)
//                {
//                    default:
//                        throw new Exception("Parameter does not exist: " + property.Name);
//                }
//            }
//        }

//    }
//    #endregion
//    #region PoopActionParameters
//    public class PoopActionParameters : ParameterState
//    {

//        // Properties


//        // Abstract implementation

//        public override string Validate()
//        {
//            var result = base.Validate();
//            if (result != null)
//            {
//                return result;
//            }

//            return null;
//        }

//        public override void Parse(VariableInstance[] properties)
//        {
//            base.Parse(properties);
//            foreach (var property in properties)
//            {
//                switch (property.Name)
//                {
//                    default:
//                        throw new Exception("Parameter does not exist: " + property.Name);
//                }
//            }
//        }

//    }
//    #endregion
//    #region EmptyActionParameters
//    public class EmptyActionParameters : ParameterState
//    {

//        // Properties


//        // Abstract implementation

//        public override string Validate()
//        {
//            var result = base.Validate();
//            if (result != null)
//            {
//                return result;
//            }

//            return null;
//        }

//        public override void Parse(VariableInstance[] properties)
//        {
//            base.Parse(properties);
//            foreach (var property in properties)
//            {
//                switch (property.Name)
//                {
//                    default:
//                        throw new Exception("Parameter does not exist: " + property.Name);
//                }
//            }
//        }

//    }
//    #endregion   
//    #endregion

//    // abstract implementation

//    public override bool Stateless => true;
//    public override bool Static => true;
//    public override Workflow.Store CreateStore()
//    {
//        return new MainWorkflow.Store();
//    }

//    // constructor

//    public MainWorkflow() : this(null) { }

//    public MainWorkflow(Institution ei) : base(ei, "main")
//    {
//        this.Name = "Main";
//        this.Description = "";

//        // actions
//        this.AddActions(new ActionBase[] {
//            new ActionMessage("Feed", ei, () => new MainWorkflow.FeedActionParameters(), null, null),
//            new ActionMessage("Poop", ei, () => new MainWorkflow.PoopActionParameters(), null, null),
//            new ActionTimeout("Empty", ei),
//        });

//        // states

//        this.AddStates(new State[] {
//            new State("start", "Start", "", this, false, 0, true, true),
//            new State("End", "End", "", this, false, 0, false, true),
//            new State("Check", "Check", "", this, false, 0, false, false),
//        });

//        // transitions

//        this.AddTransitions(new Transition[] {
//        });

//        // connections
//        this.Connect("c0", this.GetPosition("start"), this.GetPosition("Check"), this.GetAction("Feed"), 0)
//              .Condition(new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, DefaultOrganisation.Store, CitizenRole.Store, FeedActionParameters>()
//                  .Action((i, w, g, o, r, a) => { r.Food += 1; }));

//        this.Connect("c1", this.GetPosition("Check"), this.GetPosition("start"), this.GetAction("Empty"), 0)
//              .Condition(new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, DefaultOrganisation.Store, CitizenRole.Store, EmptyActionParameters>()
//                  .Allow((i, w, g, o, r, a) => { return r.Age < 10; })
//                  .Action((i, w, g, o, r, a) => { return r.Food = 0; }, (i, w, g, o, r, a) => {; }));

//        this.Connect("c2", this.GetPosition("Check"), this.GetPosition("End"), this.GetAction(""), 0)
//              .Condition(new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, DefaultOrganisation.Store, CitizenRole.Store, ParameterState>()
//                  .Allow((i, w, g, o, r, a) => { return r.Age >= 10; }));


//        // create permissions

//        // entry permissions

//        // IMPORTANT: this needs to be called to initialise connections

//        this.Init();
//    }
//}
//#endregion
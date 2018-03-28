using Ei;
using Ei.Ontology;
using Ei.Ontology.Actions;
using Ei.Ontology.Transitions;
using Ei.Runtime;
using Ei.Runtime.Planning;
using System;
using System.Collections.Generic;

#region class DefaultInstitution
public class DefaultInstitution : Institution<DefaultInstitution.Store>
{

    #region class Store
    public class Store : Institution.InstitutionState
    {
        public float Circadian { get; set; }
        public float SimulatedTime { get; set; }
        public float Tick { get; set; }

        public Store(Institution ei) : base(ei) {}
    }
    #endregion

    // constructor 

    public DefaultInstitution() : base("uruk") {
        this.Name = "Uruk";
        this.Description = "";

        // init organisations 
        this.AddOrganisations(new Organisation[] {
            new TribeOrganisation(),
        });
        

        // init roles
        this.AddRoles(new Role[] {
            new HumanRole(),
            new PhysiologyRole(),
            new BakerRole(),
            new ShepherdRole(),
            new PotterRole(),
            new FisherRole(),
        });

        // init workflows
        this.AddWorkflows(new Workflow[] {
            new MainWorkflow(this),
            new BakerWorkflow(this),
            new ExchangeWorkflow(this),
            new FishingWorkflow(this),
            new PotterWorkflow(this),
            new PhysiologyWorkflow(this),
            new ShepherdWorkflow(this),
        });
        
        // set main workfloe
        if (this.Workflows == null || this.Workflows.Count == 0) {
            throw new Exception("You need to define at least one workflow!");
        }
        this.MainWorkflowId = this.Workflows[0].Id;

        // init security

        this.AuthenticationPermissions.Add(new AuthorisationInfo[] {
            new AuthorisationInfo(null, null, "default", this.GroupById("default", "fisher"), this.GroupById("default", "potter"), this.GroupById("default", "baker"), this.GroupById("default", "shepherd")),
        });
        

        // init expressions
        this.AddExpressions(i => { i.SimulatedTime = i.TimeSeconds * i.Tick;
i.Circadian = (float) Math.Sqrt(Math.Cos((i.SimulatedTime * 3.1415926) / 21600) * Math.Cos((i.SimulatedTime * 3.1415926) / 21600));; });
        
    }

    // abstract implementation

    public override Institution Instance => new DefaultInstitution();

    public override Institution.InstitutionState Resources => new DefaultInstitution.Store(this);
}
#endregion

#region class HumanRole
public class HumanRole: Role {

    
        #region class Store
    public class Store : SearchableState
    {
        // Fields

        private List<object> defaultValues = new List<object>() {
            0,
            0,
            0,
            3,
            0,
            0,
        };

        // Properties

        public int Bread { get; set; }
        public int Fish { get; set; }
        public int Milk { get; set; }
        public int Pots { get; set; }
        public int Water { get; set; }
        public int Wood { get; set; }
       
        // Ctor
        public Store() {
            this.Bread = 0;
            this.Fish = 0;
            this.Milk = 0;
            this.Pots = 3;
            this.Water = 0;
            this.Wood = 0;
        } 

        // Methods

        public override SearchableState Clone(SearchableState cloneTo = null) {
            var clone = cloneTo == null 
                ? new HumanRole.Store()
                : (HumanRole.Store) cloneTo;

            clone.Bread = this.Bread;
            clone.Fish = this.Fish;
            clone.Milk = this.Milk;
            clone.Pots = this.Pots;
            clone.Water = this.Water;
            clone.Wood = this.Wood;

            return clone;
        }

        public override bool Contains(string name) {
            if (name == "Bread") return true;
            if (name == "Fish") return true;
            if (name == "Milk") return true;
            if (name == "Pots") return true;
            if (name == "Water") return true;
            if (name == "Wood") return true;
    
            return false; 
        }

        public override List<VariableInstance> FilterByAccess(Governor governor) {
            return new List<VariableInstance> {
                new VariableInstance("Bread", this.Bread.ToString()),
                new VariableInstance("Fish", this.Fish.ToString()),
                new VariableInstance("Milk", this.Milk.ToString()),
                new VariableInstance("Pots", this.Pots.ToString()),
                new VariableInstance("Water", this.Water.ToString()),
                new VariableInstance("Wood", this.Wood.ToString()),
            };
        }

        public override object GetValue(string name) {
            if (name == "Bread") return this.Bread;
            if (name == "Fish") return this.Fish;
            if (name == "Milk") return this.Milk;
            if (name == "Pots") return this.Pots;
            if (name == "Water") return this.Water;
            if (name == "Wood") return this.Wood;

            throw new Exception("Key does not exists: " + name);
        }

        public override void SetValue(string name, object value) {
            if (name == "Bread") { this.Bread = (int) value; return; }
            if (name == "Fish") { this.Fish = (int) value; return; }
            if (name == "Milk") { this.Milk = (int) value; return; }
            if (name == "Pots") { this.Pots = (int) value; return; }
            if (name == "Water") { this.Water = (int) value; return; }
            if (name == "Wood") { this.Wood = (int) value; return; }

            throw new Exception("Key does not exists: " + name);
        }

        public override void ResetDirty() {
            this.defaultValues[0] = this.Bread;
            this.defaultValues[1] = this.Fish;
            this.defaultValues[2] = this.Milk;
            this.defaultValues[3] = this.Pots;
            this.defaultValues[4] = this.Water;
            this.defaultValues[5] = this.Wood;
        }

        public override GoalState[] ToGoalState() {
            var list = new List<GoalState>();
            if (!this.Bread.Equals(this.defaultValues[0])) {
                list.Add(new GoalState("Bread", this.Bread, StateGoalStrategy.Equal));
            }
            if (!this.Fish.Equals(this.defaultValues[1])) {
                list.Add(new GoalState("Fish", this.Fish, StateGoalStrategy.Equal));
            }
            if (!this.Milk.Equals(this.defaultValues[2])) {
                list.Add(new GoalState("Milk", this.Milk, StateGoalStrategy.Equal));
            }
            if (!this.Pots.Equals(this.defaultValues[3])) {
                list.Add(new GoalState("Pots", this.Pots, StateGoalStrategy.Equal));
            }
            if (!this.Water.Equals(this.defaultValues[4])) {
                list.Add(new GoalState("Water", this.Water, StateGoalStrategy.Equal));
            }
            if (!this.Wood.Equals(this.defaultValues[5])) {
                list.Add(new GoalState("Wood", this.Wood, StateGoalStrategy.Equal));
            }
            
            return list.ToArray();
        }
    }
    #endregion


    public HumanRole() : base("default") {
        this.Name = "Human";
    }

    public HumanRole(string id) : base(id) { }

    public override SearchableState CreateState() {
        return new HumanRole.Store();
    }
}
#endregion
#region class PhysiologyRole
public class PhysiologyRole: HumanRole {

        #region class Store
    public new class Store : HumanRole.Store
    {
        // Fields
        private List<object> defaultValues = new List<object>() {
            0f,
            0f,
            1f,
            0f,
            343.33f,
            1f,
            0f,
            0f,
            0f,
            0.1333f,
            1f,
            0f,
            0f,
        };

        // Properties

        public float Fatigue { get; set; }
        public float FatigueDecay { get; set; }
        public float FatigueModifier { get; set; }
        public float HungerDecay { get; set; }
        public float HungerDecayVariable { get; set; }
        public float HungerModifier { get; set; }
        public float HungerReservoir { get; set; }
        public float Thirst { get; set; }
        public float ThirstDecay { get; set; }
        public float ThirstDecayVariable { get; set; }
        public float ThirstModifier { get; set; }
        public float ThirstReservoir { get; set; }
        public float Hunger { get; set; }
       
        // Ctor
        public Store() {
            this.Fatigue = 0f;
            this.FatigueDecay = 0f;
            this.FatigueModifier = 1f;
            this.HungerDecay = 0f;
            this.HungerDecayVariable = 343.33f;
            this.HungerModifier = 1f;
            this.HungerReservoir = 0f;
            this.Thirst = 0f;
            this.ThirstDecay = 0f;
            this.ThirstDecayVariable = 0.1333f;
            this.ThirstModifier = 1f;
            this.ThirstReservoir = 0f;
            this.Hunger = 0f;
        } 

        // Methods

        public override SearchableState Clone(SearchableState cloneTo = null) {
            var clone = cloneTo == null ? new Store() : (Store) cloneTo;
            base.Clone(clone);
            
            clone.Fatigue = this.Fatigue;
            clone.FatigueDecay = this.FatigueDecay;
            clone.FatigueModifier = this.FatigueModifier;
            clone.HungerDecay = this.HungerDecay;
            clone.HungerDecayVariable = this.HungerDecayVariable;
            clone.HungerModifier = this.HungerModifier;
            clone.HungerReservoir = this.HungerReservoir;
            clone.Thirst = this.Thirst;
            clone.ThirstDecay = this.ThirstDecay;
            clone.ThirstDecayVariable = this.ThirstDecayVariable;
            clone.ThirstModifier = this.ThirstModifier;
            clone.ThirstReservoir = this.ThirstReservoir;
            clone.Hunger = this.Hunger;

            return clone;
        }

        public override bool Contains(string name) {
            if (base.Contains(name)) return true;
            if (name == "Fatigue") return true;
            if (name == "FatigueDecay") return true;
            if (name == "FatigueModifier") return true;
            if (name == "HungerDecay") return true;
            if (name == "HungerDecayVariable") return true;
            if (name == "HungerModifier") return true;
            if (name == "HungerReservoir") return true;
            if (name == "Thirst") return true;
            if (name == "ThirstDecay") return true;
            if (name == "ThirstDecayVariable") return true;
            if (name == "ThirstModifier") return true;
            if (name == "ThirstReservoir") return true;
            if (name == "Hunger") return true;
    
            return false; 
        }

        public override List<VariableInstance> FilterByAccess(Governor governor) {
            var list = base.FilterByAccess(governor);
            list.AddRange(new List<VariableInstance> {
                new VariableInstance("Fatigue", this.Fatigue.ToString()),
                new VariableInstance("FatigueDecay", this.FatigueDecay.ToString()),
                new VariableInstance("FatigueModifier", this.FatigueModifier.ToString()),
                new VariableInstance("HungerDecay", this.HungerDecay.ToString()),
                new VariableInstance("HungerDecayVariable", this.HungerDecayVariable.ToString()),
                new VariableInstance("HungerModifier", this.HungerModifier.ToString()),
                new VariableInstance("HungerReservoir", this.HungerReservoir.ToString()),
                new VariableInstance("Thirst", this.Thirst.ToString()),
                new VariableInstance("ThirstDecay", this.ThirstDecay.ToString()),
                new VariableInstance("ThirstDecayVariable", this.ThirstDecayVariable.ToString()),
                new VariableInstance("ThirstModifier", this.ThirstModifier.ToString()),
                new VariableInstance("ThirstReservoir", this.ThirstReservoir.ToString()),
                new VariableInstance("Hunger", this.Hunger.ToString()),
            });
            return list;
        }

        public override object GetValue(string name) {
            if (base.Contains(name)) {
                return base.GetValue(name);
            } 
            if (name == "Fatigue") return this.Fatigue;
            if (name == "FatigueDecay") return this.FatigueDecay;
            if (name == "FatigueModifier") return this.FatigueModifier;
            if (name == "HungerDecay") return this.HungerDecay;
            if (name == "HungerDecayVariable") return this.HungerDecayVariable;
            if (name == "HungerModifier") return this.HungerModifier;
            if (name == "HungerReservoir") return this.HungerReservoir;
            if (name == "Thirst") return this.Thirst;
            if (name == "ThirstDecay") return this.ThirstDecay;
            if (name == "ThirstDecayVariable") return this.ThirstDecayVariable;
            if (name == "ThirstModifier") return this.ThirstModifier;
            if (name == "ThirstReservoir") return this.ThirstReservoir;
            if (name == "Hunger") return this.Hunger;

            throw new Exception("Key does not exists: " + name);
        }

        public override void SetValue(string name, object value) {
            if (base.Contains(name)) {
                base.SetValue(name, value);
                return;
            }
            if (name == "Fatigue") { this.Fatigue = (float) value; return; }
            if (name == "FatigueDecay") { this.FatigueDecay = (float) value; return; }
            if (name == "FatigueModifier") { this.FatigueModifier = (float) value; return; }
            if (name == "HungerDecay") { this.HungerDecay = (float) value; return; }
            if (name == "HungerDecayVariable") { this.HungerDecayVariable = (float) value; return; }
            if (name == "HungerModifier") { this.HungerModifier = (float) value; return; }
            if (name == "HungerReservoir") { this.HungerReservoir = (float) value; return; }
            if (name == "Thirst") { this.Thirst = (float) value; return; }
            if (name == "ThirstDecay") { this.ThirstDecay = (float) value; return; }
            if (name == "ThirstDecayVariable") { this.ThirstDecayVariable = (float) value; return; }
            if (name == "ThirstModifier") { this.ThirstModifier = (float) value; return; }
            if (name == "ThirstReservoir") { this.ThirstReservoir = (float) value; return; }
            if (name == "Hunger") { this.Hunger = (float) value; return; }

            throw new Exception("Key does not exists: " + name);
        }

        public override void ResetDirty() {
            base.ResetDirty();

            this.defaultValues[0] = this.Fatigue;
            this.defaultValues[1] = this.FatigueDecay;
            this.defaultValues[2] = this.FatigueModifier;
            this.defaultValues[3] = this.HungerDecay;
            this.defaultValues[4] = this.HungerDecayVariable;
            this.defaultValues[5] = this.HungerModifier;
            this.defaultValues[6] = this.HungerReservoir;
            this.defaultValues[7] = this.Thirst;
            this.defaultValues[8] = this.ThirstDecay;
            this.defaultValues[9] = this.ThirstDecayVariable;
            this.defaultValues[10] = this.ThirstModifier;
            this.defaultValues[11] = this.ThirstReservoir;
            this.defaultValues[12] = this.Hunger;
        }

        public override GoalState[] ToGoalState() {
            var list = new List<GoalState>();
            list.AddRange(base.ToGoalState());

            if (!this.Fatigue.Equals(this.defaultValues[0])) {
                list.Add(new GoalState("Fatigue", this.Fatigue, StateGoalStrategy.Equal));
            }
            if (!this.FatigueDecay.Equals(this.defaultValues[1])) {
                list.Add(new GoalState("FatigueDecay", this.FatigueDecay, StateGoalStrategy.Equal));
            }
            if (!this.FatigueModifier.Equals(this.defaultValues[2])) {
                list.Add(new GoalState("FatigueModifier", this.FatigueModifier, StateGoalStrategy.Equal));
            }
            if (!this.HungerDecay.Equals(this.defaultValues[3])) {
                list.Add(new GoalState("HungerDecay", this.HungerDecay, StateGoalStrategy.Equal));
            }
            if (!this.HungerDecayVariable.Equals(this.defaultValues[4])) {
                list.Add(new GoalState("HungerDecayVariable", this.HungerDecayVariable, StateGoalStrategy.Equal));
            }
            if (!this.HungerModifier.Equals(this.defaultValues[5])) {
                list.Add(new GoalState("HungerModifier", this.HungerModifier, StateGoalStrategy.Equal));
            }
            if (!this.HungerReservoir.Equals(this.defaultValues[6])) {
                list.Add(new GoalState("HungerReservoir", this.HungerReservoir, StateGoalStrategy.Equal));
            }
            if (!this.Thirst.Equals(this.defaultValues[7])) {
                list.Add(new GoalState("Thirst", this.Thirst, StateGoalStrategy.Equal));
            }
            if (!this.ThirstDecay.Equals(this.defaultValues[8])) {
                list.Add(new GoalState("ThirstDecay", this.ThirstDecay, StateGoalStrategy.Equal));
            }
            if (!this.ThirstDecayVariable.Equals(this.defaultValues[9])) {
                list.Add(new GoalState("ThirstDecayVariable", this.ThirstDecayVariable, StateGoalStrategy.Equal));
            }
            if (!this.ThirstModifier.Equals(this.defaultValues[10])) {
                list.Add(new GoalState("ThirstModifier", this.ThirstModifier, StateGoalStrategy.Equal));
            }
            if (!this.ThirstReservoir.Equals(this.defaultValues[11])) {
                list.Add(new GoalState("ThirstReservoir", this.ThirstReservoir, StateGoalStrategy.Equal));
            }
            if (!this.Hunger.Equals(this.defaultValues[12])) {
                list.Add(new GoalState("Hunger", this.Hunger, StateGoalStrategy.Equal));
            }
            
            return list.ToArray();
        }
    }
    #endregion

    

    public PhysiologyRole() : base("Physiology") {
        this.Name = "Physiology";
    }

    public PhysiologyRole(string id) : base(id) { }

    public override SearchableState CreateState() {
        return new PhysiologyRole.Store();
    }
}
#endregion
#region class BakerRole
public class BakerRole: PhysiologyRole {

        #region class Store
    public new class Store : PhysiologyRole.Store
    {
        // Fields
        private List<object> defaultValues = new List<object>() {
            0,
        };

        // Properties

        public int Wheat { get; set; }
       
        // Ctor
        public Store() {
            this.Wheat = 0;
        } 

        // Methods

        public override SearchableState Clone(SearchableState cloneTo = null) {
            var clone = cloneTo == null ? new Store() : (Store) cloneTo;
            base.Clone(clone);
            
            clone.Wheat = this.Wheat;

            return clone;
        }

        public override bool Contains(string name) {
            if (base.Contains(name)) return true;
            if (name == "Wheat") return true;
    
            return false; 
        }

        public override List<VariableInstance> FilterByAccess(Governor governor) {
            var list = base.FilterByAccess(governor);
            list.AddRange(new List<VariableInstance> {
                new VariableInstance("Wheat", this.Wheat.ToString()),
            });
            return list;
        }

        public override object GetValue(string name) {
            if (base.Contains(name)) {
                return base.GetValue(name);
            } 
            if (name == "Wheat") return this.Wheat;

            throw new Exception("Key does not exists: " + name);
        }

        public override void SetValue(string name, object value) {
            if (base.Contains(name)) {
                base.SetValue(name, value);
                return;
            }
            if (name == "Wheat") { this.Wheat = (int) value; return; }

            throw new Exception("Key does not exists: " + name);
        }

        public override void ResetDirty() {
            base.ResetDirty();

            this.defaultValues[0] = this.Wheat;
        }

        public override GoalState[] ToGoalState() {
            var list = new List<GoalState>();
            list.AddRange(base.ToGoalState());

            if (!this.Wheat.Equals(this.defaultValues[0])) {
                list.Add(new GoalState("Wheat", this.Wheat, StateGoalStrategy.Equal));
            }
            
            return list.ToArray();
        }
    }
    #endregion

    

    public BakerRole() : base("baker") {
        this.Name = "Baker";
    }

    public BakerRole(string id) : base(id) { }

    public override SearchableState CreateState() {
        return new BakerRole.Store();
    }
}
#endregion
#region class ShepherdRole
public class ShepherdRole: PhysiologyRole {

        #region class Store
    public new class Store : PhysiologyRole.Store
    {
        // Fields
        private List<object> defaultValues = new List<object>() {
        };

        // Properties

       
        // Ctor
        public Store() {
        } 

        // Methods

        public override SearchableState Clone(SearchableState cloneTo = null) {
            var clone = cloneTo == null ? new Store() : (Store) cloneTo;
            base.Clone(clone);
            

            return clone;
        }

        public override bool Contains(string name) {
            if (base.Contains(name)) return true;
    
            return false; 
        }

        public override List<VariableInstance> FilterByAccess(Governor governor) {
            var list = base.FilterByAccess(governor);
            list.AddRange(new List<VariableInstance> {
            });
            return list;
        }

        public override object GetValue(string name) {
            if (base.Contains(name)) {
                return base.GetValue(name);
            } 

            throw new Exception("Key does not exists: " + name);
        }

        public override void SetValue(string name, object value) {
            if (base.Contains(name)) {
                base.SetValue(name, value);
                return;
            }

            throw new Exception("Key does not exists: " + name);
        }

        public override void ResetDirty() {
            base.ResetDirty();

        }

        public override GoalState[] ToGoalState() {
            var list = new List<GoalState>();
            list.AddRange(base.ToGoalState());

            
            return list.ToArray();
        }
    }
    #endregion

    

    public ShepherdRole() : base("shepherd") {
        this.Name = "Shepherd";
    }

    public ShepherdRole(string id) : base(id) { }

    public override SearchableState CreateState() {
        return new ShepherdRole.Store();
    }
}
#endregion
#region class PotterRole
public class PotterRole: PhysiologyRole {

        #region class Store
    public new class Store : PhysiologyRole.Store
    {
        // Fields
        private List<object> defaultValues = new List<object>() {
            0,
            -1,
        };

        // Properties

        public int Clay { get; set; }
        public int ExchangeId { get; set; }
       
        // Ctor
        public Store() {
            this.Clay = 0;
            this.ExchangeId = -1;
        } 

        // Methods

        public override SearchableState Clone(SearchableState cloneTo = null) {
            var clone = cloneTo == null ? new Store() : (Store) cloneTo;
            base.Clone(clone);
            
            clone.Clay = this.Clay;
            clone.ExchangeId = this.ExchangeId;

            return clone;
        }

        public override bool Contains(string name) {
            if (base.Contains(name)) return true;
            if (name == "Clay") return true;
            if (name == "ExchangeId") return true;
    
            return false; 
        }

        public override List<VariableInstance> FilterByAccess(Governor governor) {
            var list = base.FilterByAccess(governor);
            list.AddRange(new List<VariableInstance> {
                new VariableInstance("Clay", this.Clay.ToString()),
                new VariableInstance("ExchangeId", this.ExchangeId.ToString()),
            });
            return list;
        }

        public override object GetValue(string name) {
            if (base.Contains(name)) {
                return base.GetValue(name);
            } 
            if (name == "Clay") return this.Clay;
            if (name == "ExchangeId") return this.ExchangeId;

            throw new Exception("Key does not exists: " + name);
        }

        public override void SetValue(string name, object value) {
            if (base.Contains(name)) {
                base.SetValue(name, value);
                return;
            }
            if (name == "Clay") { this.Clay = (int) value; return; }
            if (name == "ExchangeId") { this.ExchangeId = (int) value; return; }

            throw new Exception("Key does not exists: " + name);
        }

        public override void ResetDirty() {
            base.ResetDirty();

            this.defaultValues[0] = this.Clay;
            this.defaultValues[1] = this.ExchangeId;
        }

        public override GoalState[] ToGoalState() {
            var list = new List<GoalState>();
            list.AddRange(base.ToGoalState());

            if (!this.Clay.Equals(this.defaultValues[0])) {
                list.Add(new GoalState("Clay", this.Clay, StateGoalStrategy.Equal));
            }
            if (!this.ExchangeId.Equals(this.defaultValues[1])) {
                list.Add(new GoalState("ExchangeId", this.ExchangeId, StateGoalStrategy.Equal));
            }
            
            return list.ToArray();
        }
    }
    #endregion

    

    public PotterRole() : base("potter") {
        this.Name = "Potter";
    }

    public PotterRole(string id) : base(id) { }

    public override SearchableState CreateState() {
        return new PotterRole.Store();
    }
}
#endregion
#region class FisherRole
public class FisherRole: PhysiologyRole {

        #region class Store
    public new class Store : PhysiologyRole.Store
    {
        // Fields
        private List<object> defaultValues = new List<object>() {
            0,
        };

        // Properties

        public int Spear { get; set; }
       
        // Ctor
        public Store() {
            this.Spear = 0;
        } 

        // Methods

        public override SearchableState Clone(SearchableState cloneTo = null) {
            var clone = cloneTo == null ? new Store() : (Store) cloneTo;
            base.Clone(clone);
            
            clone.Spear = this.Spear;

            return clone;
        }

        public override bool Contains(string name) {
            if (base.Contains(name)) return true;
            if (name == "Spear") return true;
    
            return false; 
        }

        public override List<VariableInstance> FilterByAccess(Governor governor) {
            var list = base.FilterByAccess(governor);
            list.AddRange(new List<VariableInstance> {
                new VariableInstance("Spear", this.Spear.ToString()),
            });
            return list;
        }

        public override object GetValue(string name) {
            if (base.Contains(name)) {
                return base.GetValue(name);
            } 
            if (name == "Spear") return this.Spear;

            throw new Exception("Key does not exists: " + name);
        }

        public override void SetValue(string name, object value) {
            if (base.Contains(name)) {
                base.SetValue(name, value);
                return;
            }
            if (name == "Spear") { this.Spear = (int) value; return; }

            throw new Exception("Key does not exists: " + name);
        }

        public override void ResetDirty() {
            base.ResetDirty();

            this.defaultValues[0] = this.Spear;
        }

        public override GoalState[] ToGoalState() {
            var list = new List<GoalState>();
            list.AddRange(base.ToGoalState());

            if (!this.Spear.Equals(this.defaultValues[0])) {
                list.Add(new GoalState("Spear", this.Spear, StateGoalStrategy.Equal));
            }
            
            return list.ToArray();
        }
    }
    #endregion

    

    public FisherRole() : base("fisher") {
        this.Name = "Fisher";
    }

    public FisherRole(string id) : base(id) { }

    public override SearchableState CreateState() {
        return new FisherRole.Store();
    }
}
#endregion
#region class TribeOrganisation
public class TribeOrganisation: Organisation {

    
        #region class Store
    public class Store : SearchableState
    {
        // Fields

        private List<object> defaultValues = new List<object>() {
        };

        // Properties

       
        // Ctor
        public Store() {
        } 

        // Methods

        public override SearchableState Clone(SearchableState cloneTo = null) {
            var clone = cloneTo == null 
                ? new TribeOrganisation.Store()
                : (TribeOrganisation.Store) cloneTo;


            return clone;
        }

        public override bool Contains(string name) {
    
            return false; 
        }

        public override List<VariableInstance> FilterByAccess(Governor governor) {
            return new List<VariableInstance> {
            };
        }

        public override object GetValue(string name) {

            throw new Exception("Key does not exists: " + name);
        }

        public override void SetValue(string name, object value) {

            throw new Exception("Key does not exists: " + name);
        }

        public override void ResetDirty() {
        }

        public override GoalState[] ToGoalState() {
            var list = new List<GoalState>();
            
            return list.ToArray();
        }
    }
    #endregion


    public TribeOrganisation() : base("default") {
        this.Name = "Tribe";
    }

    public TribeOrganisation(string id) : base(id) { }

    protected override SearchableState CreateState() {
        return new TribeOrganisation.Store();
    }
}
#endregion
#region class MainWorkflow
public class MainWorkflow : Workflow {

    // store
    #region Store
    public new class Store : Workflow.Store {

        public override Workflow.Store Clone(Workflow.Store store = null) {
            var current = new MainWorkflow.Store();

            // clone parent properties
            base.Clone(current);

            // add workflow properties

            return current;
}
    }
    #endregion

    // workflow actions

    #region Action Parameters
    
    #region PhysiologyworkflowActionParameters
    public class PhysiologyworkflowActionParameters: ActionJoinWorkflow.Parameters {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

        public override void CopyTo(Workflow.Store workflowStore) {
            var store = (PhysiologyWorkflow.Store) workflowStore;

        }
    }
    #endregion
    #region BakerWorkflowActionParameters
    public class BakerWorkflowActionParameters: ActionJoinWorkflow.Parameters {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

        public override void CopyTo(Workflow.Store workflowStore) {
            var store = (BakerWorkflow.Store) workflowStore;

        }
    }
    #endregion
    #region PotterWorkflowActionParameters
    public class PotterWorkflowActionParameters: ActionJoinWorkflow.Parameters {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

        public override void CopyTo(Workflow.Store workflowStore) {
            var store = (PotterWorkflow.Store) workflowStore;

        }
    }
    #endregion
    #region ExchangeWorkflowActionParameters
    public class ExchangeWorkflowActionParameters: ActionJoinWorkflow.Parameters {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

        public override void CopyTo(Workflow.Store workflowStore) {
            var store = (ExchangeWorkflow.Store) workflowStore;

        }
    }
    #endregion
    #region ShepherdWorkflowActionParameters
    public class ShepherdWorkflowActionParameters: ActionJoinWorkflow.Parameters {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

        public override void CopyTo(Workflow.Store workflowStore) {
            var store = (ShepherdWorkflow.Store) workflowStore;

        }
    }
    #endregion
    #region FisherWorkflowActionParameters
    public class FisherWorkflowActionParameters: ActionJoinWorkflow.Parameters {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

        public override void CopyTo(Workflow.Store workflowStore) {
            var store = (FishingWorkflow.Store) workflowStore;

        }
    }
    #endregion
    #region DrinkActionParameters
    public class DrinkActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion
    #region DrinkMilkActionParameters
    public class DrinkMilkActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion
    #region EatFishActionParameters
    public class EatFishActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion
    #region EatBreadActionParameters
    public class EatBreadActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion
    #region InitAgentActionParameters
    public class InitAgentActionParameters: ParameterState {
        
        // Properties

        public float FatigueModifier { get; set; }
        public float HungerModifier { get; set; }
        public float ThirstModifier { get; set; }
        public float Tick { get; set; }

        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                    case "FatigueModifier":
                        this.FatigueModifier = float.Parse(property.Value);
                        break;
                    case "HungerModifier":
                        this.HungerModifier = float.Parse(property.Value);
                        break;
                    case "ThirstModifier":
                        this.ThirstModifier = float.Parse(property.Value);
                        break;
                    case "Tick":
                        this.Tick = float.Parse(property.Value);
                        break;
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion   
    #endregion

    // abstract implementation

    public override bool Stateless => true;
    public override bool Static => true;
    public override Workflow.Store CreateStore() {
        return new MainWorkflow.Store();
    }

    // constructor

    public MainWorkflow(): this(null) { }

    public MainWorkflow(Institution ei) : base(ei, "main") {
        this.Name = "Main";
        this.Description = "";

        // actions
        this.AddActions(new ActionBase [] {
            new ActionJoinWorkflow("physiologyworkflow", ei, "PhysiologyWorkflow", () => new MainWorkflow.PhysiologyworkflowActionParameters()),
            new ActionJoinWorkflow("bakerWorkflow", ei, "BakerWorkflow", () => new MainWorkflow.BakerWorkflowActionParameters()),
            new ActionJoinWorkflow("potterWorkflow", ei, "PotterWorkflow", () => new MainWorkflow.PotterWorkflowActionParameters()),
            new ActionJoinWorkflow("exchangeWorkflow", ei, "ExchangeWorkflow", () => new MainWorkflow.ExchangeWorkflowActionParameters()),
            new ActionJoinWorkflow("shepherdWorkflow", ei, "ShepherdWorkflow", () => new MainWorkflow.ShepherdWorkflowActionParameters()),
            new ActionJoinWorkflow("fisherWorkflow", ei, "FisherWorkflow", () => new MainWorkflow.FisherWorkflowActionParameters()),
            new ActionMessage("drink", ei, () => new MainWorkflow.DrinkActionParameters(), null, null),
            new ActionMessage("drinkMilk", ei, () => new MainWorkflow.DrinkMilkActionParameters(), null, null),
            new ActionMessage("eatFish", ei, () => new MainWorkflow.EatFishActionParameters(), null, null),
            new ActionMessage("eatBread", ei, () => new MainWorkflow.EatBreadActionParameters(), null, null),
            new ActionMessage("initAgent", ei, () => new MainWorkflow.InitAgentActionParameters(), null, null),
        });

        // states

        this.AddStates(new State[] {
            new State("start", "Start", "", this, false, 0, true, false),
            new State("end", "End", "", this, false, 0, false, true),
            new State("init", "Init", "", this, false, 0, false, false),
            new State("wait", "Wait", "", this, true, 0, false, false),
            new State("physiology", "Physiology", "", this, false, 0, false, false),
            new State("Alternative", "Alternative", "", this, false, 0, true, false),
        });

        // transitions

        this.AddTransitions(new Transition[] {
            new TransitionSplit("split", "Split", "", false, new[] {   
                new[] { "wait", "Main" }, 
                new[] { "physiology", "Physiology" }, 
            }, this),
            new TransitionJoin("join", "Join", "", this),
        });

        // connections
        this.Connect("c0", this.GetPosition("start"), this.GetPosition("init"), this.GetAction("initAgent"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, PhysiologyRole.Store, InitAgentActionParameters>()
                  .Action((i, w, g, o, r, a) => { r.HungerModifier = a.HungerModifier;
r.ThirstModifier = a.ThirstModifier;
r.FatigueModifier = a.FatigueModifier;; } ));
            
        this.Connect("c4", this.GetPosition("init"), this.GetPosition("split"), this.GetAction(""), 0);
            
        this.Connect("c1", this.GetPosition("split"), this.GetPosition("wait"), this.GetAction(""), 0);
            
        this.Connect("c2", this.GetPosition("split"), this.GetPosition("physiology"), this.GetAction("physiologyworkflow"), 0);
            
        this.Connect("c3", this.GetPosition("wait"), this.GetPosition("join"), this.GetAction(""), 0);
            
        this.Connect("c5", this.GetPosition("physiology"), this.GetPosition("join"), this.GetAction(""), 0);
            
        this.Connect("c6", this.GetPosition("join"), this.GetPosition("end"), this.GetAction(""), 0);
            
        this.Connect("c10", this.GetPosition(""), this.GetPosition(""), this.GetAction("fisherWorkflow"), 2)
              .Condition(new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, FisherRole.Store, FisherWorkflowActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return true; }))
              .AddEffects(new AccessCondition[] { 
                  new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, HumanRole.Store, ParameterState>()
                  .Action((i, w, g, o, r, a) => { r.Water++;; } ), 
                  new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, FisherRole.Store, ParameterState>()
                  .Action((i, w, g, o, r, a) => { r.Fish++;; } ), 
                  new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, HumanRole.Store, ParameterState>()
                  .Action((i, w, g, o, r, a) => { r.Wood++;; } ), 
                  new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, FisherRole.Store, ParameterState>()
                  .Action((i, w, g, o, r, a) => { r.Spear++;; } ),  });
            
        this.Connect("c9", this.GetPosition(""), this.GetPosition(""), this.GetAction("shepherdWorkflow"), 3)
              .Condition(new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, ShepherdRole.Store, ShepherdWorkflowActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return true; }))
              .AddEffects(new AccessCondition[] { 
                  new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, HumanRole.Store, ParameterState>()
                  .Action((i, w, g, o, r, a) => { r.Water++;; } ), 
                  new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, HumanRole.Store, ParameterState>()
                  .Action((i, w, g, o, r, a) => { r.Milk++;; } ),  });
            
        this.Connect("c8", this.GetPosition(""), this.GetPosition(""), this.GetAction("potterWorkflow"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, PotterRole.Store, PotterWorkflowActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return true; }))
              .AddEffects(new AccessCondition[] { 
                  new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, PotterRole.Store, ParameterState>()
                  .Action((i, w, g, o, r, a) => { r.Pots++;; } ), 
                  new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, HumanRole.Store, ParameterState>()
                  .Action((i, w, g, o, r, a) => { r.Water++;; } ), 
                  new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, PotterRole.Store, ParameterState>()
                  .Action((i, w, g, o, r, a) => { r.Clay++;; } ),  });
            
        this.Connect("c11", this.GetPosition(""), this.GetPosition(""), this.GetAction("exchangeWorkflow"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, BakerRole.Store, ExchangeWorkflowActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return r.Bread > 1; }))
              .Condition(new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, ShepherdRole.Store, ParameterState>()
                  .Allow((i, w, g, o, r, a) => { return r.Milk > 1; }))
              .Condition(new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, FisherRole.Store, ParameterState>()
                  .Allow((i, w, g, o, r, a) => { return r.Fish > 2; }))
              .AddEffects(new AccessCondition[] { 
                  new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, HumanRole.Store, ParameterState>()
                  .Action((i, w, g, o, r, a) => { r.Pots ++;; } ),  });
            
        this.Connect("c7", this.GetPosition(""), this.GetPosition(""), this.GetAction("bakerWorkflow"), 2)
              .Condition(new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, BakerRole.Store, BakerWorkflowActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return true; }))
              .AddEffects(new AccessCondition[] { 
                  new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, HumanRole.Store, ParameterState>()
                  .Action((i, w, g, o, r, a) => { r.Bread++;; } ), 
                  new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, HumanRole.Store, ParameterState>()
                  .Action((i, w, g, o, r, a) => { r.Water++;; } ), 
                  new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, BakerRole.Store, ParameterState>()
                  .Action((i, w, g, o, r, a) => { r.Wheat++;; } ),  });
            
        this.Connect("c12", this.GetPosition(""), this.GetPosition(""), this.GetAction("drink"), 4)
              .Condition(new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, PhysiologyRole.Store, DrinkActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return r.Milk > 0 && r.Thirst > 0.5; })
                  .Action((i, w, g, o, r, a) => { r.ThirstDecay = r.ThirstDecay - 1;
r.ThirstReservoir = 1 - ((3.2f - r.ThirstDecay) / 3.2f);
r.Thirst = (i.Circadian + r.ThirstReservoir) / 2;
r.Water = r.Water - 1;; } ));
            
        this.Connect("c13", this.GetPosition(""), this.GetPosition(""), this.GetAction("drinkMilk"), 2)
              .Condition(new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, PhysiologyRole.Store, DrinkMilkActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return r.Milk > 0 && r.Hunger > 0.5; })
                  .Action((i, w, g, o, r, a) => { r.ThirstDecay = r.ThirstDecay - 0.5f;
r.ThirstReservoir = 1 - ((3.2f - r.ThirstDecay) / 3.2f);
r.Thirst = (i.Circadian + r.ThirstReservoir) / 2;
r.HungerDecay = r.HungerDecay - 1100;
r.HungerReservoir = 1 - ((8336 - r.HungerDecay) / 8336);
r.Hunger = (i.Circadian + r.HungerReservoir) / 2;
r.Milk = r.Milk - 1;; } ));
            
        this.Connect("c14", this.GetPosition(""), this.GetPosition(""), this.GetAction("eatFish"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, PhysiologyRole.Store, EatFishActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return r.Fish > 0 && r.Hunger > 0.5; })
                  .Action((i, w, g, o, r, a) => { r.HungerDecay = r.HungerDecay - 4560; // 456 kj/100g (Assumin 1kg fish)
r.HungerReservoir = 1 - ((8336 - r.HungerDecay) / 8336);
r.Hunger = (i.Circadian + r.HungerReservoir) / 2;
r.Fish = r.Fish - 1;; } ));
            
        this.Connect("c15", this.GetPosition(""), this.GetPosition(""), this.GetAction("eatBread"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, PhysiologyRole.Store, EatBreadActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return r.Bread > 0 && r.Hunger > 0.5; }))
              .AddEffects(new AccessCondition[] { 
                  new AccessCondition<DefaultInstitution.Store, MainWorkflow.Store, TribeOrganisation.Store, PhysiologyRole.Store, ParameterState>()
                  .Action((i, w, g, o, r, a) => { r.HungerDecay = r.HungerDecay - 2400; // 1200 kj/100g (Assumin double serve)
r.HungerReservoir = 1 - ((8336 - r.HungerDecay) / 8336);
r.Hunger = (i.Circadian + r.HungerReservoir) / 2;
r.Bread = r.Bread - 1;; } ),  });
            
        this.Connect("c16", this.GetPosition("Alternative"), this.GetPosition("wait"), this.GetAction(""), 0);
            

        // create permissions

        // entry permissions

        // IMPORTANT: this needs to be called to initialise connections

        this.Init();
    }
}
#endregion
#region class BakerWorkflow
public class BakerWorkflow : Workflow {

    // store
    #region Store
    public new class Store : Workflow.Store {

        public override Workflow.Store Clone(Workflow.Store store = null) {
            var current = new BakerWorkflow.Store();

            // clone parent properties
            base.Clone(current);

            // add workflow properties

            return current;
}
    }
    #endregion

    // workflow actions

    #region Action Parameters
    
    #region FindWaterActionParameters
    public class FindWaterActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion
    #region MakeBreadActionParameters
    public class MakeBreadActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion
    #region FindWheatActionParameters
    public class FindWheatActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion   
    #endregion

    // abstract implementation

    public override bool Stateless => true;
    public override bool Static => true;
    public override Workflow.Store CreateStore() {
        return new BakerWorkflow.Store();
    }

    // constructor

    public BakerWorkflow(): this(null) { }

    public BakerWorkflow(Institution ei) : base(ei, "BakerWorkflow") {
        this.Name = "Baker";
        this.Description = "";

        // actions
        this.AddActions(new ActionBase [] {
            new ActionMessage("FindWater", ei, () => new BakerWorkflow.FindWaterActionParameters(), null, null),
            new ActionMessage("MakeBread", ei, () => new BakerWorkflow.MakeBreadActionParameters(), null, null),
            new ActionMessage("FindWheat", ei, () => new BakerWorkflow.FindWheatActionParameters(), null, null),
        });

        // states

        this.AddStates(new State[] {
            new State("Start", "Start", "", this, true, 0, true, false),
            new State("End", "End", "", this, false, 0, false, true),
        });

        // transitions

        this.AddTransitions(new Transition[] {
        });

        // connections
        this.Connect("c0", this.GetPosition(""), this.GetPosition(""), this.GetAction("MakeBread"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, BakerWorkflow.Store, TribeOrganisation.Store, BakerRole.Store, MakeBreadActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return r.Water >= 1 &&
r.Wheat >= 2 &&
(
    r.Pots 
    - Math.Ceiling(r.Water / 5f) 
    - Math.Ceiling(r.Wheat / 5f) 
    - Math.Ceiling(r.Bread / 3f) > 0
); })
                  .Action((i, w, g, o, r, a) => { r.Bread = r.Bread + 1;
r.Water = r.Water - 1;
r.Wheat = r.Wheat - 2;; } ));
            
        this.Connect("c1", this.GetPosition(""), this.GetPosition(""), this.GetAction("FindWheat"), 2)
              .Condition(new AccessCondition<DefaultInstitution.Store, BakerWorkflow.Store, TribeOrganisation.Store, BakerRole.Store, FindWheatActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return r.Wheat <= 30 &&
(
    r.Pots 
    - Math.Ceiling(r.Water / 5f) 
    - Math.Ceiling(r.Wheat / 5f) 
    - Math.Ceiling(r.Bread / 3f) > 0
); })
                  .Action((i, w, g, o, r, a) => { r.Wheat = r.Wheat + 1;; } ));
            
        this.Connect("c2", this.GetPosition(""), this.GetPosition(""), this.GetAction("FindWater"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, BakerWorkflow.Store, TribeOrganisation.Store, BakerRole.Store, FindWaterActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return r.Water <= 20 &&
( // Check free pots
    r.Pots 
    - Math.Ceiling(r.Water / 5f) 
    - Math.Ceiling(r.Wheat / 5f) 
    - Math.Ceiling(r.Bread / 3f) > 0
); })
                  .Action((i, w, g, o, r, a) => { r.Water = r.Water + 1; } ));
            
        this.Connect("c3", this.GetPosition("Start"), this.GetPosition("End"), this.GetAction(""), 0);
            

        // create permissions
        this.AddCreatePermissions(new AccessCondition<DefaultInstitution.Store, BakerWorkflow.Store, TribeOrganisation.Store, BakerRole.Store, ParameterState>()
                  .Allow((i, w, g, o, r, a) => { return r.Water > 0 && 
r.Wheat > 1 && 
// Check free pots
(r.Pots 
    - Math.Ceiling(r.Water / 5f) 
    - Math.Ceiling(r.Wheat / 5f) 
    - Math.Ceiling(r.Bread / 3f) > 0); })
                  .Action((i, w, g, o, r, a) => { r.Bread = r.Bread + 1;
r.Water = r.Water - 1;
r.Wheat = r.Wheat - 2;; } ));

        // entry permissions

        // IMPORTANT: this needs to be called to initialise connections

        this.Init();
    }
}
#endregion
#region class ExchangeWorkflow
public class ExchangeWorkflow : Workflow {

    // store
    #region Store
    public new class Store : Workflow.Store {

        public override Workflow.Store Clone(Workflow.Store store = null) {
            var current = new ExchangeWorkflow.Store();

            // clone parent properties
            base.Clone(current);

            // add workflow properties

            return current;
}
    }
    #endregion

    // workflow actions

    #region Action Parameters
    
    #region ExchangePotActionParameters
    public class ExchangePotActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion   
    #endregion

    // abstract implementation

    public override bool Stateless => true;
    public override bool Static => false;
    public override Workflow.Store CreateStore() {
        return new ExchangeWorkflow.Store();
    }

    // constructor

    public ExchangeWorkflow(): this(null) { }

    public ExchangeWorkflow(Institution ei) : base(ei, "ExchangeWorkflow") {
        this.Name = "Exchange";
        this.Description = "";

        // actions
        this.AddActions(new ActionBase [] {
            new ActionMessage("ExchangePot", ei, () => new ExchangeWorkflow.ExchangePotActionParameters(), null, null),
        });

        // states

        this.AddStates(new State[] {
            new State("Start", "Start", "", this, false, 0, true, false),
            new State("End", "End", "", this, false, 0, false, true),
        });

        // transitions

        this.AddTransitions(new Transition[] {
        });

        // connections
        this.Connect("c0", this.GetPosition("Start"), this.GetPosition("End"), this.GetAction("ExchangePot"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, ExchangeWorkflow.Store, TribeOrganisation.Store, BakerRole.Store, ExchangePotActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return r.Bread > 2; })
                  .Action((i, w, g, o, r, a) => { if (w.Owner == null) {
    return;
}
var potter = (PotterRole.Store) w.Owner.FindProvider("Pots");
potter.Pots = potter.Pots - 1;
potter.Bread = potter.Bread + 2;
r.Pots = r.Pots + 1;
r.Bread = r.Bread - 2;; } ));
            
        this.Connect("c1", this.GetPosition("Start"), this.GetPosition("End"), this.GetAction("ExchangePot"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, ExchangeWorkflow.Store, TribeOrganisation.Store, ShepherdRole.Store, ExchangePotActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return r.Milk >= 3; })
                  .Action((i, w, g, o, r, a) => { if (w.Owner == null) {
    return;
}
var potter = (PotterRole.Store) w.Owner.FindProvider("Pots");
potter.Pots = potter.Pots - 1;
potter.Milk = potter.Milk + 3;
r.Pots = r.Pots + 1;
r.Bread = r.Milk - 3;; } ));
            
        this.Connect("c2", this.GetPosition("Start"), this.GetPosition("End"), this.GetAction("ExchangePot"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, ExchangeWorkflow.Store, TribeOrganisation.Store, FisherRole.Store, ExchangePotActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return r.Fish >= 2; })
                  .Action((i, w, g, o, r, a) => { if (w.Owner == null) {
    return;
}
var potter = (PotterRole.Store) w.Owner.FindProvider("Pots");
potter.Pots = potter.Pots - 1;
potter.Milk = potter.Fish + 2;
r.Pots = r.Pots + 1;
r.Bread = r.Fish - 2;; } ));
            

        // create permissions

        // entry permissions

        // IMPORTANT: this needs to be called to initialise connections

        this.Init();
    }
}
#endregion
#region class FishingWorkflow
public class FishingWorkflow : Workflow {

    // store
    #region Store
    public new class Store : Workflow.Store {

        public override Workflow.Store Clone(Workflow.Store store = null) {
            var current = new FishingWorkflow.Store();

            // clone parent properties
            base.Clone(current);

            // add workflow properties

            return current;
}
    }
    #endregion

    // workflow actions

    #region Action Parameters
    
    #region FindWaterActionParameters
    public class FindWaterActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion
    #region FindWoodActionParameters
    public class FindWoodActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion
    #region MakeSpearActionParameters
    public class MakeSpearActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion
    #region CatchFishActionParameters
    public class CatchFishActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion   
    #endregion

    // abstract implementation

    public override bool Stateless => true;
    public override bool Static => true;
    public override Workflow.Store CreateStore() {
        return new FishingWorkflow.Store();
    }

    // constructor

    public FishingWorkflow(): this(null) { }

    public FishingWorkflow(Institution ei) : base(ei, "FisherWorkflow") {
        this.Name = "Fishing";
        this.Description = "";

        // actions
        this.AddActions(new ActionBase [] {
            new ActionMessage("FindWater", ei, () => new FishingWorkflow.FindWaterActionParameters(), null, null),
            new ActionMessage("FindWood", ei, () => new FishingWorkflow.FindWoodActionParameters(), null, null),
            new ActionMessage("MakeSpear", ei, () => new FishingWorkflow.MakeSpearActionParameters(), null, null),
            new ActionMessage("CatchFish", ei, () => new FishingWorkflow.CatchFishActionParameters(), null, null),
        });

        // states

        this.AddStates(new State[] {
            new State("Start", "Start", "", this, true, 0, true, false),
            new State("End", "End", "", this, false, 0, false, true),
        });

        // transitions

        this.AddTransitions(new Transition[] {
        });

        // connections
        this.Connect("c0", this.GetPosition(""), this.GetPosition(""), this.GetAction("FindWood"), 2)
              .Condition(new AccessCondition<DefaultInstitution.Store, FishingWorkflow.Store, TribeOrganisation.Store, HumanRole.Store, FindWoodActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return r.Wood <= 5; })
                  .Action((i, w, g, o, r, a) => { r.Wood++;; } ));
            
        this.Connect("c1", this.GetPosition(""), this.GetPosition(""), this.GetAction("CatchFish"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, FishingWorkflow.Store, TribeOrganisation.Store, FisherRole.Store, CatchFishActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return r.Spear > 0 &&
r.Fish <= 50 &&
// check if there is a pot to store the bread (we store max 5 per pot)
r.Pots - Math.Ceiling(r.Water / 5f) - Math.Ceiling(r.Fish / 3f) > 0; })
                  .Action((i, w, g, o, r, a) => { r.Fish++;
r.Spear = r.Spear - (new Random().Next() % 2);; } ));
            
        this.Connect("c2", this.GetPosition(""), this.GetPosition(""), this.GetAction("MakeSpear"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, FishingWorkflow.Store, TribeOrganisation.Store, FisherRole.Store, MakeSpearActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return r.Spear <= 3 &&
r.Wood >= 2; })
                  .Action((i, w, g, o, r, a) => { r.Wood -= 2;
r.Spear++;; } ));
            
        this.Connect("c3", this.GetPosition(""), this.GetPosition(""), this.GetAction("FindWater"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, FishingWorkflow.Store, TribeOrganisation.Store, FisherRole.Store, FindWaterActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return r.Water <= 20 &&
r.Pots - Math.Ceiling(r.Water / 5f) - Math.Ceiling(r.Fish / 3f) > 0; })
                  .Action((i, w, g, o, r, a) => { r.Water++;; } ));
            
        this.Connect("c4", this.GetPosition("Start"), this.GetPosition("End"), this.GetAction(""), 0);
            

        // create permissions

        // entry permissions

        // IMPORTANT: this needs to be called to initialise connections

        this.Init();
    }
}
#endregion
#region class PotterWorkflow
public class PotterWorkflow : Workflow {

    // store
    #region Store
    public new class Store : Workflow.Store {

        public override Workflow.Store Clone(Workflow.Store store = null) {
            var current = new PotterWorkflow.Store();

            // clone parent properties
            base.Clone(current);

            // add workflow properties

            return current;
}
    }
    #endregion

    // workflow actions

    #region Action Parameters
    
    #region FindWaterActionParameters
    public class FindWaterActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion
    #region MakePotActionParameters
    public class MakePotActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion
    #region FindClayActionParameters
    public class FindClayActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion
    #region ExchangePotActionParameters
    public class ExchangePotActionParameters: ActionJoinWorkflow.Parameters {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion   
    #endregion

    // abstract implementation

    public override bool Stateless => true;
    public override bool Static => true;
    public override Workflow.Store CreateStore() {
        return new PotterWorkflow.Store();
    }

    // constructor

    public PotterWorkflow(): this(null) { }

    public PotterWorkflow(Institution ei) : base(ei, "PotterWorkflow") {
        this.Name = "Potter";
        this.Description = "";

        // actions
        this.AddActions(new ActionBase [] {
            new ActionMessage("FindWater", ei, () => new PotterWorkflow.FindWaterActionParameters(), null, null),
            new ActionMessage("MakePot", ei, () => new PotterWorkflow.MakePotActionParameters(), null, null),
            new ActionMessage("FindClay", ei, () => new PotterWorkflow.FindClayActionParameters(), null, null),
            new ActionJoinWorkflow("ExchangePot", ei, "", () => new PotterWorkflow.ExchangePotActionParameters()),
        });

        // states

        this.AddStates(new State[] {
            new State("Start", "Start", "", this, true, 0, true, false),
            new State("End", "End", "", this, false, 0, false, true),
        });

        // transitions

        this.AddTransitions(new Transition[] {
        });

        // connections
        this.Connect("c0", this.GetPosition(""), this.GetPosition(""), this.GetAction("MakePot"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, PotterWorkflow.Store, TribeOrganisation.Store, PotterRole.Store, MakePotActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return r.ExchangeId >= 0 &&
r.Clay >= 1 &&
r.Water >= 1; })
                  .Action((i, w, g, o, r, a) => { r.Pots += 1;
r.Water -= 1;
r.Clay -= 1;; } ));
            
        this.Connect("c1", this.GetPosition(""), this.GetPosition(""), this.GetAction("FindClay"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, PotterWorkflow.Store, TribeOrganisation.Store, PotterRole.Store, FindClayActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return r.Clay <= 10; })
                  .Action((i, w, g, o, r, a) => { r.Clay++;; } ));
            
        this.Connect("c2", this.GetPosition(""), this.GetPosition(""), this.GetAction("FindWater"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, PotterWorkflow.Store, TribeOrganisation.Store, HumanRole.Store, FindWaterActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return r.Water <= 20; })
                  .Action((i, w, g, o, r, a) => { r.Water++;; } ));
            
        this.Connect("c3", this.GetPosition(""), this.GetPosition(""), this.GetAction("ExchangePot"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, PotterWorkflow.Store, TribeOrganisation.Store, PotterRole.Store, ExchangePotActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return r.ExchangeId == -1; })
                  .Action((i, w, g, o, r, a) => { r.ExchangeId = g.CreatedInstanceId;; } ))
              .AddEffects(new AccessCondition[] { 
                  new AccessCondition<DefaultInstitution.Store, PotterWorkflow.Store, TribeOrganisation.Store, PotterRole.Store, ParameterState>()
                  .Action((i, w, g, o, r, a) => { r.ExchangeId = 1;; } ),  });
            
        this.Connect("c4", this.GetPosition("Start"), this.GetPosition("End"), this.GetAction(""), 0);
            

        // create permissions

        // entry permissions

        // IMPORTANT: this needs to be called to initialise connections

        this.Init();
    }
}
#endregion
#region class PhysiologyWorkflow
public class PhysiologyWorkflow : Workflow {

    // store
    #region Store
    public new class Store : Workflow.Store {

        public override Workflow.Store Clone(Workflow.Store store = null) {
            var current = new PhysiologyWorkflow.Store();

            // clone parent properties
            base.Clone(current);

            // add workflow properties

            return current;
}
    }
    #endregion

    // workflow actions

    #region Action Parameters
    
    #region TimeoutActionParameters
    public class TimeoutActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion
    #region SleepActionParameters
    public class SleepActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion
    #region WakeUpActionParameters
    public class WakeUpActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion
    #region MovingActionParameters
    public class MovingActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion
    #region StoppedActionParameters
    public class StoppedActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion
    #region RestActionParameters
    public class RestActionParameters: ParameterState {
        
        // Properties

        public float Fatigue { get; set; }

        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                    case "Fatigue":
                        this.Fatigue = float.Parse(property.Value);
                        break;
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion
    #region RestedActionParameters
    public class RestedActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion
    #region SetFatigueActionParameters
    public class SetFatigueActionParameters: ParameterState {
        
        // Properties

        public float Fatigue { get; set; }

        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                    case "Fatigue":
                        this.Fatigue = float.Parse(property.Value);
                        break;
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion
    #region EatActionParameters
    public class EatActionParameters: ParameterState {
        
        // Properties

        public float Hunger { get; set; }

        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                    case "Hunger":
                        this.Hunger = float.Parse(property.Value);
                        break;
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion   
    #endregion

    // abstract implementation

    public override bool Stateless => false;
    public override bool Static => false;
    public override Workflow.Store CreateStore() {
        return new PhysiologyWorkflow.Store();
    }

    // constructor

    public PhysiologyWorkflow(): this(null) { }

    public PhysiologyWorkflow(Institution ei) : base(ei, "PhysiologyWorkflow") {
        this.Name = "Physiology";
        this.Description = "";

        // actions
        this.AddActions(new ActionBase [] {
            new ActionTimeout("Timeout", ei),
            new ActionMessage("Sleep", ei, () => new PhysiologyWorkflow.SleepActionParameters(), null, null),
            new ActionMessage("WakeUp", ei, () => new PhysiologyWorkflow.WakeUpActionParameters(), null, null),
            new ActionMessage("Moving", ei, () => new PhysiologyWorkflow.MovingActionParameters(), null, null),
            new ActionMessage("Stopped", ei, () => new PhysiologyWorkflow.StoppedActionParameters(), null, null),
            new ActionMessage("Rest", ei, () => new PhysiologyWorkflow.RestActionParameters(), null, null),
            new ActionMessage("Rested", ei, () => new PhysiologyWorkflow.RestedActionParameters(), null, null),
            new ActionMessage("SetFatigue", ei, () => new PhysiologyWorkflow.SetFatigueActionParameters(), null, null),
            new ActionMessage("Eat", ei, () => new PhysiologyWorkflow.EatActionParameters(), null, null),
        });

        // states

        this.AddStates(new State[] {
            new State("Start", "Start", "", this, true, 0, true, false),
            new State("End", "End", "", this, false, 0, false, true),
        });

        // transitions

        this.AddTransitions(new Transition[] {
        });

        // connections
        this.Connect("c0", this.GetPosition("Start"), this.GetPosition("End"), this.GetAction(""), 0);
            
        this.Connect("c1", this.GetPosition("Start"), this.GetPosition("Start"), this.GetAction("Timeout"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, PhysiologyWorkflow.Store, TribeOrganisation.Store, PhysiologyRole.Store, TimeoutActionParameters>()
                  .Action((i, w, g, o, r, a) => { var owner = (PhysiologyRole.Store) w.Owner.FindRole(typeof(HumanRole.Store));
owner.Hunger = (i.Circadian + owner.HungerReservoir) / 2;
owner.HungerDecay += ((owner.HungerDecayVariable * (i.Tick / 3600)) * owner.HungerModifier);
owner.HungerReservoir = 1 - ((8336 - owner.HungerDecay) / 8336);
owner.Thirst = (i.Circadian + owner.ThirstReservoir) / 2;
owner.ThirstDecay += ((owner.ThirstDecayVariable * (i.Tick / 3600)) * owner.ThirstModifier);
owner.ThirstReservoir = 1 - ((3.2f - owner.ThirstDecay) / 3.2f);
owner.Fatigue += owner.FatigueModifier * ((7 * (i.Tick / 3600)) / 3);; } ));
            
        this.Connect("c2", this.GetPosition(""), this.GetPosition(""), this.GetAction("Sleep"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, PhysiologyWorkflow.Store, TribeOrganisation.Store, PhysiologyRole.Store, SleepActionParameters>()
                  .Action((i, w, g, o, r, a) => { r.FatigueModifier = -1.5f;
r.HungerDecayVariable = 311.17f;
r.ThirstDecayVariable = 0.06f;; } ));
            
        this.Connect("c3", this.GetPosition(""), this.GetPosition(""), this.GetAction("WakeUp"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, PhysiologyWorkflow.Store, TribeOrganisation.Store, PhysiologyRole.Store, WakeUpActionParameters>()
                  .Action((i, w, g, o, r, a) => { r.FatigueModifier = 1;
r.HungerDecayVariable = 343.33f;
r.ThirstDecayVariable = 0.1333f;; } ));
            
        this.Connect("c4", this.GetPosition(""), this.GetPosition(""), this.GetAction("Moving"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, PhysiologyWorkflow.Store, TribeOrganisation.Store, PhysiologyRole.Store, MovingActionParameters>()
                  .Action((i, w, g, o, r, a) => { r.FatigueModifier = 1;; } ));
            
        this.Connect("c5", this.GetPosition(""), this.GetPosition(""), this.GetAction("Stopped"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, PhysiologyWorkflow.Store, TribeOrganisation.Store, PhysiologyRole.Store, StoppedActionParameters>()
                  .Action((i, w, g, o, r, a) => { r.FatigueModifier = 0.00001f;; } ));
            
        this.Connect("c6", this.GetPosition(""), this.GetPosition(""), this.GetAction("Rest"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, PhysiologyWorkflow.Store, TribeOrganisation.Store, PhysiologyRole.Store, RestActionParameters>()
                  .Action((i, w, g, o, r, a) => { r.Fatigue = a.Fatigue;
r.FatigueModifier /= 10000;; } ));
            
        this.Connect("c7", this.GetPosition(""), this.GetPosition(""), this.GetAction("Rested"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, PhysiologyWorkflow.Store, TribeOrganisation.Store, PhysiologyRole.Store, RestedActionParameters>()
                  .Action((i, w, g, o, r, a) => { r.FatigueModifier *= 10000;; } ));
            
        this.Connect("c8", this.GetPosition(""), this.GetPosition(""), this.GetAction("SetFatigue"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, PhysiologyWorkflow.Store, TribeOrganisation.Store, PhysiologyRole.Store, SetFatigueActionParameters>()
                  .Action((i, w, g, o, r, a) => { r.Fatigue = a.Fatigue;; } ));
            
        this.Connect("c9", this.GetPosition(""), this.GetPosition(""), this.GetAction("Eat"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, PhysiologyWorkflow.Store, TribeOrganisation.Store, PhysiologyRole.Store, EatActionParameters>()
                  .Action((i, w, g, o, r, a) => { r.Hunger = a.Hunger;; } ));
            

        // create permissions

        // entry permissions

        // IMPORTANT: this needs to be called to initialise connections

        this.Init();
    }
}
#endregion
#region class ShepherdWorkflow
public class ShepherdWorkflow : Workflow {

    // store
    #region Store
    public new class Store : Workflow.Store {

        public override Workflow.Store Clone(Workflow.Store store = null) {
            var current = new ShepherdWorkflow.Store();

            // clone parent properties
            base.Clone(current);

            // add workflow properties

            return current;
}
    }
    #endregion

    // workflow actions

    #region Action Parameters
    
    #region FindWaterActionParameters
    public class FindWaterActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion
    #region MilkCowActionParameters
    public class MilkCowActionParameters: ParameterState {
        
        // Properties


        // Abstract implementation

        public override string Validate() {
            var result = base.Validate();
            if (result != null) {
                return result;
            }
    
            return null;
        }

        public override void Parse(VariableInstance[] properties) {
            base.Parse(properties);
            foreach (var property in properties) {
                switch (property.Name) {
                default:
                throw new Exception("Parameter does not exist: " + property.Name);
                }
            }
        }

    }
    #endregion   
    #endregion

    // abstract implementation

    public override bool Stateless => true;
    public override bool Static => true;
    public override Workflow.Store CreateStore() {
        return new ShepherdWorkflow.Store();
    }

    // constructor

    public ShepherdWorkflow(): this(null) { }

    public ShepherdWorkflow(Institution ei) : base(ei, "ShepherdWorkflow") {
        this.Name = "Shepherd";
        this.Description = "";

        // actions
        this.AddActions(new ActionBase [] {
            new ActionMessage("FindWater", ei, () => new ShepherdWorkflow.FindWaterActionParameters(), null, null),
            new ActionMessage("MilkCow", ei, () => new ShepherdWorkflow.MilkCowActionParameters(), null, null),
        });

        // states

        this.AddStates(new State[] {
            new State("Start", "Start", "", this, true, 0, true, false),
            new State("End", "End", "", this, false, 0, false, true),
        });

        // transitions

        this.AddTransitions(new Transition[] {
        });

        // connections
        this.Connect("c0", this.GetPosition(""), this.GetPosition(""), this.GetAction("FindWater"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, ShepherdWorkflow.Store, TribeOrganisation.Store, HumanRole.Store, FindWaterActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return r.Water <= 20 &&
r.Water <= 20 &&
(r.Pots - Math.Ceiling(r.Water / 5f) - Math.Ceiling(r.Milk / 5f)) > 0; })
                  .Action((i, w, g, o, r, a) => { r.Water++;; } ));
            
        this.Connect("c1", this.GetPosition(""), this.GetPosition(""), this.GetAction("MilkCow"), 0)
              .Condition(new AccessCondition<DefaultInstitution.Store, ShepherdWorkflow.Store, TribeOrganisation.Store, ShepherdRole.Store, MilkCowActionParameters>()
                  .Allow((i, w, g, o, r, a) => { return r.Milk <= 30 &&
(r.Pots - Math.Ceiling(r.Water / 5f) - Math.Ceiling(r.Milk / 5f)) > 0; })
                  .Action((i, w, g, o, r, a) => { r.Milk++;; } ));
            
        this.Connect("c2", this.GetPosition("Start"), this.GetPosition("End"), this.GetAction(""), 0);
            

        // create permissions

        // entry permissions

        // IMPORTANT: this needs to be called to initialise connections

        this.Init();
    }
}
#endregion
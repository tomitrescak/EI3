//using Ei.Ontology;
//using Ei.Runtime;
//using Ei.Runtime.Planning;
//using System;
//using System.Collections.Generic;

//#region class ChildOrganisation
//class ChildOrganisation: ParentOrganisation {

//    #region class Store
//    public new class Store : ParentOrganisation.Store
//    {
//        // Properties

//        int Child { get; set; }
       
//        // Ctor
//        public Store() {
//            this.Child = 5;

//            this.defaultValues.AddRange(new List<object>() {
//                5,
//            });
//        } 

//        // Methods

//        public override SearchableState Clone(SearchableState cloneTo = null) {
//            var clone = new Store();
//            base.Clone(clone);
            
//            clone.Child = this.Child;

//            return clone;
//        }

//        public override bool Contains(string name) {
//            if (base.Contains(name)) return true;
//            if (name == "Child") return true;
    
//            return false; 
//        }

//        public override List<VariableInstance> FilterByAccess(Governor governor) {
//            var list = base.FilterByAccess(governor);
//            list.AddRange(new List<VariableInstance> {
//                new VariableInstance("Child", this.Child.ToString())
//            });
//            return list;
//        }

//        public override object GetValue(string name) {
//            if (base.Contains(name)) {
//                return base.GetValue(name);
//            } 
//            if (name == "Child") return this.Child;

//            throw new Exception("Key does not exists: " + name);
//        }

//        public override void SetValue(string name, object value) {
//            if (base.Contains(name)) {
//                base.SetValue(name, value);
//                return;
//            }
//            if (name == "Child") { this.Child = (int) value; return; }

//            throw new Exception("Key does not exists: " + name);
//        }

//        public override ResourceState Merge(BaseState state) {
//            return this;
//        }

//        public override ResourceState NewInstance() {
//            return new ChildOrganisation.Store();
//        }

//        public override void ResetDirty() {
//            base.ResetDirty();

//            defaultValues[0] = this.Child;
//        }

//        public override GoalState[] ToGoalState() {
//            var list = new List<GoalState>();
//            list.AddRange(base.ToGoalState());

//            if (!this.Child.Equals(this.DefaultValues[0])) {
//                list.Add(new GoalState("Child", this.Child, StateGoalStrategy.Equal));
//            }
            
//            return list.ToArray();
//        }
//    }
//    #endregion



    

//    protected override SearchableState CreateState() {
//        return new ChildOrganisation.Store();
//    }
//}
//#endregion
//#region class ParentOrganisation
//class ParentOrganisation: Organisation {


//    #region class Store
//    public class Store : SearchableState
//    {
//        // Fields

//        protected List<object> defaultValues = new List<object>() {
//            2,
//            "Tomi",
//        };

//        // Properties

//        int ParentParameter { get; set; }
//        string ParentPrivateParameter { get; set; }

//        protected override List<object> DefaultValues => defaultValues;
       
//        // Ctor
//        public Store() {
//            this.ParentParameter = 2;
//            this.ParentPrivateParameter = "Tomi";
//        } 

//        // Methods

//        public override SearchableState Clone(SearchableState cloneTo = null) {
//            var clone = cloneTo == null 
//                ? new ParentOrganisation.Store()
//                : (ParentOrganisation.Store) cloneTo;

//            clone.ParentParameter = this.ParentParameter;
//            clone.ParentPrivateParameter = this.ParentPrivateParameter;

//            return clone;
//        }

//        public override bool Contains(string name) {
//            if (name == "ParentParameter") return true;
//            if (name == "ParentPrivateParameter") return true;
    
//            return false; 
//        }

//        public override List<VariableInstance> FilterByAccess(Governor governor) {
//            return new List<VariableInstance> {
//                new VariableInstance("ParentParameter", this.ParentParameter.ToString())
//            };
//        }

//        public override object GetValue(string name) {
//            if (name == "ParentParameter") return this.ParentParameter;
//            if (name == "ParentPrivateParameter") return this.ParentPrivateParameter;

//            throw new Exception("Key does not exists: " + name);
//        }

//        public override void SetValue(string name, object value) {
//            if (name == "ParentParameter") { this.ParentParameter = (int) value; return; }
//            if (name == "ParentPrivateParameter") { this.ParentPrivateParameter = (string) value; return; }

//            throw new Exception("Key does not exists: " + name);
//        }

//        public override ResourceState Merge(BaseState state) {
//            return this;
//        }

//        public override ResourceState NewInstance() {
//            return new ParentOrganisation.Store();
//        }

//        public override void ResetDirty() {
//            defaultValues[0] = this.ParentParameter;
//            defaultValues[1] = this.ParentPrivateParameter;
//        }

//        public override GoalState[] ToGoalState() {
//            var list = new List<GoalState>();
//            if (!this.ParentParameter.Equals(this.DefaultValues[0])) {
//                list.Add(new GoalState("ParentParameter", this.ParentParameter, StateGoalStrategy.Equal));
//            }
//            if (this.ParentPrivateParameter != null && !this.ParentPrivateParameter.Equals(this.DefaultValues[1])) {
//                list.Add(new GoalState("ParentPrivateParameter", this.ParentPrivateParameter, StateGoalStrategy.Equal));
//            }
            
//            return list.ToArray();
//        }
//    }
//    #endregion


//    public ParentOrganisation() : base("parent") {
//        this.Name = "Parent";
//    }

//    protected override SearchableState CreateState() {
//        return new ParentOrganisation.Store();
//    }
//}
//#endregion
using Ei.Runtime;
using Ei.Runtime.Planning;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ei.Tests.Bdd.Unit.Runtime
{
    [TestFixture]
    class VariableState_Test
    {
        public class OtherVariableState : ResourceState
        {
            public int Boo { get; set; }
           
        }

        public class ParentVariableState : ResourceState
        {
            public int Foo { get; set; }
            
        }

        public class ChildVariableState : ParentVariableState
        {
            public string Bar { get; set; }
            
        }

        public class StateWithAttributes : ResourceState
        {
            [Variable]
            public int Public { get; set; }

            [Variable(Access = VariableAccess.Private, DefaultValue = "8")]
            public string Private { get; set; }
        }

        //[TestCase]
        //public void InitialisesFromAllParameters() {
        //    Func<ParentVariableState, int> selector = (ParentVariableState s) => s.Foo;
        //    Action<ParentVariableState, int> updater = (ParentVariableState s, int value) => s.Foo = value;

        //    var prop = new VariableDefinition<int, ParentVariableState>("Test", VariableAccess.Private, 5, int.Parse, selector, updater);

        //}

        [TestCase]
        public void InitialisesPropertyDescriptors() {
            var parentState = new ParentVariableState
            {
                Foo = 4
            };

            Assert.AreEqual(1, parentState.Descriptors.Length);

            // check descriptor
            var prop = parentState.FindByName("Foo");
            var typedProp = (VariableDefinition<int, ParentVariableState>) prop;
            Assert.AreEqual(0, prop.DefaultValue);
            Assert.AreEqual(0, typedProp.TypedDefaultValue);
            Assert.IsTrue(prop.CanAccess(null, null));
            Assert.AreEqual(4, prop.Value(parentState));

            // check modifications and updates
            prop.Update(parentState, 10);
            Assert.AreEqual(10, prop.Value(parentState));
            Assert.AreEqual(10, parentState.Foo);

            prop.Parse(parentState, "20");
            Assert.AreEqual(20, prop.Value(parentState));
            Assert.AreEqual(20, parentState.Foo);

            // check parsing
            Assert.AreEqual(6, typedProp.Parse("6"));

        }

        [TestCase]
        public void InitialisesInheritedPropertyDescriptors() {
            var childState = new ChildVariableState
            {
                Foo = 4,
                Bar = "5"
            };

            Assert.AreEqual(childState.Descriptors.Length, 2);

            Assert.AreEqual(4, childState.FindByName("Foo").Value(childState));
            Assert.AreEqual("5", childState.FindByName("Bar").Value(childState));

            childState.Foo = 10;
            Assert.AreEqual(10, childState.FindByName("Foo").Value(childState));
        }

        [TestCase]
        public void Clone_CanBeCloned() {
            var childState = new ChildVariableState
            {
                Foo = 4,
                Bar = "5"
            };

            var clonedState = (ChildVariableState) childState.Clone();

            Assert.AreEqual(clonedState.Descriptors.Length, 2);
            Assert.AreEqual(4, clonedState.Foo);
            Assert.AreEqual(4, clonedState.FindByName("Foo").Value(clonedState));
            Assert.AreEqual("5", clonedState.Bar);
            Assert.AreEqual("5", clonedState.FindByName("Bar").Value(clonedState));
        }

        [TestCase]
        public void CanBeInitiatedFromVariableInstances() {
            var variables = new[] {
                new VariableInstance("Foo", "7"),
                new VariableInstance("Bar", "9")
            };
            var childState = new ChildVariableState();
            childState.Parse(variables);


            Assert.AreEqual(7, childState.Foo);
            Assert.AreEqual("9", childState.Bar);
        }

        [TestCase]
        public void Merge_MergesTwoVariableStates() {

            var state1 = new ChildVariableState
            {
                Foo = 3,
                Bar = "4"
            };
            var state2 = new ChildVariableState
            {
                Foo = 10,
            };

            state1.Merge(state2);

            Assert.AreEqual(10, state1.Foo);
            Assert.AreEqual("4", state1.Bar);
        }

        [TestCase]
        public void GetValue_AllowsTOretreiveValueOfParameterByName() {

            var state = new ChildVariableState
            {
                Foo = 3
            };
            Assert.AreEqual(3, state.GetValue("Foo"));
        }

        [TestCase]
        public void ToGoalState_CreatesGoalStateFromCurrentState() {

            // find all

            var state = new ChildVariableState
            {
                Foo = 3
            };

            var goalState = state.ToGoalState();
            Assert.AreEqual(2, goalState.Length);

            Assert.AreEqual("Bar", goalState[0].Name);
            Assert.AreEqual(null, goalState[0].Value);
            Assert.AreEqual(StateGoalStrategy.Equal, goalState[0].Strategy);

            Assert.AreEqual("Foo", goalState[1].Name);
            Assert.AreEqual(3, goalState[1].Value);
            Assert.AreEqual(StateGoalStrategy.Equal, goalState[1].Strategy);
        }

        [TestCase]
        public void ToGoalState_CreatesGoalStateFromDirtyState() {

            // find all

            var state = new ChildVariableState
            {
                Foo = 3
            };

            var goalState = state.ToGoalState(true);
            Assert.AreEqual(1, goalState.Length);

            Assert.AreEqual("Foo", goalState[0].Name);
            Assert.AreEqual(3, goalState[0].Value);
            Assert.AreEqual(StateGoalStrategy.Equal, goalState[0].Strategy);
        }

        [TestCase]
        public void ToGoalState_CreatesGoalStateFromResetDirtyState() {

            // find all

            var state = new ChildVariableState
            {
                Foo = 3
            };

            // after reset nothing should be there

            state.ResetDirty();

            var goalState = state.ToGoalState(true);
            Assert.AreEqual(0, goalState.Length);
        }

        [TestCase]
        public void VariableState__Variables_Can_Be_Controlled_By_Properties() {

            // find all

            var state = new StateWithAttributes();
            
            Assert.AreEqual(VariableAccess.Private, state.GetVariableDefiniton("Private").Access);
            Assert.AreEqual("8", state.Private);
        }
    }
}

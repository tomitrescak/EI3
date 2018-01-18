using System;
using System.Collections.Generic;
using System.Text;

namespace Ei.Tests.Steps
{
    public class ScenarioContext
    {
        private List<object> items;

        public ScenarioContext() {
            this.items = new List<object>();
        }

        public T Get<T>() {
            return (T) this.items.Find(i => i is T);
        }

        public void Set<T>(T value) {
            this.items.Add(value);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Runtime
{
    public struct VariableInstance
    {
        public string Name;
        public string Value;

        public VariableInstance(string name, string value) {
            this.Name = name;
            this.Value = value;
        }
    }
}

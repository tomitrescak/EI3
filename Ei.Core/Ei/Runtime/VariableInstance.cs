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

        public static VariableInstance[] Create(params string[] parameteres)
        {
            if (parameteres.Length % 2 == 1) throw new ApplicationException("Parameters have to come in key/value");

            var result = new VariableInstance[parameteres.Length/2];

            for (var i = 0; i < parameteres.Length/2; i++)
            {
                result[i] = new VariableInstance(parameteres[i*2], parameteres[i*2 + 1]);
            }
            return result;
        }
    }
}

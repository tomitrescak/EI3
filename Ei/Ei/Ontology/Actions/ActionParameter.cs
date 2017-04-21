using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Ontology.Actions
{
    public struct ActionParameter1
    {
        public string Key;
        public string Value;

        public ActionParameter1(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public static ActionParameter1[] Create(params string[] parameteres)
        {
            if (parameteres.Length % 2 == 1) throw new ApplicationException("Parameters have to come in key/value");

            var result = new ActionParameter1[parameteres.Length/2];

            for (var i = 0; i < parameteres.Length/2; i++)
            {
                result[i] = new ActionParameter1(parameteres[i*2], parameteres[i*2 + 1]);
            }
            return result;
        }
    }
}

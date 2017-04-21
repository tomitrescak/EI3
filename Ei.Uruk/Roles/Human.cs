using Ei.Ontology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Uruk.Roles
{
    public class Human: Role
    {
        public int Pots { get; set; }
        public int Water { get; set; }

        public Human(string role) : base(role) { }

        public Human(): base("human") {
            this.Pots = 0;
            this.Water = 0;    
        }
    }
}

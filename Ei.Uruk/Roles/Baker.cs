using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Uruk.Roles
{
    public class Baker: Human
    {
        public int Wheat { get; set; }
        public int Bread { get; set; }

        public Baker(string role): base(role) { }

        public Baker(): base("baker") {
            this.Pots = 3;
            this.Wheat = 0;
            this.Bread = 0;
        }
    }
}

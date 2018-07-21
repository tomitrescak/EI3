using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Persistence
{
    public class BareConnectionDao
    {
        public string StateId { get; set; }
        public AccessConditionDao[] Postconditions { get; set; }
    }
}

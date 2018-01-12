using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Persistence
{
    public class BacktrackDao
    {
        public AccessConditionDao[] Allow { get; set; }
        public AccessConditionDao[] Deny { get; set; }
        public AccessConditionDao[] Postconditions { get; set; }
    }
}

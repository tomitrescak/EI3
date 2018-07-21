using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Persistence.Actions
{
    public class ActionDao : ParametricEntityDao
    {
        public string ActionType { get { return this.GetType().Name; } } 
    }
}

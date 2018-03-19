using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Simulator.Core
{
    public abstract class ProjectStatistics
    {
        protected ProjectStatistics(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }
    }
}

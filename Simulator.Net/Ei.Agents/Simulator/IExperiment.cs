using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Simulator.Experiments
{
    public interface IExperiment
    {
        void Evaluate(Ei.Simulator.Core.Project project);
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ei.Simulation.Behaviours.Environment.Objects
{
    public abstract class ObjectAction: MonoBehaviour
    {
        public int Index;

        //[JsonIgnore]
        //public virtual Func<EnvironmentAction, Task<bool>> BeforeEach { get;  }
        //[JsonIgnore]
        //public virtual Func<EnvironmentAction, Task<bool>> AfterEach { get;  }
        //[JsonIgnore]
        //public virtual Func<EnvironmentAction, Task<bool>> BeforeAll { get;  }
        //[JsonIgnore]
        //public virtual Func<EnvironmentAction, Task<bool>> AfterAll { get; }

        public abstract Task<bool> UseObject(SimulationAgent agent, EnvironmentAction action);
    }
}

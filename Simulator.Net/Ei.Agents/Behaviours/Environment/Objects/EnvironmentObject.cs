using Ei.Core.Runtime;
using Ei.Logs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ei.Simulation.Behaviours.Environment.Objects
{

    public class EnvironmentObject : MonoBehaviour
    {
        
        //private ObjectAction[] withBeforeAll;
        //private ObjectAction[] withAfterAll;
        //private ObjectAction[] withBeforeEach;
        //private ObjectAction[] withAfterEach;
       

        // public string Id;

        // [NonSerialized]
        // public Governor Owner;

        [JsonIgnore]
        public string Name { get => this.gameObject.name; } 

        public string Icon;
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ei.Agents.Sims
{
    public class SimObject : MonoBehaviour
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        
        public SimAction[] Actions { get; set; }        
    }
}

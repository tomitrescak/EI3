using UnityEngine;

namespace Ei.Simulation.Sims.Behaviours
{
    public class SimObject : MonoBehaviour
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        
        public SimAction[] Actions { get; set; }        
    }
}

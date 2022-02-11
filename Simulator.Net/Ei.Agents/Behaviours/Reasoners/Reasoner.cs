using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ei.Simulation.Behaviours.Reasoners
{
    public abstract class Reasoner : MonoBehaviour
    {
        public abstract void Reason();
    }
}

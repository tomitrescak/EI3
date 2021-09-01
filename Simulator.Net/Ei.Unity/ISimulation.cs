using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace UnityEngine
{
    public interface ISimulation
    {
        Collection<GameObject> Agents { get; }
        Dictionary<Type, List<MonoBehaviour>> Behaviours { get; }
        GameObject CreateInstance(GameObject agent, bool init = false, int rendered = 0);
        void RemoveAgent(GameObject agent);
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace UnityEngine
{
    public interface IGameEngine
    {
        bool IsRunning { get; }
        Collection<GameObject> GameObjects { get; }
        Dictionary<Type, List<MonoBehaviour>> Behaviours { get; }
        GameObject Instantiate(GameObject agent, bool init = true);
        void Destroy(GameObject agent);
        IEnumerable<T> FindObjectsOfType<T>() where T : MonoBehaviour;
        T FindObjectOfType<T>() where T : MonoBehaviour;
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace Ei.Agents.Core
{
    public interface IProject
    {
        List<GameObject> Agents { get; }
        string ResolvePath(string path);
        // void Start();
    }
}

using UnityEngine;

namespace Ei.Simulation.Core
{
    public class EiBehaviour : MonoBehaviour
    {
        //private Transform tr;
        //[Browsable(false)]
        //protected Transform transform {
        //    get {
        //        if (this.tr == null) {
        //            this.tr = this.transform;
        //        }
        //        return this.tr;
        //    }
        //}

        /// <summary>
        /// Creates and initialises a new gameobject
        /// </summary>
        /// <param name="agent">Gameobject to be created</param>
        /// <param name="init">Should Init function be called?</param>
        /// <param name="renderer">0 = Nothing, 1=Fast Renderer, 2=Icon Renderer, 3=Sim Renderer</param>
        /// <returns>Created GameObject</returns>
        public void CreateInstance(GameObject agent, bool init, int renderer) {
            this.gameObject.GetSimulator().CreateInstance(agent, init, renderer);
        }
    }
}

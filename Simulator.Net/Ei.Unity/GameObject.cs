using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using UnityEngine;
using Newtonsoft.Json;

namespace UnityEngine
{
    public class GameObject 
    {
        public static IGameEngine simulation;
   
        private Transform _transform;
        private MonoBehaviour[] _updatebleComponents;

        // ctors

        public GameObject() {
            this.Components = new List<MonoBehaviour>();
            // this.AddComponent<Transform>();
            this.Enabled = true;
        }

        public GameObject(string name): this() {
            this.name = name;
        }

        // properties

        [Browsable(false)]
        public List<MonoBehaviour> Components { get; set; }
        public bool Enabled { get; set; }
        [Browsable(false)]
        public string name { get; set; }

        [JsonIgnore]
        [Browsable(false)]
        public bool Updates { get { return this._updatebleComponents.Length > 0; } }
        [Browsable(false)]
        [JsonIgnore]
        public bool Selected { get; set; }
        public int layer;

        [JsonIgnore]
        [Browsable(false)]
        public Transform transform {
            get {
                if (this._transform == null) {
                    this._transform = this.GetComponent<Transform>();
                }
                return this._transform;
            }
        }

        [JsonIgnore]
        public IGameEngine GameEngine { get; private set; }

        public static GameObject Find(string name)
        {
            return GameObject.simulation.GameObjects.FirstOrDefault(g => g.name == name);
        }

        // public proxy methods



        public GameObject Instantiate(GameObject go)
        {
            return this.GameEngine.Instantiate(go);
        }

        public T GetComponent<T>() where T : MonoBehaviour
        {
            return (T)this.Components.FirstOrDefault((c) => c is T);
        }

        public T[] GetComponents<T>() where T : MonoBehaviour
        {
            return (T[]) this.Components.Where((c) => c is T).Cast<T>().ToArray();
        }

        public T AddComponent<T>(T component) where T : MonoBehaviour
        {
            component.gameObject = this;

            this.Components.Add(component);
            return component;
        }

        public T AddComponent<T>() where T : MonoBehaviour
        {
            var component = Activator.CreateInstance<T>();

            return this.AddComponent(component);
        }


        // public methods

        public void Init(IGameEngine simulation) {
            
            foreach (var component in this.Components) {
                this.InitComponent(component, simulation);
                this.UpdateComponents(component);
            }
        }

        public static void Destroy(GameObject gameObject) {
            // remove from agents
            simulation.Destroy(gameObject);

            foreach (var component in gameObject.Components) {
                simulation.Behaviours[component.GetType()].Remove(component);
            }
        }

        private IGameEngine GetSimulator() {
            return this.GameEngine;
        }

        public void Start() {
            foreach (var component in this.Components) {
                if (component.StartAction != null) {
                    component.StartAction();
                }
            }
        }

        public void Update() {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            foreach (var component in this._updatebleComponents) {
                // Stopwatch sw1 = new Stopwatch();
                // sw1.Start();

                // TODO: Analyse how fast is this, otherwise I will have to use the IUpdate approach
                component.UpdateAction.Invoke();
                // Debug.WriteLine($"Component {component.GetType().Name} processed in {sw1.ElapsedMilliseconds} {sw1.ElapsedTicks}");
            }

            //Debug.WriteLine($"Agent processed in {sw.ElapsedMilliseconds} {sw.ElapsedTicks}");

        }

        


        // helper methods

        private void InitComponent(MonoBehaviour component, IGameEngine simulation)
        {
            this.GameEngine = simulation;
            component.gameObject = this;
            component.InitAction?.Invoke();
        }

        private void UpdateComponents(MonoBehaviour component)
        {
            // add to cache
            var componentType = component.GetType();
            if (!GameObject.simulation.Behaviours.ContainsKey(componentType))
            {
                GameObject.simulation.Behaviours.Add(componentType, new List<MonoBehaviour>());
            }
            if (!GameObject.simulation.Behaviours[componentType].Contains(component))
            {
                GameObject.simulation.Behaviours[componentType].Add(component);
            }
            this._updatebleComponents = this.Components.Where(c => c.UpdateAction != null).ToArray();
        }

    }
}

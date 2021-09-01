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

namespace UnityEngine
{
    public class GameObject 
    {
        public static ISimulation simulation;
   
        private Transform _transform;
        private IUpdates[] _updatebleComponents;

        // ctors

        public GameObject() {
            this.Components = new List<MonoBehaviour>();
            this.AddComponent<Transform>();
        }

        public GameObject(string name): this() {
            this.name = name;
        }

        // properties

        [Browsable(false)]
        public List<MonoBehaviour> Components { get; set; }
        public bool Enabled { get; set; }
        [Browsable(false)]
        public bool Started { get; set; }
        public string name { get; set; }

        [Browsable(false)]
        public bool Updates { get { return this._updatebleComponents.Length > 0; } }
        [Browsable(false)]
        public bool Selected { get; set; }
        public int layer;

        [Browsable(false)]
        public Transform Transform {
            get {
                if (this._transform == null) {
                    this._transform = this.GetComponent<Transform>();
                }
                return this._transform;
            }
        }
        [Browsable(false)]
        public Transform transform {
            get {
                if (this._transform == null) {
                    this._transform = this.GetComponent<Transform>();
                }
                return this._transform;
            }
        }

        protected ISimulation Simulator { get; set; }

        // public static methods

        public static IEnumerable<T> FindObjectsOfType<T>() where T: MonoBehaviour {
            // return GameObject.simulation.Behaviours[typeof(T)].Cast<T>().ToList();
            if (GameObject.simulation.Behaviours.ContainsKey(typeof(T))) {
                return GameObject.simulation.Behaviours[typeof(T)].Cast<T>();
            }
            return new T[0];
        }

        // public methods

        public void Init(ISimulation simulation) {
            
            foreach (var component in this.Components) {
                this.InitComponent(component, simulation);
                this.UpdateComponents(component);
            }
        }

        public void InitComponent(MonoBehaviour component, ISimulation simulation) {
            this.Simulator = simulation;
            component.gameObject = this;
            component.InitAction?.Invoke();
        }

        public void UpdateComponents(MonoBehaviour component) {
            // add to cache
            var componentType = component.GetType();
            if (!GameObject.simulation.Behaviours.ContainsKey(componentType)) {
                GameObject.simulation.Behaviours.Add(componentType, new List<MonoBehaviour>());
            }
            if (!GameObject.simulation.Behaviours[componentType].Contains(component)) {
                GameObject.simulation.Behaviours[componentType].Add(component);
            }
            this._updatebleComponents = this.Components.Where(c => c is IUpdates).Cast<IUpdates>().ToArray();
        }

        public static void Destroy(GameObject gameObject) {
            // remove from agents
            simulation.RemoveAgent(gameObject);

            foreach (var component in gameObject.Components) {
                simulation.Behaviours[component.GetType()].Remove(component);
            }
        }

        public ISimulation GetSimulator() {
            return this.Simulator;
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
                component.Update();
                // Debug.WriteLine($"Component {component.GetType().Name} processed in {sw1.ElapsedMilliseconds} {sw1.ElapsedTicks}");
            }

            //Debug.WriteLine($"Agent processed in {sw.ElapsedMilliseconds} {sw.ElapsedTicks}");

        }

        public GameObject Instantiate(GameObject agent) {
            return this.Simulator.CreateInstance(agent);
        }

        public T GetComponent<T>() where T : MonoBehaviour {
            return (T) this.Components.FirstOrDefault((c) => c is T);
        }

        public T AddComponent<T>(T component) where T : MonoBehaviour {
            component.gameObject = this;
 
            this.Components.Add(component);
            return component;
        }
        
        public T AddComponent<T>() where T : MonoBehaviour {
            var component = Activator.CreateInstance<T>();
            
            return this.AddComponent(component);
        }

    }
}

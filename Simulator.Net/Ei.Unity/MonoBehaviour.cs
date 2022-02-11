using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace UnityEngine
{
    //public interface IUpdates
    //{
    //    void Update();
    //}

    public class MonoBehaviour : INotifyPropertyChanged
    {
        private Dictionary<string, float> notifications = new Dictionary<string, float>();

        [Browsable(false)]
        [JsonIgnore]
        public Transform transform => this.gameObject.transform;

        [JsonIgnore]
        public GameObject gameObject;

        public delegate void ActionDelegate();

        [JsonIgnore]
        public readonly ActionDelegate StartAction;

        [JsonIgnore]
        public readonly ActionDelegate InitAction;

        [JsonIgnore]
        public readonly ActionDelegate UpdateAction;

        public MonoBehaviour() {
            var initMethod = this.GetType().GetMethod("Init");
            if (initMethod != null) {
                this.InitAction = (ActionDelegate)Delegate.CreateDelegate(typeof(ActionDelegate), this, initMethod);
            }
            var startMethod = this.GetType().GetMethod("Start");
            if (startMethod != null) {
                this.StartAction = (ActionDelegate)Delegate.CreateDelegate(typeof(ActionDelegate), this, startMethod);
            }
            var updateMethod = this.GetType().GetMethod("Update");
            if (updateMethod != null) {
                this.UpdateAction = (ActionDelegate)Delegate.CreateDelegate(typeof(ActionDelegate), this, updateMethod);
            }
        }

        public T GetComponent<T>() where T : MonoBehaviour {
            return this.gameObject.GetComponent<T>();
        }

        public T[] GetComponents<T>() where T : MonoBehaviour
        {
            return this.gameObject.GetComponents<T>();
        }

        public T AddComponent<T>(T component) where T : MonoBehaviour {
            return this.gameObject.AddComponent(component);
        }

        public IEnumerable<T> FindObjectsOfType<T>() where T : MonoBehaviour
        {
            // return GameObject.simulation.Behaviours[typeof(T)].Cast<T>().ToList();
            if (this.gameObject.GameEngine.Behaviours.ContainsKey(typeof(T)))
            {
                return this.gameObject.GameEngine.Behaviours[typeof(T)].Cast<T>();
            }
            // TODO: Return all subtypes
            var inheritedKey = this.gameObject.GameEngine.Behaviours.Keys.FirstOrDefault(x => x.IsSubclassOf(typeof(T)));
            if (inheritedKey != null)
            {
                return this.gameObject.GameEngine.Behaviours[inheritedKey].Cast<T>();
            }
            return null; // new T[0];
        }

        public T FindObjectOfType<T>() where T : MonoBehaviour
        {
            // return GameObject.simulation.Behaviours[typeof(T)].Cast<T>().ToList();
            if (this.gameObject.GameEngine.Behaviours.ContainsKey(typeof(T)))
            {
                return this.gameObject.GameEngine.Behaviours[typeof(T)].Cast<T>().First();
            }
            var inheritedKey = this.gameObject.GameEngine.Behaviours.Keys.FirstOrDefault(x => x.IsSubclassOf(typeof (T)));
            if (inheritedKey != null)
            {
                return this.gameObject.GameEngine.Behaviours[inheritedKey].Cast<T>().First();
            }
            return null; // new T[0];
        }


        // notifications

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            if (this.PropertyChanged == null) { return; }
            if (!notifications.ContainsKey(propertyName)) {
                notifications[propertyName] = Time.time;
            }

            if (Time.time - notifications[propertyName] < 1) {
                return;
            }
            notifications[propertyName] = Time.time;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected GameObject Instantiate(GameObject agent)
        {
            return this.gameObject.GameEngine.Instantiate(agent);
        }
    }
}

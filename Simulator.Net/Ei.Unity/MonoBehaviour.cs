using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace UnityEngine
{
    public interface IUpdates
    {
        void Update();
    }

    public class MonoBehaviour : INotifyPropertyChanged
    {
        private Dictionary<string, float> notifications = new Dictionary<string, float>();

        [Browsable(false)]
        public Transform transform {
            get {
                return this.gameObject.transform;
            }
        }
        public GameObject gameObject;

        public delegate void ActionDelegate();

        public ActionDelegate StartAction;
        public ActionDelegate InitAction;
        public ActionDelegate UpdateAction;

        public MonoBehaviour() {
            MethodInfo initMethod = this.GetType().GetMethod("Init");
            if (initMethod != null) {
                this.InitAction = (ActionDelegate)Delegate.CreateDelegate(typeof(ActionDelegate), this, initMethod);
            }
            MethodInfo startMethod = this.GetType().GetMethod("Start");
            if (startMethod != null) {
                this.StartAction = (ActionDelegate)Delegate.CreateDelegate(typeof(ActionDelegate), this, startMethod);
            }
            MethodInfo updateMethod = this.GetType().GetMethod("Update");
            if (updateMethod != null) {
                this.UpdateAction = (ActionDelegate)Delegate.CreateDelegate(typeof(ActionDelegate), this, updateMethod);
            }
        }

        public T GetComponent<T>() where T : MonoBehaviour {
            return this.gameObject.GetComponent<T>();
        }

        public T AddComponent<T>(T component) where T : MonoBehaviour {
            return this.gameObject.AddComponent(component);
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
    }
}

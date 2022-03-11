// using Glide;

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Ei.Logs;
using Ei.Simulation.Core;
using Newtonsoft.Json;
using UnityEngine;

namespace Ei.Simulation.Behaviours
{
    [DisplayName("Linear Navigation")]
    public class LinearNavigation : NavigationBase //, IUpdates
    {
        private Tweener tween;
        private DateTime startTime;
        private TaskCompletionSource<bool> tsc;

        [JsonIgnore]
        public float SpeedPxPerSecond;

        [JsonIgnore]
        public bool Navigating { get; private set; }

        public void Start()
        {
            var env = FindObjectOfType<AgentEnvironment>();
            var timer = FindObjectOfType<SimulationTimer>();  
            var project = FindObjectOfType<SimulationProject>();

            if (this.SpeedPxPerSecond == 0)
            {
                this.SpeedPxPerSecond = this.SpeedKmH * // base speed (e.g. 5 km/h = 1.47 m/s)
                    (86440 / timer.DayLengthInSeconds);
                this.SpeedPxPerSecond = this.SpeedPxPerSecond / env.Definition.MetersPerPixel;
            }

            if (this.SpeedPxPerSecond == 0)
            {
                Log.Error(this.gameObject.name, "Navigation", "Agent immobile as his speed is set to 0");
            }
        }

        public override void StopNavigation()
        {
            this.tsc?.TrySetCanceled();
        }

        public override Task<bool> MoveToDestination(float x, float y) {
            
            this.Navigating = true;
            Log.Debug(this.gameObject.name, "Navigation", $"Moving to destination [{x}, {y}] at {this.SpeedKmH} km/h and {this.SpeedPxPerSecond} px/sec");

            if (this.SpeedPxPerSecond == 0) {
                Log.Warning(this.gameObject.name, "Navigation", "Agent cannot move as its speed is 0");
                return Task.FromResult(false);
            }

            this.destinationX = x;
            this.destinationY = y;

            var distance = Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.y), new Vector2(x, y));
            var time = distance / (this.SpeedPxPerSecond * BehaviourConfiguration.SimulatedDecay);

            this.startTime = DateTime.Now;

            var length = (float) time;
            this.tween = new Tweener(this.transform, this.destinationX, this.destinationY, length);

            this.tsc = new TaskCompletionSource<bool>();
            return tsc.Task;
        }


        public void Update() {
            if (this.tsc == null || this.tsc.Task.IsCanceled) {
                return;
            }

            var xDelta = Math.Abs(this.transform.position.x - this.destinationX);
            var yDelta = Math.Abs(this.transform.position.y - this.destinationY);

            // Debug.WriteLine($"{xDelta} {yDelta}");

            if (xDelta < 0.001 && yDelta < 0.001) {
                Log.Debug(this.gameObject.name, "Navigation", $"Moved to destination [{this.destinationX}, {this.destinationY}]");

                this.tsc.TrySetResult(true);
                this.tsc = null;
                this.Navigating = false;

                // Debug.WriteLine($"Finished at '{(Timer.Now - this.startTime).TotalSeconds:0.00}' ${xDelta:0.0},{yDelta:0.0}");
            } else {
                this.tween.Update(Time.deltaTime);

                Log.Debug(this.gameObject.name, "Navigation", $"Moved to position [{this.transform.X}, {this.transform.Y}]");
            }
        }
    }
}

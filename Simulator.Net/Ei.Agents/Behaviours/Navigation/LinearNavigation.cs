// using Glide;

using System;
using System.ComponentModel;
using Ei.Logs;
using Ei.Simulation.Core;
using UnityEngine;

namespace Ei.Simulation.Behaviours
{
    [DisplayName("Linear Navigation")]
    public class LinearNavigation : NavigationBase //, IUpdates
    {
        private Tweener tween;
        private DateTime startTime;
        public float SpeedPxPerSecond;

        public void Start()
        {
            var env = FindObjectOfType<AgentEnvironment>();
            var timer = FindObjectOfType<SimulationTimer>();  
            var project = FindObjectOfType<SimulationProject>();

            if (this.SpeedPxPerSecond == 0)
            {
                this.SpeedPxPerSecond = this.Speed * // base speed (e.g. 5 km/h = 1.47 m/s)
                    (86440 / timer.DayLengthInSeconds);
                this.SpeedPxPerSecond = this.SpeedPxPerSecond / env.Definition.MetersPerPixel;
            }

            if (this.SpeedPxPerSecond == 0)
            {
                Log.Error(this.gameObject.name, "Agent immobile as his speed is set to 0");
            }
        }

        public override void MoveToDestination(float x, float y) {
            if (this.SpeedPxPerSecond == 0) {
                return;
            }

            this.destinationX = x;
            this.destinationY = y;

            var distance = Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.y), new Vector2(x, y));
            var time = distance / (this.SpeedPxPerSecond * BehaviourConfiguration.SimulatedDecay);

            this.startTime = DateTime.Now;

            var length = (float) time;
            this.tween = new Tweener(this.transform, this.destinationX, this.destinationY, length);

            // this.tweener.Tween(this.Transform, new { X = x, Y = y }, time, 0);

            this.Navigating = true;

            // Debug.WriteLine($"Starting Navigation to {this.destinationX},{this.destinationY}  in {time} distance {distance}");
        }

 

        public void Update() {
            if (!this.Navigating) {
                return;
            }

            var xDelta = Math.Abs(this.transform.position.x - this.destinationX);
            var yDelta = Math.Abs(this.transform.position.y - this.destinationY);

            // Debug.WriteLine($"{xDelta} {yDelta}");

            if (xDelta < 0.001 && yDelta < 0.001) {
                this.Navigating = false;
                this.MovedToDestination();

                // Debug.WriteLine($"Finished at '{(Timer.Now - this.startTime).TotalSeconds:0.00}' ${xDelta:0.0},{yDelta:0.0}");
            } else {
                this.tween.Update(Time.deltaTime);
            }
        }
    }
}

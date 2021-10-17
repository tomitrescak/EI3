using System.ComponentModel;
using UnityEngine;

namespace Ei.Simulation.Behaviours
{
    [DisplayName("Random Navigation")]
    public class RandomNavigation : NavigationBase
    {
        private LinearNavigation navigation;
 
        public int X { get; set; }
        public int Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public override void MoveToDestination(float x, float y)
        {
            this.navigation.MoveToDestination(x,y);
        }

        public void Start() {
            this.navigation = GetComponent<LinearNavigation>();
        }

        public void Update() {
            if (!this.navigation.Navigating) {
                var x = (float) (this.X + UnityEngine.Random.Range(0, this.Width));
                var y = (float) (this.Y + UnityEngine.Random.Range(0, this.Height));

                this.navigation.MoveToDestination(x, y);
            }
        }
    }
}

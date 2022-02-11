using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;

namespace Ei.Simulation.Behaviours
{
    [DisplayName("Random Navigation")]
    public class RandomNavigation : MonoBehaviour
    {
        private LinearNavigation navigation;
 
        public int X { get; set; }
        public int Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public async Task MoveToRandomDestination()
        {
            var x = (float)(this.X + UnityEngine.Random.Range(0, this.Width));
            var y = (float)(this.Y + UnityEngine.Random.Range(0, this.Height));
            await this.navigation.MoveToDestination(x, y);
        }

        public void Start() {
            this.navigation = GetComponent<LinearNavigation>();
        }

        public void Update() {
            if (!this.navigation.Navigating) {
                this.MoveToRandomDestination();
            }
        }
    }
}

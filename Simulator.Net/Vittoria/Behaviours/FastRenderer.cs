using System.Windows.Media;
using System.Windows.Media.Imaging;
using Ei.Simulation.Core;
using UnityEngine;
using Vittoria.Core;

namespace Vittoria.Behaviours
{
    public class FastRenderer : EiBehaviour, IUpdates
    {
        // const int Radius = 6;
        public Color AvatarColor { get; set; }
        public Color SelectedColor { get; set; }
        public int Radius { get; set; }
        private Simulation simulation;
        private ColorProvider colorProvider;

        public FastRenderer() {
            this.AvatarColor = Colors.Yellow;
            this.SelectedColor = Colors.Red;
            this.Radius = 10;
        }

        public void Init() {
            this.simulation = (Simulation) this.gameObject.GetSimulator();
            this.colorProvider = GetComponent<ColorProvider>();        
        }

        public void Update() {
            if (this.colorProvider != null && 
                (this.AvatarColor.R != this.colorProvider.Red || this.AvatarColor.G != this.colorProvider.Green || this.AvatarColor.B != this.colorProvider.Blue)) {
                this.AvatarColor = Color.FromRgb(this.colorProvider.Red, this.colorProvider.Green, this.colorProvider.Blue);
            }
            this.simulation.writeableBmp.FillEllipseCentered(
                (int)this.transform.position.x, 
                (int)this.transform.position.y, 
                Radius, Radius, 
                this.gameObject.Selected ? this.SelectedColor : this.AvatarColor);
            this.simulation.writeableBmp.DrawEllipseCentered((int)this.transform.position.x, (int)this.transform.position.y, Radius, Radius, Colors.Black);
            
        }
    }
}

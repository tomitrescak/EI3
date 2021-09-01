using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Ei.Simulation.Core;
using Ei.Simulation.Sims.Behaviours;
using UnityEngine;
using Vittoria.Core;

namespace Vittoria.Behaviours
{
    public class SimRenderer : EiBehaviour, IUpdates
    {
        // const int Radius = 6;
        public Color AvatarColor { get; set; }
        public Color SelectedColor { get; set; }
        public int Radius { get; set; }
        private Simulation simulation;
        private Sim sim;

        public SimRenderer() {
            this.AvatarColor = Colors.Yellow;
            this.SelectedColor = Colors.Blue;
            this.Radius = 10;
        }

        public void Init() {
            this.simulation = (Simulation)this.gameObject.GetSimulator();
            this.sim = this.GetComponent<Sim>();
        }

        public void Update() {
            // dead avatar is black avatar
            if (this.sim.IsDead) {
                this.AvatarColor = Colors.Black;
                this.simulation.writeableBmp.FillEllipseCentered(
                    (int)this.transform.position.x,
                    (int)this.transform.position.y,
                    Radius + 2, Radius + 2,
                    this.AvatarColor);
                return;
            }
            // draw overall happiness
            this.AvatarColor = this.Interpolate(Colors.Red, Colors.Green, this.sim.Happiness / 100);

            if (this.gameObject.Selected) {
                this.simulation.writeableBmp.FillEllipseCentered(
                (int)this.transform.position.x,
                (int)this.transform.position.y,
                Radius + 5, Radius + 5,
                Colors.LightGray);
            }

            this.simulation.writeableBmp.FillEllipseCentered(
            (int)this.transform.position.x,
            (int)this.transform.position.y,
            Radius + 2, Radius + 2,
            this.AvatarColor);

            // draw wors happiness
            // var worst = this.sim.modifiers.Max(m => m.discomfort) / 100f;
            var worst = (200 - this.sim.modifiers.Min(m => m.XValue + 100)) / 200f;

            var icol = this.InterpolateRedGreen(worst);
            this.simulation.writeableBmp.FillEllipseCentered(
                (int)this.transform.position.x,
                (int)this.transform.position.y,
                Radius - 3, Radius - 3,
                icol);

            this.simulation.writeableBmp.DrawEllipseCentered((int)this.transform.position.x, (int)this.transform.position.y, Radius + 2, Radius + 2, Colors.Black);

        }

        private Color InterpolateRedGreen(float lambda) {
            return Color.FromRgb(
                (byte)(lambda < 0.5 ? lambda * 2 * 255 : 255),
                (byte)(lambda <= 0.5 ? 255 : (255 - (lambda - 0.5) * 2 * 255)),
                0);
        }

        private Color Interpolate(Color start, Color mid, Color end, float lambda) {
            Color a = lambda <= 0.5 ? start : mid;
            Color b = lambda <= 0.5 ? mid : end;
            float value = lambda > 0.5 ? lambda - 0.5f : lambda;

            return Color.FromRgb((byte)((b.R - a.R) * lambda), (byte)((b.G - a.G) * lambda), (byte)((b.B - a.B) * lambda));
        }

        private Color Interpolate(Color start, Color end, float lambda) {
            return Color.FromRgb(
                (byte)(start.R + (end.R - start.R) * lambda),
                (byte)(start.G + (end.G - start.G) * lambda),
                (byte)(start.B + (end.B - start.B) * lambda));
        }
    }
}

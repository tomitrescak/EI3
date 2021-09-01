using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Ei.Simulation.Core;
using Vittoria.Core;

namespace Vittoria.Behaviours
{
    public class LsdRenderer : EiBehaviour
    {
        // const int Radius = 6;
        public Color AvatarColor { get; set; }

        private Simulation simulation;

        static Random randonGen = new Random();

        private int Radius;
        private bool goingUp;
        private int counter;

        private byte colorR;
        private byte colorG;
        private byte colorB;
        private bool colorUp;


        public LsdRenderer() {
            this.colorR = (byte)randonGen.Next(255);
            this.colorG = (byte)randonGen.Next(255);
            this.colorB = (byte)randonGen.Next(255);
            this.colorUp = randonGen.Next() % 2 == 0;

            Color randomColor = Color.FromArgb(this.colorR, this.colorG, this.colorB, 255);

            // this.AvatarColor = Colors.Yellow;
            this.AvatarColor = randomColor;

            this.goingUp = randonGen.Next() % 2 == 0;
            this.Radius = (int)(randonGen.NextDouble() * 15);
        }

        public void Init() {
            this.simulation = ((Simulation) this.gameObject.GetSimulator());
        }

        public void Update() {
            //if (this.lastX != this.Transform.X) {
            //    Canvas.SetLeft(this.Avatar, this.Transform.X);
            //    this.lastX = this.Transform.X;
            //}

            //if (this.lastY != this.Transform.Y) {
            //    Canvas.SetTop(this.Avatar, this.Transform.Y);
            //    this.lastY = this.Transform.Y;
            //}

            // RANDOM STUFF

            if (randonGen.Next() % 4 == 0) {
                this.Radius = this.Radius + (this.goingUp ? 1 : -1);
                if (this.Radius >= 20) {
                    this.goingUp = false;
                }
                if (this.Radius <= 3) {
                    this.goingUp = true;
                }
                this.counter = 0;

                byte inc = (byte)(this.colorUp ? 1 : -1);
                this.colorR += inc;
                this.colorG += inc;
                this.colorB += inc;

                if (this.colorR == 0 || this.colorG == 0 || this.colorB == 0) {
                    this.colorUp = true;
                }
                if (this.colorR == 255 || this.colorG == 255 || this.colorB == 255) {
                    this.colorUp = false;
                }
                Color randomColor = Color.FromArgb(this.colorR, this.colorG, this.colorB, 255);
                this.AvatarColor = randomColor;
            }

            this.simulation.writeableBmp.FillEllipseCentered((int)this.transform.position.x, (int)this.transform.position.y, Radius, Radius, this.AvatarColor);

            if (this.gameObject.Selected) {
                this.simulation.writeableBmp.DrawEllipseCentered((int)this.transform.position.x, (int)this.transform.position.y, Radius, Radius, Colors.Red);
            } else {
                this.simulation.writeableBmp.DrawEllipseCentered((int)this.transform.position.x, (int)this.transform.position.y, Radius, Radius, Colors.Black);
            }
        }
    }
}

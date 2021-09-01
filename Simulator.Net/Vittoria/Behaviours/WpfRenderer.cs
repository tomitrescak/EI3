using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ei.Simulation.Core;
using Vittoria.Core;

namespace Vittoria.Behaviours
{
    public class WpfRenderer : EiBehaviour
    {
        [Browsable(false)]
        public Border Avatar { get; private set; }
        public Color AvatarColor { get; set; }

        private float lastX = -1;
        private float lastY = -1;

        private Canvas canvas;

        public WpfRenderer() {
        }

        public void Init() {
            this.canvas = ((Simulation) this.gameObject.GetSimulator()).Canvas;

            // init controls
            this.AvatarColor = Colors.Yellow;
            this.Avatar = new Border();
            this.Avatar.Padding = new Thickness(1);
            this.Avatar.BorderBrush = new SolidColorBrush(Colors.Black);
            this.Avatar.BorderThickness = new Thickness(2);
            this.Avatar.Background = new SolidColorBrush(this.AvatarColor);
            this.Avatar.CornerRadius = new CornerRadius(5);

            var text = new TextBlock();
            text.Text = this.gameObject.name;
            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.FontSize = 10;

            this.Avatar.Child = text;

            //            // Create a SolidColorBrush with a red color to fill the 
            //            // Ellipse with.
            //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            //
            //            // Describes the brush's color using RGB values. 
            //            // Each value has a range of 0-255.
            //            mySolidColorBrush.Color = AvatarColor;
            //            this.Avatar.Fill = mySolidColorBrush;
            //            this.Avatar.StrokeThickness = 2;
            //            this.Avatar.Stroke = Brushes.Black;

            // Set the width and height of the Ellipse.
            this.Avatar.Width = 20;
            this.Avatar.Height = 20;

            this.canvas.Children.Add(this.Avatar);

            Canvas.SetLeft(this.Avatar, this.transform.position.x);
            Canvas.SetTop(this.Avatar, this.transform.position.y);
        }

        public void Start() {
            Canvas.SetLeft(this.Avatar, this.transform.position.x);
            Canvas.SetTop(this.Avatar, this.transform.position.y);
        }

        public void Update() {
            if (this.lastX != this.transform.position.x) {
                Canvas.SetLeft(this.Avatar, this.transform.position.x);
                this.lastX = (float) this.transform.position.x;
            }

            if (this.lastY != this.transform.position.y) {
                Canvas.SetTop(this.Avatar, this.transform.position.y);
                this.lastY = (float) this.transform.position.y;
            }
        }
    }
}

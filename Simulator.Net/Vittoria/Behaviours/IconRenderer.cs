using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Ei.Simulation.Core;
using Ei.Simulation.Sims.Behaviours;
using UnityEngine;
using Vittoria.Core;

namespace Vittoria.Behaviours
{
    public class IconRenderer : EiBehaviour, IUpdates
    {
        // const int Radius = 6;
        private string icon;
        private Simulation simulation;
        Bitmap bitmap;
        BitmapData data;

        public void Init() {
            this.simulation = (Simulation)this.gameObject.GetSimulator();

            this.icon = this.GetComponent<SimObject>().Icon;

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data", icon).Replace("/", "\\");
            if (!File.Exists(path)) {
                throw new Exception("File does not exists: " + path);
            }

            bitmap = new Bitmap(path); // (Bitmap) System.Drawing.Image.FromFile(path);
            data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        public void Update() {
            this.simulation.writeableBmp.WritePixels(
                new Int32Rect(0, 0, this.bitmap.Width, this.bitmap.Height),
                this.data.Scan0,
                1000000,
                this.data.Stride,
                (int) Mathf.Clamp(this.transform.position.x - this.bitmap.Width / 2, 0, (int) this.simulation.writeableBmp.Width),
                (int) Mathf.Clamp(this.transform.position.y - this.bitmap.Height / 2, 0, (int)this.simulation.writeableBmp.Height));
        }
    }

    static class BitmapExtensions
    {
        public static void SetAlpha(this Bitmap bmp, byte alpha) {
            if (bmp == null) throw new ArgumentNullException("bmp");

            var data = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var line = data.Scan0;
            var eof = line + data.Height * data.Stride;
            while (line != eof) {
                var pixelAlpha = line + 3;
                var eol = pixelAlpha + data.Width * 4;
                while (pixelAlpha != eol) {
                    System.Runtime.InteropServices.Marshal.WriteByte(
                        pixelAlpha, alpha);
                    pixelAlpha += 4;
                }
                line += data.Stride;
            }
            bmp.UnlockBits(data);
        }
    }
}

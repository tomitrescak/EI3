using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEngine
{
    public struct Vector3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public Vector3(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static double Distance(Vector3 value1, Vector3 value2) {
            double v1 = value1.x - value2.x, v2 = value1.y - value2.y;
            return (double)Math.Sqrt((v1 * v1) + (v2 * v2));
        }
    }
}

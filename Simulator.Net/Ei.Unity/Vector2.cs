using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEngine
{
    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y) {
            this.x = x;
            this.y = y;
        }

        // methods
    
        public static double Distance(Vector2 value1, Vector2 value2) {
            double v1 = value1.x - value2.x, v2 = value1.y - value2.y;
            return (double)Math.Sqrt((v1 * v1) + (v2 * v2));
        }


        public static double DistanceSquared(Vector2 value1, Vector2 value2) {
            double v1 = value1.x - value2.x, v2 = value1.y - value2.y;
            return (v1 * v1) + (v2 * v2);
        }
    }
}

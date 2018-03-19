using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEngine
{
    public static class Random
    {
        public static System.Random random = new System.Random();

        public static float Range(float min, float max) {
            return min + (float) (random.NextDouble() * (max - min));
        }

        public static int Range(int min, int max) {
            return min +  (int) (random.NextDouble() * (max - min));
        }

    }
}

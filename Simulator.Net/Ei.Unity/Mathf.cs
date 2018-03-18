using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEngine
{
    public static class Mathf
    {
        public static float Clamp(float value, float min, float max) {
            return value <= min ? min : value >= max ? max : value;
        }
    }
}

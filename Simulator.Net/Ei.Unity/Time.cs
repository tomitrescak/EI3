using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace UnityEngine
{
    
    public static class Time
    {
        static DateTime timerStart;
        static Stopwatch globalTimer;
        static Stopwatch fpsTimer;

        public static float deltaTime { get; private set; }
        public static float time { get; private set; }
        public static int Fps { get; private set; }
        // methods

        static Time() {
            globalTimer = new Stopwatch();
            fpsTimer = new Stopwatch();
        }

        public static void Start() {
            time = 0;
            timerStart = DateTime.Now;
            globalTimer.Reset();
            globalTimer.Start();
            fpsTimer.Start();
        }

        public static float Elapsed(DateTime currentTime) {
            return (float) (currentTime - timerStart).TotalMilliseconds / 1000f;
        }


        static int frames = 0;
        static float total;

        static float lastSeconds = 0;
 
        public static void FrameEnd() {
            
            time = (float) globalTimer.Elapsed.TotalSeconds;
            deltaTime = time - lastSeconds;
            lastSeconds = time;

            total += deltaTime;

            frames++;
            
            // Console.WriteLine("Frame {0} at {1}", frames, time);
            
            if (fpsTimer.ElapsedMilliseconds >= 1000) {
                // Console.WriteLine("Ending frame at: " + time + ": " + Fps);
                
                Fps = frames;
                frames = 0;
                fpsTimer.Reset();
                fpsTimer.Start();

            }

            

            // Debug.WriteLine($"{DeltaTime} sec {Fps} Fps");
        }
    }
}

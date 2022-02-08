using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;

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

        private static StreamWriter sw;
        
        static Time() {
            globalTimer = new Stopwatch();
            fpsTimer = new Stopwatch();
            
            // log file
            // This text is added only once to the file.
            sw = File.CreateText("log.txt");
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
 
        // calculation variables
        const int DesiredFps = 121;
        static float currentFrames;
        static float averageFrame;
        static float expectedFrame;
 
        public static void FrameEnd() {
            
            // do the temporary calculation 
            time = (float) globalTimer.Elapsed.TotalSeconds;
            deltaTime = time - lastSeconds;
            frames++;
            
            // Console.WriteLine("Frame {0} at {1}", frames, time);
            
            // adaptable framerate adjustment
            // postpone if needed
            currentFrames += deltaTime;
            averageFrame = currentFrames / frames;
            expectedFrame = DesiredFps == frames ? 1 : ((1000f - fpsTimer.ElapsedMilliseconds) / (DesiredFps - frames));
            
            // sw.WriteLine("Frame: {0}, Elapsed: {1}, Average: {2}, ExpectedFrame: {3}, Sleep: {4}", frames, fpsTimer.ElapsedMilliseconds, averageFrame, expectedFrame, expectedFrame - deltaTime);
            
            // only sleep if expected frame is bigger then last frame
            if ((int)(expectedFrame - deltaTime) > 0) {
                Thread.Sleep((int) (expectedFrame - deltaTime));
            }
            
            // reset is needed
            if (fpsTimer.ElapsedMilliseconds >= 999) {
                // Console.WriteLine("Ending frame at: " + time + ": " + Fps);      
                // sw.WriteLine("============= " + frames);
                // sw.Flush();
                Fps = frames;
                frames = 0;
                fpsTimer.Restart();
            }
            
            
            // do the final calculation
            time = (float) globalTimer.Elapsed.TotalSeconds;
            deltaTime = time - lastSeconds;
            total += deltaTime;
            lastSeconds = time;

        }
    }
}

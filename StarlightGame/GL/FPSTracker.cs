using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace StarlightGame.GL
{
    internal class FPSTracker
    {
        private Stopwatch stopwatch;
        private List<double> fpsList;
        private double totalTime;
        private int frameCount;

        public FPSTracker()
        {
            stopwatch = new Stopwatch();
            fpsList = new List<double>();
            totalTime = 0;
            frameCount = 0;
            stopwatch.Start();
        }

        public void Update(double deltaTime)
        {
            totalTime += deltaTime;
            frameCount++;
            if (totalTime >= 1.0)
            {
                double fps = frameCount / totalTime;
                fpsList.Add(fps);
                if (fpsList.Count > 60) fpsList.RemoveAt(0);
                double averageFps = fpsList.Average();
                Console.WriteLine($"FPS: {fps:F2}, Average FPS: {averageFps:F2}");
                totalTime = 0;
                frameCount = 0;
            }
        }
    }
}
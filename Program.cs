using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        int totalCircles = 1000; // Total number of circles
        int paintingDelayMs = 20; // Delay for painting a circle (in milliseconds)
        List<int> workerCounts = new List<int> { 5, 20, 100 }; // Different numbers of workers to simulate

        // Generate random circle coordinmates
        var circles = GenerateCircles(totalCircles);

        // Simulate painting for each worker count
        foreach (var workerCount in workerCounts)
        {
            Console.WriteLine($"\nSimulating with {workerCount} workers:");
            var stopwatch = Stopwatch.StartNew();

            SimulatePainting(circles, workerCount, paintingDelayMs);

            stopwatch.Stop();
            Console.WriteLine($"All circles painted with {workerCount} workers in {stopwatch.ElapsedMilliseconds} ms.");
        }
    }

    // Generates random circle coordinates
    static List<(double x, double y)> GenerateCircles(int count)
    {
        Random random = new Random();
        var circles = new List<(double x, double y)>();
        for (int i = 0; i < count; i++)
        {
            circles.Add((random.NextDouble() * 100, random.NextDouble() * 100));
        }
        return circles;
    }

    // Simulates the painting process
    static void SimulatePainting(List<(double x, double y)> circles, int workerCount, int paintingDelayMs)
    {
        int totalCircles = circles.Count;
        var completedCircles = new HashSet<int>();
        var locker = new object();

        var tasks = new List<Task>();
        for (int i = 0; i < workerCount; i++)
        {
            int workerId = i;
            tasks.Add(Task.Run(() =>
            {
                while (true)
                {
                    int circleToPaint = -1;

                    lock (locker)
                    {
                        for (int j = 0; j < totalCircles; j++)
                        {
                            if (!completedCircles.Contains(j))
                            {
                                completedCircles.Add(j);
                                circleToPaint = j;
                                break;
                            }
                        }
                    }

                    if (circleToPaint == -1) break; // Stop if no circles left

                    // Simulate painting the circle
                    Console.WriteLine($"Worker {workerId + 1} is painting circle {circleToPaint + 1}.");
                    Thread.Sleep(paintingDelayMs);
                }
            }));
        }

        // Wait for all workers to finish
        Task.WaitAll(tasks.ToArray());
    }
}

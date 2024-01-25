using System;
using UnityEngine;
using System.Diagnostics;
using Unity.Burst;

public class PerformanceTester : MonoBehaviour
{
    void Start()
    {
        Stopwatch stopwatch = new Stopwatch();

        stopwatch.Start();
//        BFSTest();
        stopwatch.Stop();
        
        TimeSpan timeTaken = stopwatch.Elapsed;
        UnityEngine.Debug.Log("BFS took: " + timeTaken.ToString(@"m\:ss\.fff"));

        stopwatch.Reset();
        stopwatch.Start();
//        FastTest();
        stopwatch.Stop();
        
        timeTaken = stopwatch.Elapsed;
        UnityEngine.Debug.Log("Fast took: " + timeTaken.ToString(@"m\:ss\.fff"));
    }
}

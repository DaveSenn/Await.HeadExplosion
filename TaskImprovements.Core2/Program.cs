using System;
using BenchmarkDotNet.Running;

internal class Program
{
    private static void Main( String[] args )
    {
        var summary = BenchmarkRunner.Run<TaskImprovements>();
    }
}
using System;
using System.IO;

static class UnwrappingExtensions 
{
    public static void PrintStartProxy(this Unwrapping runnable, int id) 
    {
        Console.WriteLine($"start proxy {id}");
    }

    public static void PrintEndProxy(this Unwrapping runnable, int id) 
    {
        Console.WriteLine($"end proxy {id}");
    }

    public static void PrintStartActual(this Unwrapping runnable, int id) 
    {
        Console.WriteLine($"start actual {id}");
    }  

    public static void PrintEndActual(this Unwrapping runnable, int id) 
    {
        Console.WriteLine($"end actual {id}");
    }

    public static void Explain(this Unwrapping runnable, TextWriter writer)
    {
        writer.WriteLine(" - Async in `Task.Factory.StartNew` returns a proxy task `Task<Task>`");
        writer.WriteLine(" - Proxy task is completed before the actual task is completed");
    }            
}
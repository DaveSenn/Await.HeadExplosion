using System;
using System.IO;
using System.Threading;

internal static class ConcurrentExecutionExtensions
{
    public static void Explain( this ConcurrentExecution runnable, TextWriter writer )
    {
        writer.WriteLine( @"
- `Task.WhenAll` materializes enumerable
- Tasks are executed concurrently
- `Task.WhenAll` task is done when all done
- When one task threw an exception the task is faulted and the exception rethrown
- When interested in the outcome of each task you have to loop over the tasks
" );
    }

    public static void PrintEnd( this ConcurrentExecution runnable, Int32 element )
    {
        Console.WriteLine( $"done {element} / {Thread.CurrentThread.ManagedThreadId}" );
    }

    public static void PrintStart( this ConcurrentExecution runnable, Int32 element )
    {
        Console.WriteLine( $"start {element} / {Thread.CurrentThread.ManagedThreadId}" );
    }
}
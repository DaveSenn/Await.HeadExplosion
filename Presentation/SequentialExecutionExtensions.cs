using System;
using System.IO;
using System.Threading;

internal static class SequentialExecutionExtensions
{
    public static void Explain( this SequentialExecution runnable, TextWriter writer )
    {
        writer.WriteLine( @"
- `Task.Delay` represents a true asynchronous operation, no offloading needed
- Protip: `Task.Delay` uses a timer and is subjected to timer resolution on the system
- Lazy nature of enumerable creates tasks when iterating
- `Await` means sequentialize here
" );
    }

    public static void PrintEnd( this SequentialExecution runnable, Int32 element )
    {
        Console.WriteLine( $"done {element} / {Thread.CurrentThread.ManagedThreadId}" );
    }

    public static void PrintStart( this SequentialExecution runnable, Int32 element )
    {
        Console.WriteLine( $"start {element} / {Thread.CurrentThread.ManagedThreadId}" );
    }
}
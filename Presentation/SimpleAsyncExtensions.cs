using System;
using System.IO;
using System.Threading;

internal static class SimpleAsyncExtensions
{
    public static void Explain( this SimpleAsync runnable, TextWriter writer )
    {
        writer.WriteLine( @"
- Every `await` statement is a chance for the calling thread to do something else
- Much more efficient due to less thread usage
- Can achieve higher saturation of ressources available
" );
    }

    public static void PrintEnd( this SimpleAsync runnable )
    {
        Console.WriteLine( $"done ManagedThreadId: {Thread.CurrentThread.ManagedThreadId}" );
    }

    public static void PrintStart( this SimpleAsync runnable )
    {
        Console.WriteLine( $"start ManagedThreadId: {Thread.CurrentThread.ManagedThreadId}" );
    }
}
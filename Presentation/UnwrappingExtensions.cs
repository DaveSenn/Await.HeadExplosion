using System;
using System.IO;

internal static class UnwrappingExtensions
{
    public static void Explain( this Unwrapping runnable, TextWriter writer )
    {
        writer.WriteLine( @"
- Async in `Task.Factory.StartNew` returns a proxy task `Task<Task>`
- `Task.Run` doesn't have this problem that's why it is your friend
- Proxy task is completed before the actual task is completed
- Can lead to interesting bugs (seen in the wild many times)
" );
    }

    public static void PrintEndActual( this Unwrapping runnable, Int32 id )
    {
        Console.WriteLine( $"end actual {id}" );
    }

    public static void PrintEndProxy( this Unwrapping runnable, Int32 id )
    {
        Console.WriteLine( $"end proxy {id}" );
    }

    public static void PrintStartActual( this Unwrapping runnable, Int32 id )
    {
        Console.WriteLine( $"start actual {id}" );
    }

    public static void PrintStartProxy( this Unwrapping runnable, Int32 id )
    {
        Console.WriteLine( $"start proxy {id}" );
    }
}
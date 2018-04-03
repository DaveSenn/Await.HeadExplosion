using System.IO;

internal static class AsyncAllTheWayExtensions
{
    public static void Explain( this AsyncAllTheWay runnable, TextWriter writer )
    {
        writer.WriteLine( @"
- It is OK to call sync code from async context
- It defeats the purpose of async when async is called from sync code
- It is dangerous to call async code from sync code, it can lead to deadlocks.
" );
    }
}
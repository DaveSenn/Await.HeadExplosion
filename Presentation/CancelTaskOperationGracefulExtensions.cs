using System.IO;

internal static class CancelTaskOperationGracefulExtensions
{
    public static void Explain( this CancelTaskOperationGraceful runnable, TextWriter writer )
    {
        writer.WriteLine( @"
- It is up to the implementor to decide whether exceptions should be observed by the caller
- For graceful shutdown scenarios the root task should not transition into 'canceled'
" );
    }
}
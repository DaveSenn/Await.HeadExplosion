using System.IO;

internal static class CancelTaskExtensions
{
    public static void Explain( this CancelTask runnable, TextWriter writer )
    {
        writer.WriteLine( @"
- Passing a token to a task does only impact the final state of the task
- Cancellation is a cooperative effort
" );
    }
}
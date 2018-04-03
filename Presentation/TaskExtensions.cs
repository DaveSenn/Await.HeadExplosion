using System;
using System.Threading.Tasks;

internal static class TaskExtensions
{
    public static void Ignore( this Task task )
    {
    }

    public static async Task IgnoreCancellation( this Task task )
    {
        try
        {
            await task.ConfigureAwait( false );
        }
        catch ( OperationCanceledException )
        {
        }
    }
}
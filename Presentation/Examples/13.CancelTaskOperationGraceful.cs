using System;
using System.Threading;
using System.Threading.Tasks;

[Order( 13 )]
internal class CancelTaskOperationGraceful : IRunnable
{
    public Task Run()
    {
        var tokenSource = new CancellationTokenSource();
        tokenSource.CancelAfter( TimeSpan.FromSeconds( 3 ) );
        var token = tokenSource.Token;

        return Task.Delay( TimeSpan.FromMinutes( 1 ), token )
                   .IgnoreCancellation();
    }
}
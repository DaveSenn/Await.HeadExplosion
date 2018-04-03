using System;
using System.Threading;
using System.Threading.Tasks;

[Order( 5 )]
internal class AsyncAllTheWay : IRunnable
{
    #region Constants

    private static readonly LimitedConcurrencyLevelTaskScheduler Scheduler = new LimitedConcurrencyLevelTaskScheduler( 1 );

    #endregion

    public Task Run()
        => WrapInContext( Method );

    private void Method()
    {
        if ( !MethodAsync()
            .Wait( 1000 ) )
            throw new TimeoutException( "Timed out after deadlock." );
    }

    private async Task MethodAsync()
    {
        await Task.Delay( 200 );
    }

    private static Task WrapInContext( Action action )
        => Task.Factory.StartNew( action, CancellationToken.None, TaskCreationOptions.None, Scheduler );
}
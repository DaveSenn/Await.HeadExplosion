using System;
using System.Threading;
using System.Threading.Tasks;

[Order( 17 )]
internal class ConcurrencyLimit : IRunnable
{
    #region Constants

    private const Int32 maxConcurrency = 5;

    #endregion

    public async Task Run()
    {
        var semaphore = new SemaphoreSlim( maxConcurrency, maxConcurrency );
        var pumpTask = Task.Run( async () =>
        {
            var token = this.TokenThatCancelsAfterTwoSeconds();
            var workCount = 0;

            while ( !token.IsCancellationRequested )
            {
                await semaphore.WaitAsync( token );

                var runningTask = this.SimulateWorkThatTakesOneSecond( workCount++ );

                runningTask
                    .ContinueWith( ( t, state ) =>
                                   {
                                       var s = (SemaphoreSlim) state;
                                       s.Release();
                                   },
                                   semaphore,
                                   TaskContinuationOptions.RunContinuationsAsynchronously )
                    .Ignore();
            }
        } );

        await pumpTask.IgnoreCancellation();

        await WaitForPendingWork( semaphore );
    }

    private async Task WaitForPendingWork( SemaphoreSlim semaphore )
    {
        while ( semaphore.CurrentCount != maxConcurrency )
            await Task.Delay( 50 );
    }
}
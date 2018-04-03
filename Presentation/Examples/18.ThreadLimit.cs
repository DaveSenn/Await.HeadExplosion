using System;
using System.Threading;
using System.Threading.Tasks;

[Order( 18 )]
internal class ThreadLimit : IRunnable
{
    public async Task Run()
    {
        await LimitingThreads( TaskCreationOptions.None, false );
        Console.WriteLine( "\n\n" );
        await LimitingThreads( TaskCreationOptions.None, true );
        Console.WriteLine( "\n\n" );
        await LimitingThreads( TaskCreationOptions.HideScheduler, false );
        Console.WriteLine( "\n\n" );
        await LimitingThreads( TaskCreationOptions.HideScheduler, true );
    }

    private Task LimitingThreads( TaskCreationOptions options, Boolean configureAwait )
    {
        this.PrintOptions( options, configureAwait );

        var scheduler = new LimitedConcurrencyLevelTaskScheduler( 1 );
        return this.PumpWithSemaphoreConcurrencyTwo( ( current, token )
                                                         => Task
                                                            .Factory
                                                            .StartNew( () => WorkUnderSpecialScheduler( configureAwait, current ), CancellationToken.None, options, scheduler )
                                                            .Unwrap() );
    }

    private Task WorkUnderSpecialScheduler( Boolean configureAwait, Int32 current, CancellationToken token = default )
    {
        var startNewTask = Task.Factory.StartNew( async () =>
        {
            this.PrintStartNewBefore( current );

            await Task.Delay( 1000, token )
                      .ConfigureAwait( configureAwait );

            this.PrintStartNewAfter( current );
        } );

        var runTask = Task.Run( async () =>
        {
            this.PrintRunBefore( current );

            await Task.Delay( 1000, token )
                      .ConfigureAwait( configureAwait );

            this.PrintRunAfter( current );
        } );

        return Task.WhenAll( startNewTask.Unwrap(), runTask );
    }
}
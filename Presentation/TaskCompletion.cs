using System;
using System.Threading.Tasks;

[Order( 19 )]
internal class TaskCompletion : IRunnable
{
    public async Task Run()
    {
        var taskCompletionSource = new TaskCompletionSource<Boolean>();

        var simulator = new Simulator();
        simulator.Fired += ( sender, args ) => taskCompletionSource.TrySetResult( true );
        this.PrintStart();
        simulator.Start();

        await taskCompletionSource.Task.ConfigureAwait( false );

        this.PrintEnd();
    }
}
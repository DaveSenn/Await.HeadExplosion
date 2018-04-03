using System.Threading;
using System.Threading.Tasks;

[Order( 11 )]
internal class CancelTask : IRunnable
{
    public Task Run()
    {
        return Task.Run( () => { }, new CancellationToken( true ) );
    }
}
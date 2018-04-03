using System;
using System.Threading.Tasks;

[Order( 5 )]
internal class AsyncAllTheWay : IRunnable
{
    public Task Run()
    {
        return this.WrapInContext( () =>
                                       Method()
        );
    }

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
}
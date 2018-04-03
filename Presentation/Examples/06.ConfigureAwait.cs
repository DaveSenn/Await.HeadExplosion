using System;
using System.Threading.Tasks;

[Order( 6 )]
internal class ConfigureAwait : IRunnable
{
    public Task Run()
    {
        return this.WrapInContext( async () =>
        {
            this.PrintStart();
            await Method();
            this.PrintEnd();
            Console.WriteLine( "\n\n" );

            this.PrintStart();
            await Method()
                .ConfigureAwait( false );
            this.PrintEnd();
        } );
    }

    private async Task Method()
    {
        this.PrintBeforeDelay();
        
        await Task.Delay( 100 )
                  .ConfigureAwait( false );
        this.PrintAfterDelayConfigureAwait();
        
        await Task.Delay( 100 );
        this.PrintAfterDelay();
    }
}
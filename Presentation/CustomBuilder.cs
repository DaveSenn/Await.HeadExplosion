using System;
using System.Threading.Tasks;

[Order( 23 )]
internal class CustomBuilder : IRunnable
{
    public async Task Run()
    {
        var result = await Calculate();
        this.PrintResult( result );
    }

    private static async Taskk<Int32> Calculate()
    {
        var value = 0;
        value += await GetValue()
            .ConfigureAwait( false );
        value += await GetValue()
            .ConfigureAwait( false );
        value += await GetValue()
            .ConfigureAwait( false );
        return value;
    }

    private static async Taskk<Int32> GetValue()
    {
        await Task.Delay( 1 );
        return 1;
    }
}
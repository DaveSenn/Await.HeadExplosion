using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[Order( 21 )]
internal class CustomAwaiter : IRunnable
{
    public async Task Run()
    {
        await 1000;
        await TimeSpan.FromSeconds( 1 );
    }
}

public static class CustomTaskExtensions
{
    public static TaskAwaiter GetAwaiter( this Int32 millisecondsDelay ) => Task.Delay( millisecondsDelay )
                                                                                .GetAwaiter();

    public static TaskAwaiter GetAwaiter( this TimeSpan delay ) => Task.Delay( delay )
                                                                       .GetAwaiter();
}
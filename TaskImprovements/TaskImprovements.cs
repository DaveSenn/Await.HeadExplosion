using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

[Config( typeof(Config) )]
public class TaskImprovements
{
    [Benchmark]
    public async Task Actions()
    {
        await SomeMethod();
    }

    [Benchmark( Baseline = true )]
    public async Task Return()
    {
        await DoWorkReturn();
    }

    [Benchmark]
    public async Task Simple()
    {
        await DoWork();
    }

    private static async Task DoWork()
    {
        await Task.Delay( 1 );
    }

    private static Task DoWorkReturn() => Task.Delay( 1 );

    private static async Task SomeMethod()
    {
        await Task.Run( () => Thread.Sleep( 1 ) );
    }
}
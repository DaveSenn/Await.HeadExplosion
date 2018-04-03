using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

// From https://github.com/adamsitnik/StateOfTheDotNetPerformance/blob/master/ValueTaskOverheadBenchmarks.cs
// All credit go to Adam Sitnik
[MemoryDiagnoser]
[ShortRunJob]
public class ValueTaskOverheadBenchmarks
{
    #region Properties

    [Params( 100, 1000 )]
    public Int32 Repeats { get; set; }

    #endregion

    [Benchmark]
    public Task<Int32> ConsumeTask() => ConsumeTask( Repeats );

    [Benchmark]
    public ValueTask<Int32> ConsumeValueTaskCrazy() => ConsumeCrazy( Repeats );

    [Benchmark( Baseline = true )]
    public ValueTask<Int32> ConsumeValueTaskProperly() => ConsumeProperly( Repeats );

    [Benchmark]
    public ValueTask<Int32> ConsumeValueTaskWrong() => ConsumeWrong( Repeats );

    private ValueTask<Int32> ConsumeCrazy( Int32 repeats )
    {
        var total = 0;
        while ( repeats-- > 0 )
        {
            var valueTask = SampleUsage(); // INLINEABLE

            if ( valueTask.IsCompleted )
                total += valueTask.Result;
            else
                return ContinueAsync( valueTask, repeats, total );
        }

        return new ValueTask<Int32>( total );
    }

    private async ValueTask<Int32> ConsumeProperly( Int32 repeats )
    {
        var total = 0;
        while ( repeats-- > 0 )
        {
            var valueTask = SampleUsage(); // INLINEABLE

            total += valueTask.IsCompleted
                ? valueTask.Result
                : await valueTask.AsTask();
        }

        return total;
    }

    private async Task<Int32> ConsumeTask( Int32 repeats )
    {
        var total = 0;
        while ( repeats-- > 0 )
            total += await SampleUsageAsync();

        return total;
    }

    private async ValueTask<Int32> ConsumeWrong( Int32 repeats )
    {
        var total = 0;
        while ( repeats-- > 0 )
            total += await SampleUsage();

        return total;
    }

    private async ValueTask<Int32> ContinueAsync( ValueTask<Int32> valueTask, Int32 repeats, Int32 total )
    {
        total += await valueTask;

        while ( repeats-- > 0 )
        {
            valueTask = SampleUsage();

            if ( valueTask.IsCompleted )
                total += valueTask.Result;
            else
                total += await valueTask;
        }

        return total;
    }

    private Task<Int32> ExecuteAsync() => Task.FromResult( 1 );

    private Int32 ExecuteSynchronous() => 1;

    [MethodImpl( MethodImplOptions.NoInlining )]
    private Boolean IsFastSynchronousExecutionPossible() => true;

    [MethodImpl( MethodImplOptions.AggressiveInlining )] // super important!
    private ValueTask<Int32> SampleUsage()
        => IsFastSynchronousExecutionPossible()
            ? new ValueTask<Int32>(ExecuteSynchronous() ) // INLINEABLE!!!
            : new ValueTask<Int32>(ExecuteAsync() );

    private Task<Int32> SampleUsageAsync() => Task.FromResult( 1 );
}
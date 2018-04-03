using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

[Order( 20 )]
internal class ValueTasks : IRunnable
{
    #region Fields

    internal ConcurrentDictionary<String, Int32> cachedValues = new ConcurrentDictionary<String, Int32>();

    #endregion

    public Task Run()
    {
        return this.LoopTenTimesAndSumResult( async i =>
        {
            var valueTask = Get( "Foo" );

            if ( valueTask.IsCompleted )
            {
                this.PrintFastPath( i );
                return valueTask.Result;
            }

            this.PrintAsyncPath( i );
            return await valueTask.AsTask();
        } );
    }

    private ValueTask<Int32> Get( String key )
    {
        if ( cachedValues.TryGetValue( key, out var value ) )
            return new ValueTask<Int32>( value );

        return new ValueTask<Int32>( this.LoadFromFileAndCache( key ) );
    }
}
﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;

[Order( 9 )]
public class ParallelExecution : IRunnable
{
    public Task Run()
    {
        var concurrent = Enumerable.Range( 0, 20 )
                                   .Select( t => Task.Run( () =>
                                   {
                                       this.PrintStart( t );

                                       Thread.Sleep( 1500 );

                                       this.PrintEnd( t );
                                   } ) );

        return Task.WhenAll( concurrent );
    }
}
using System.Linq;
using System.Threading.Tasks;

[Order( 8 )]
internal class ConcurrentExecution : IRunnable
{
    public Task Run()
    {
        var concurrent = Enumerable.Range( 0, 20 )
                                   .Select( async t =>
                                   {
                                       this.PrintStart( t );

                                       await Task.Delay( 1500 );

                                       this.PrintEnd( t );
                                   } );

        return Task.WhenAll( concurrent );
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;

internal class Simulator
{
    public event EventHandler Fired = delegate { };

    public void Start()
    {
        Task.Delay( 500 )
            .ContinueWith( t => OnFired() );
    }

    private void OnFired()
    {
        Console.WriteLine( $"Fire on {Thread.CurrentThread.ManagedThreadId}" );
        Fired( this, EventArgs.Empty );
    }
}
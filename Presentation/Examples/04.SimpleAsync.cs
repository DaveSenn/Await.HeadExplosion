using System.Threading.Tasks;

[Order( 4 )]
internal class SimpleAsync : IRunnable
{
    public async Task Run()
    {
        this.PrintStart();
        await Task.Delay( 1000 );
        this.PrintEnd();
    }
}
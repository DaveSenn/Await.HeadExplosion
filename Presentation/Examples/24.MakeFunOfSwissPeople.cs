using System.Threading.Tasks;

[Order( 24 )]
internal class MakeFunOfSwissPeople : IRunnable
{
    public Task Run()
    {
        this.PrintJoke();
        return Task.CompletedTask;
    }
}
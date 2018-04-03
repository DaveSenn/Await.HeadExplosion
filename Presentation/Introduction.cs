using System.Threading.Tasks;

[Order( -1 )]
internal class Introduction : IRunnable
{
    public Task Run()
    {
        this.PrintStart();
        this.PrintEnd();
        return Task.CompletedTask;
    }
}
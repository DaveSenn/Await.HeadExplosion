using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

internal static class ConfigureAwaitExtensions
{
    #region Constants

    private static readonly LimitedConcurrencyLevelTaskScheduler scheduler = new LimitedConcurrencyLevelTaskScheduler( 1 );

    #endregion

    public static void Explain( this ConfigureAwait runnable, TextWriter writer )
    {
        writer.WriteLine( @"
- `ConfigureAwait` controls whether context capturing is enabled
- Context capturing as a simplification can be understood as restoring the TaskScheduler that was visible before the `await`
- Context capturing affects the continuation of an asynchronous method

```
await Method().ConfigureAwait(true|false);
await Continuation(); // <-- affected by line above
```
" );
    }

    public static void PrintAfterDelay( this ConfigureAwait runnable )
    {
        var visible = TaskScheduler.Current == scheduler ? "visible" : "not visible";
        Console.WriteLine( $"  after delay context is {visible}" );
    }

    public static void PrintAfterDelayConfigureAwait( this ConfigureAwait runnable )
    {
        var visible = TaskScheduler.Current == scheduler ? "visible" : "not visible";
        Console.WriteLine( $"  after delay with configure await context is {visible}" );
    }

    public static void PrintBeforeDelay( this ConfigureAwait runnable )
    {
        var visible = TaskScheduler.Current == scheduler ? "visible" : "not visible";
        Console.WriteLine( $"  before delay context is {visible}" );
    }

    public static void PrintEnd( this ConfigureAwait runnable )
    {
        var visible = TaskScheduler.Current == scheduler ? "visible" : "not visible";
        Console.WriteLine( $"done context is {visible}" );
    }

    public static void PrintStart( this ConfigureAwait runnable )
    {
        var visible = TaskScheduler.Current == scheduler ? "visible" : "not visible";
        Console.WriteLine( $"start context is {visible}" );
    }

    public static Task WrapInContext( this ConfigureAwait runnable, Func<Task> action )
        => Task
           .Factory
           .StartNew( action, CancellationToken.None, TaskCreationOptions.None, scheduler )
           .Unwrap();
}
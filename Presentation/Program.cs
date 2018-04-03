using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

internal class Program
{
    #region Constants

    private static Boolean explanationHeaderEnabled = true;
    private static readonly Stack<Int32> threadIds = new Stack<Int32>( 7 );
    private static readonly MethodInfo ExplanationHeaderPrinter = typeof(Program).GetMethod( nameof(PrintExplanationHeader), BindingFlags.NonPublic | BindingFlags.Static );

    #endregion

    private static Action<TextWriter> CreateExplainer( IRunnable runnable )
    {
        var extensionType = Type.GetType( $"{runnable.GetType() .Name}Extensions", false );
        if ( extensionType != null )
        {
            var method = extensionType.GetMethod( "Explain", BindingFlags.Public | BindingFlags.Static );
            if ( method != null )
            {
                var parameter = Expression.Parameter( typeof(TextWriter), "writer" );
                var block = Expression.Block( Expression.Call( null, ExplanationHeaderPrinter ), Expression.Call( null, method, Expression.Constant( runnable ), parameter ) );
                return Expression.Lambda<Action<TextWriter>>( block, parameter )
                                 .Compile();
                ;
            }
        }

        return writer => { };
    }

    private static async Task Main( String[] args )
    {
        Console.Clear();

        var runnables = (
            from type in typeof(Program).Assembly.GetTypes()
            where typeof(IRunnable).IsAssignableFrom( type ) && type != typeof(IRunnable)
            let activatedRunnable = (IRunnable) Activator.CreateInstance( type )
            let order = type.GetCustomAttribute<OrderAttribute>()
                            .Order
            let explainer = CreateExplainer( activatedRunnable )
            orderby order
            select new { Order = order, ActivatedRunnable = activatedRunnable, Explainer = explainer }
        ).ToDictionary( k => k.Order, v => new RunnerWithExplainer( v.ActivatedRunnable, v.Explainer ) );

        UpdateDescription( runnables );
        PrintRunnables( runnables );

        String line;
        while ( ( line = Console.ReadLine()
                                .ToLowerInvariant() ) != "exit" )
        {
            if ( line == "clear" )
            {
                Console.Clear();
                PrintRunnables( runnables );
            }

            if ( Int32.TryParse( line, out var itemNumber ) )
                if ( runnables.TryGetValue( itemNumber, out var runnable ) )
                {
                    Console.Clear();
                    var currentColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Yellow;

                    var expectionCaught = false;
                    var stopWatch = Stopwatch.StartNew();
                    var run = runnable.Run();
                    try
                    {
                        Console.WriteLine( $"-- Task state: {run.Status}" );
                        Console.WriteLine();
                        await run.ConfigureAwait( false );
                    }
                    catch ( Exception ex )
                    {
                        expectionCaught = true;
                        Console.WriteLine();
                        Console.WriteLine( $"-- Caught: {ex.GetType() .Name}('{ex.Message}')" );
                    }
                    finally
                    {
                        stopWatch.Stop();
                        if ( !expectionCaught )
                            Console.WriteLine();
                        Console.WriteLine( $"-- Task state: {run.Status}" );
                        Console.WriteLine( $"-- Execution time: {stopWatch.Elapsed}" );

                        runnable.Explain();

                        Console.ForegroundColor = currentColor;
                    }
                }
        }
    }

    private static String PadBoth( String source, Int32 length )
    {
        var spaces = length - source.Length;
        var padLeft = spaces / 2 + source.Length;
        return source.PadLeft( padLeft )
                     .PadRight( length );
    }

    private static void PrintExplanationHeader()
    {
        if ( explanationHeaderEnabled )
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine();
            Console.WriteLine( "|================================================|" );
            Console.WriteLine( $"| {"Remember".PadRight( 47 )}|" );
            Console.WriteLine( "|================================================|" );
            Console.WriteLine();
        }
    }

    private static void PrintRunnables( Dictionary<Int32, RunnerWithExplainer> runnables )
    {
        var currentThreadId = Thread.CurrentThread.ManagedThreadId;
        if ( threadIds.Count > 4 )
            threadIds.Clear();

        threadIds.Push( currentThreadId );

        var longest = runnables.Values.Max( d => d.Name.Length ) + 5;
        var fullWidth = longest * 2;

        var currentColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;

        Console.WriteLine( $"|{String.Join( "=", Enumerable.Repeat( String.Empty, fullWidth ) )}|" );
        Console.WriteLine( $"{$"| Thread(s): {String.Join( ",", threadIds )}".PadRight( fullWidth )}|" );
        Console.WriteLine( $"|{String.Join( "=", Enumerable.Repeat( String.Empty, fullWidth ) )}|" );
        Console.WriteLine();

        var elements = runnables.Values.Count;
        var half = elements / 2;
        for ( var i = 0; i < half; i++ )
        {
            var left = runnables.ElementAtOrDefault( i );
            if ( left.Equals( default ) )
                break;
            var right = runnables.ElementAtOrDefault( i + half );
            if ( right.Equals( default ) )
                break;

            if ( left.Key == right.Key )
                continue;
            var leftString = $" ({PadBoth( left.Key.ToString(), 5 )}) {left.Value.Name}";
            var rightString = $"({PadBoth( right.Key.ToString(), 5 )}) {right.Value.Name}";
            Console.WriteLine( $"{leftString.PadRight( longest )}{rightString}" );
        }

        if ( elements % 2 == 1 )
        {
            var last = runnables.Last();
            var lastString = $"({PadBoth( last.Key.ToString(), 5 )}) {last.Value.Name}";
            Console.WriteLine( $"{"".PadRight( longest )}{lastString}" );
        }

        Console.WriteLine();
        Console.WriteLine( $"|{String.Join( "=", Enumerable.Repeat( String.Empty, fullWidth ) )}|" );
        Console.WriteLine( $"{$"| github.com/danielmarbach/Await.HeadExplosion (*)".PadRight( fullWidth )}|" );
        Console.WriteLine( $"{$"| @danielmarbach | planetgeek.ch | LinkedIn : https://goo.gl/YyWJGf".PadRight( fullWidth )}|" );
        Console.WriteLine( $"|{String.Join( "=", Enumerable.Repeat( String.Empty, fullWidth ) )}|" );
        Console.WriteLine( " (*) don't forget to star the repo ;)" );
        Console.ForegroundColor = currentColor;
    }

    private static void UpdateDescription( Dictionary<Int32, RunnerWithExplainer> runnables )
    {
        explanationHeaderEnabled = false;
        try
        {
            using ( var file = File.CreateText( "README.MD" ) )
                foreach ( var item in runnables.Values )
                {
                    file.WriteLine( $"## {item.Name}" );
                    item.Explain( file );
                }
        }
        finally
        {
            explanationHeaderEnabled = true;
        }
    }

    #region Nested Types

    private class RunnerWithExplainer
    {
        #region Fields

        private readonly Action<TextWriter> explainer;

        private readonly IRunnable runnable;

        #endregion

        #region Properties

        public String Name { get; }

        #endregion

        #region Ctor

        public RunnerWithExplainer( IRunnable runnable, Action<TextWriter> explainer )
        {
            this.runnable = runnable;
            this.explainer = explainer;
            Name = runnable.GetType()
                           .Name;
        }

        #endregion

        public void Explain( TextWriter writer = null )
        {
            explainer( writer ?? Console.Out );
        }

        public Task Run() => runnable.Run();
    }

    #endregion
}
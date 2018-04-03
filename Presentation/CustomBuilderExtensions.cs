using System;
using System.IO;

internal static class CustomBuilderExtensions
{
    public static void Explain( this CustomBuilder runnable, TextWriter writer )
    {
        writer.WriteLine( @"
- Category useless knowledge
- Make fun of your coworkers
" );
    }

    public static void PrintResult( this CustomBuilder runnable, Int32 result )
    {
        Console.WriteLine( $"Result: {result}" );
    }
}
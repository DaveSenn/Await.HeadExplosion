using System;
using System.Threading;

internal static class CpuBound
{
    #region Constants

    private static readonly Random random = new Random();

    #endregion

    public static void Compute( Int32 numberOfElements )
    {
        var threadId = Thread.CurrentThread.ManagedThreadId;
        var elements = new Int32[numberOfElements];
        for ( var i = 0; i < numberOfElements; i++ )
            elements[i] = random.Next( 0, 100 );

        var unsorted = String.Join( ",", elements );
        Console.WriteLine( $"Begin {numberOfElements}/{threadId}" );

        Quicksort( elements, 0, elements.Length - 1 );

        Console.WriteLine( $"Done {numberOfElements}/{threadId}: '{unsorted}' => '{String.Join( ",", elements )}'" );
    }

    private static void Quicksort( Int32[] elements, Int32 left, Int32 right )
    {
        var i = left;
        var j = right;
        var pivot = elements[( left + right ) / 2];

        while ( i <= j )
        {
            while ( elements[i]
                        .CompareTo( pivot ) < 0 )
                i++;

            while ( elements[j]
                        .CompareTo( pivot ) > 0 )
                j--;

            if ( i <= j )
            {
                // Swap
                var tmp = elements[i];
                elements[i] = elements[j];
                elements[j] = tmp;

                i++;
                j--;
            }
        }

        // Recursive calls
        if ( left < j )
            Quicksort( elements, left, j );

        if ( i < right )
            Quicksort( elements, i, right );
    }
}
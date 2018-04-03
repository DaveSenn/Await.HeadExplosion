using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[Order( 22 )]
internal class NotifyCompletion : IRunnable
{
    public async Task Run()
    {
        this.PrintCurrentCulture();
        await CultureInfo.CurrentCulture;
        this.PrintCurrentCulture();
    }
}

public static class CultureAwaitExtensions
{
    public static CultureAwaiter GetAwaiter( this CultureInfo info ) => new CultureAwaiter( info );

    #region Nested Types

    public struct CultureAwaiter : ICriticalNotifyCompletion
    {
        private readonly CultureInfo cultureInfo;
        private readonly Task task;

        public CultureAwaiter( CultureInfo cultureInfo )
        {
            this.cultureInfo = cultureInfo;
            task = Task.Delay( 2000 );
        }

        // magic property
        public Boolean IsCompleted => task.IsCompleted;

        public void OnCompleted( Action continuation )
        {
            task.GetAwaiter()
                .OnCompleted( continuation );
        }

        public void UnsafeOnCompleted( Action continuation )
        {
            task.GetAwaiter()
                .UnsafeOnCompleted( continuation );
        }

        // magic Method
        public void GetResult()
        {
            CultureInfo.CurrentCulture = new CultureInfo( "en-us" );
        }
    }

    #endregion
}
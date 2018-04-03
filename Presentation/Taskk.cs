using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[AsyncMethodBuilder( typeof(TaskkMethodBuilder<>) )]
public struct Taskk<TResult>
{
    public override Int32 GetHashCode()
    {
        unchecked
        {
            return ( EqualityComparer<TResult>.Default.GetHashCode( _result ) * 397 ) ^ _hasResult.GetHashCode();
        }
    }

    internal readonly TResult _result;
    internal readonly Boolean _hasResult;
    internal readonly Task<TResult> _task;

    public Taskk( Task<TResult> task )
    {
        _result = default;
        _task = task;
        _hasResult = false;
    }

    public Taskk( TResult result )
    {
        _result = result;
        _hasResult = true;
        _task = null;
    }

    public Boolean HasValue => _hasResult;
    public Boolean Equals( Taskk<TResult> other ) => !_hasResult && !other._hasResult || _hasResult && other._hasResult && _result.Equals( other._result );
    public override Boolean Equals( Object obj ) => obj is Taskk<TResult> && Equals( (Taskk<TResult>) obj );
    public static Boolean operator ==( Taskk<TResult> left, Taskk<TResult> right ) => left.Equals( right );
    public static Boolean operator !=( Taskk<TResult> left, Taskk<TResult> right ) => !left.Equals( right );
    public override String ToString() => _hasResult ? _result.ToString() : "";
    public Boolean IsCompleted => _task == null || _task.IsCompleted;

    public Boolean IsCompletedSuccessfully => _task == null || _task.Status == TaskStatus.RanToCompletion;

    public Boolean IsFaulted => _task != null && _task.IsFaulted;

    public Boolean IsCanceled => _task != null && _task.IsCanceled;

    public TResult Result => _task == null
        ? _result
        : _task.GetAwaiter()
               .GetResult();

    public Task<TResult> AsTask() => _task ?? Task.FromResult( _result );

    public TaskkAwaiter<TResult> GetAwaiter() => new TaskkAwaiter<TResult>( this );

    public ConfiguredTaskkAwaiter<TResult> ConfigureAwait( Boolean continueOnCapturedContext ) =>
        new ConfiguredTaskkAwaiter<TResult>( this, continueOnCapturedContext );

    [EditorBrowsable( EditorBrowsableState.Never )] // intended only for compiler consumption
    public static TaskkMethodBuilder<TResult> CreateAsyncMethodBuilder() => TaskkMethodBuilder<TResult>.Create();
}
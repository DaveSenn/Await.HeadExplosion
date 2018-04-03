using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[StructLayout( LayoutKind.Auto )]
public struct TaskkAwaiter<TResult> : ICriticalNotifyCompletion
{
    private readonly Taskk<TResult> _value;

    internal TaskkAwaiter( Taskk<TResult> value ) => _value = value;

    public Boolean IsCompleted => _value.IsCompleted;

    public TResult GetResult() =>
        _value._task == null
            ? _value._result
            : _value._task.GetAwaiter()
                    .GetResult();

    public void OnCompleted( Action continuation ) =>
        _value.AsTask()
              .ConfigureAwait( true )
              .GetAwaiter()
              .OnCompleted( continuation );

    public void UnsafeOnCompleted( Action continuation ) =>
        _value.AsTask()
              .ConfigureAwait( true )
              .GetAwaiter()
              .UnsafeOnCompleted( continuation );
}

[StructLayout( LayoutKind.Auto )]
public struct ConfiguredTaskkAwaiter<TResult> : ICriticalNotifyCompletion
{
    private readonly Taskk<TResult> _value;
    private readonly Boolean _continueOnCapturedContext;

    internal ConfiguredTaskkAwaiter( Taskk<TResult> value, Boolean continueOnCapturedContext )
    {
        _value = value;
        _continueOnCapturedContext = continueOnCapturedContext;
    }

    public ConfiguredTaskkAwaiter<TResult> GetAwaiter() =>
        new ConfiguredTaskkAwaiter<TResult>( _value, _continueOnCapturedContext );

    public Boolean IsCompleted => _value.IsCompleted;

    public TResult GetResult() =>
        _value._task == null
            ? _value._result
            : _value._task.GetAwaiter()
                    .GetResult();

    public void OnCompleted( Action continuation ) =>
        _value.AsTask()
              .ConfigureAwait( _continueOnCapturedContext )
              .GetAwaiter()
              .OnCompleted( continuation );

    public void UnsafeOnCompleted( Action continuation ) =>
        _value.AsTask()
              .ConfigureAwait( _continueOnCapturedContext )
              .GetAwaiter()
              .UnsafeOnCompleted( continuation );
}
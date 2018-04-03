using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

internal class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
{
    #region Constants

    // Indicates whether the current thread is processing work items.
    [ThreadStatic]
    private static Boolean _currentThreadIsProcessingItems;

    #endregion

    #region Fields

    // The maximum concurrency level allowed by this scheduler.  
    private readonly Int32 _maxDegreeOfParallelism;

    // The list of tasks to be executed  
    private readonly LinkedList<Task> _tasks = new LinkedList<Task>(); // protected by lock(_tasks) 

    // Indicates whether the scheduler is currently processing work items.  
    private Int32 _delegatesQueuedOrRunning;

    #endregion

    #region Properties

    // Gets the maximum concurrency level supported by this scheduler.  
    public sealed override Int32 MaximumConcurrencyLevel => _maxDegreeOfParallelism;

    #endregion

    #region Ctor

    // Creates a new instance with the specified degree of parallelism.  
    public LimitedConcurrencyLevelTaskScheduler( Int32 maxDegreeOfParallelism )
    {
        if ( maxDegreeOfParallelism < 1 ) throw new ArgumentOutOfRangeException( "maxDegreeOfParallelism" );
        _maxDegreeOfParallelism = maxDegreeOfParallelism;
    }

    #endregion

    // Gets an enumerable of the tasks currently scheduled on this scheduler.  
    protected sealed override IEnumerable<Task> GetScheduledTasks()
    {
        var lockTaken = false;
        try
        {
            Monitor.TryEnter( _tasks, ref lockTaken );
            if ( lockTaken ) return _tasks;
            else throw new NotSupportedException();
        }
        finally
        {
            if ( lockTaken ) Monitor.Exit( _tasks );
        }
    }

    // Queues a task to the scheduler.  
    protected sealed override void QueueTask( Task task )
    {
        // Add the task to the list of tasks to be processed.  If there aren't enough  
        // delegates currently queued or running to process tasks, schedule another.  
        lock ( _tasks )
        {
            _tasks.AddLast( task );
            if ( _delegatesQueuedOrRunning < _maxDegreeOfParallelism )
            {
                ++_delegatesQueuedOrRunning;
                NotifyThreadPoolOfPendingWork();
            }
        }
    }

    // Attempt to remove a previously scheduled task from the scheduler.  
    protected sealed override Boolean TryDequeue( Task task )
    {
        lock ( _tasks ) return _tasks.Remove( task );
    }

    // Attempts to execute the specified task on the current thread.  
    protected sealed override Boolean TryExecuteTaskInline( Task task, Boolean taskWasPreviouslyQueued )
    {
        // If this thread isn't already processing a task, we don't support inlining 
        if ( !_currentThreadIsProcessingItems ) return false;

        // If the task was previously queued, remove it from the queue 
        if ( taskWasPreviouslyQueued )
            // Try to run the task.  
            if ( TryDequeue( task ) )
                return TryExecuteTask( task );
            else
                return false;
        return TryExecuteTask( task );
    }

    // Inform the ThreadPool that there's work to be executed for this scheduler.  
    private void NotifyThreadPoolOfPendingWork()
    {
        ThreadPool.UnsafeQueueUserWorkItem( _ =>
                                            {
                                                // Note that the current thread is now processing work items. 
                                                // This is necessary to enable inlining of tasks into this thread.
                                                _currentThreadIsProcessingItems = true;
                                                Console.WriteLine( $"°°°° CurrentThread of scheduler is {Thread.CurrentThread.ManagedThreadId} °°°°" );
                                                try
                                                {
                                                    // Process all available items in the queue. 
                                                    while ( true )
                                                    {
                                                        Task item;
                                                        lock ( _tasks )
                                                        {
                                                            // When there are no more items to be processed, 
                                                            // note that we're done processing, and get out. 
                                                            if ( _tasks.Count == 0 )
                                                            {
                                                                --_delegatesQueuedOrRunning;
                                                                break;
                                                            }

                                                            // Get the next item from the queue
                                                            item = _tasks.First.Value;
                                                            _tasks.RemoveFirst();
                                                        }

                                                        // Execute the task we pulled out of the queue 
                                                        TryExecuteTask( item );
                                                    }
                                                }
                                                // We're done processing items on the current thread 
                                                finally
                                                {
                                                    _currentThreadIsProcessingItems = false;
                                                }
                                            },
                                            null );
    }
}
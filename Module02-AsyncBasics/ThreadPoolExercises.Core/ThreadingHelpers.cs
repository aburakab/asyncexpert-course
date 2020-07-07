using System;
using System.Threading;

namespace ThreadPoolExercises.Core
{
    public class ThreadingHelpers
    {
        // * Create a thread and execute there `action` given number of `repeats` - waiting for the execution!
        //   HINT: you may use `Join` to wait until created Thread finishes
        // * In a loop, check whether `token` is not cancelled
        // * If an `action` throws and exception (or token has been cancelled) - `errorAction` should be invoked (if provided)


        public static void ExecuteOnThread(Action action, int repeats, 
            CancellationToken token = default, Action<Exception>? errorAction = null)
        {
            var isCanceledOrException = false;

            for (int i = 0; i < repeats; i++)
            {
                if (isCanceledOrException) return;

                var thread = new Thread(() =>
                {
                    try
                    {
                        action.Invoke();
                    }
                    catch (Exception ex)
                    {
                        isCanceledOrException = true;
                        errorAction?.Invoke(ex);
                    }
                    finally
                    {
                        if (token.IsCancellationRequested)
                        {
                            isCanceledOrException = true;
                            errorAction?.Invoke(new OperationCanceledException(token));
                        }
                    }
                })
                {
                    IsBackground = true
                };
                thread.Start();
                thread.Join();
            }
        }

        // * Queue work item to a thread pool that executes `action` given number of `repeats` - waiting for the execution!
        //   HINT: you may use `AutoResetEvent` to wait until the queued work item finishes
        // * In a loop, check whether `token` is not cancelled
        // * If an `action` throws and exception (or token has been cancelled) - `errorAction` should be invoked (if provided)

        public static void ExecuteOnThreadPool(Action action,
            int repeats, CancellationToken token = default, Action<Exception>? errorAction = null)
        {
            var hasExeption = false;

            for (int i = 0; i < repeats; i++)
            {
                if (hasExeption)
                    return;

                if (token.IsCancellationRequested)
                {
                    errorAction?.Invoke(new OperationCanceledException(token));
                    return;
                }
                using var waitHandle = new AutoResetEvent(false);
                ThreadPool.QueueUserWorkItem(new WaitCallback((x) =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        hasExeption = true;
                        errorAction?.Invoke(ex);
                    }
                    finally
                    {
                        waitHandle.Set();
                    }

                }), waitHandle);

                waitHandle.WaitOne();
            }
        }
    }
}

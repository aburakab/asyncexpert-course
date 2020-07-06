using System;
using System.Threading;

namespace ThreadPoolExercises.Core
{
    public class ThreadingHelpers
    {
        public static void ExecuteOnThread(Action action, int repeats, CancellationToken token = default, Action<Exception>? errorAction = null)
        {
            // * Create a thread and execute there `action` given number of `repeats` - waiting for the execution!
            //   HINT: you may use `Join` to wait until created Thread finishes
            // * In a loop, check whether `token` is not cancelled
            // * If an `action` throws and exception (or token has been cancelled) - `errorAction` should be invoked (if provided)

            var isCanceledOrException = false;
            
            var ts = new ThreadStart(() =>
            {
                if (isCanceledOrException)
                    return;

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
                    if (token.IsCancellationRequested || isCanceledOrException)
                    {
                        isCanceledOrException = true;
                        errorAction?.Invoke(new OperationCanceledException(token));
                    }
                }
            });

            for (int i = 0; i < repeats; i++)
            {
                var thread = new Thread(ts)
                {
                    IsBackground = true
                };
                thread.Start();
                thread.Join();
            }
        }

        public static void ExecuteOnThreadPool(Action action, int repeats, CancellationToken token = default, Action<Exception>? errorAction = null)
        {
            // * Queue work item to a thread pool that executes `action` given number of `repeats` - waiting for the execution!
            //   HINT: you may use `AutoResetEvent` to wait until the queued work item finishes
            // * In a loop, check whether `token` is not cancelled
            // * If an `action` throws and exception (or token has been cancelled) - `errorAction` should be invoked (if provided)

            var isCanceledOrException = false;

            for (int i = 0; i < repeats; i++)
            {
                if (token.IsCancellationRequested || isCanceledOrException)
                {
                    errorAction?.Invoke(new OperationCanceledException(token));
                    break;
                }

                using var waitHandle = new AutoResetEvent(false);

                RegisteredWaitHandle? handle = null;
                handle = ThreadPool.RegisterWaitForSingleObject(waitHandle, (state, timedout) =>
                {
                    handle!.Unregister(waitHandle);

                    if (isCanceledOrException)
                        return;

                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        isCanceledOrException = true;
                        errorAction?.Invoke(ex);
                    }

                }, null, 100, true);

                waitHandle.WaitOne();

            }
        }
    }
}

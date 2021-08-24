using System;
using System.Threading;
using System.Threading.Tasks;

namespace proxmox_cloud
{
    public static class Extensions
    {
        public static Task<bool> WaitOneAsync(this WaitHandle handle, int millisecondsTimeout) => handle.WaitOneAsync(millisecondsTimeout, CancellationToken.None);

        public static async Task<bool> WaitOneAsync(this WaitHandle handle, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            RegisteredWaitHandle registeredHandle = null;
            CancellationTokenRegistration tokenRegistration = default;
            try
            {
                var tcs = new TaskCompletionSource<bool>();
                registeredHandle = ThreadPool.RegisterWaitForSingleObject(
                    handle,
                    (state, timedOut) => ((TaskCompletionSource<bool>)state).TrySetResult(!timedOut),
                    tcs,
                    millisecondsTimeout,
                    true);
                tokenRegistration = cancellationToken.Register(
                    state => ((TaskCompletionSource<bool>)state).TrySetCanceled(),
                    tcs);
                return await tcs.Task;
            }
            finally
            {
                if (registeredHandle != null)
                    registeredHandle.Unregister(null);
                tokenRegistration.Dispose();
            }
        }

        public static Task<bool> WaitOneAsync(this WaitHandle handle, TimeSpan timeout) => handle.WaitOneAsync((int)timeout.TotalMilliseconds);

        public static Task<bool> WaitOneAsync(this WaitHandle handle, TimeSpan timeout, CancellationToken cancellationToken) => handle.WaitOneAsync((int)timeout.TotalMilliseconds, cancellationToken);

        public static Task<bool> WaitOneAsync(this WaitHandle handle) => handle.WaitOneAsync(Timeout.Infinite);

        public static Task<bool> WaitOneAsync(this WaitHandle handle, CancellationToken cancellationToken) => handle.WaitOneAsync(Timeout.Infinite, cancellationToken);
    }
}
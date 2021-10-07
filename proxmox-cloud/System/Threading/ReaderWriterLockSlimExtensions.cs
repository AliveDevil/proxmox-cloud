using System;
using System.Threading;

namespace proxmox_cloud.System.Threading
{
    public static class ReaderWriterLockSlimExtensions
    {
        public static Scope UseReadLock(this ReaderWriterLockSlim @this) => new(@this.EnterReadLock, @this.ExitReadLock);

        public static Scope UseWriteLock(this ReaderWriterLockSlim @this) => new(@this.EnterWriteLock, @this.ExitWriteLock);

        public ref struct Scope
        {
            private readonly Action release;

            public Scope(Action enter, Action release)
            {
                this.release = release;
                enter();
            }

            public void Dispose() => release();
        }
    }
}
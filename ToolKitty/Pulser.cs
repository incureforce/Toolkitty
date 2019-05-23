using System;
using System.Collections.Generic;
using System.Threading;

namespace System
{
    public class Pulser<T>
    {
        private readonly object syncLock = new object();

        private volatile bool wait;
        private readonly Queue<T> queue = new Queue<T>();

        public bool Push(T item, bool queueIfEmpty = true) {
            lock (syncLock) {
                var oldWait = wait;
                if (oldWait || queueIfEmpty) {
                    queue.Enqueue(item);

                    Monitor.Pulse(syncLock);
                }

                return oldWait;
            }
        }

        public bool Wait(out T item, int timeout = -1) {
            lock (syncLock) {
                wait = true;

                var okay = queue.Count > 0 || Monitor.Wait(syncLock, timeout);

                wait = false;

                if (okay) {
                    item = queue.Dequeue();
                } else {
                    item = default(T);
                }

                return okay;
            }
        }
    }
}
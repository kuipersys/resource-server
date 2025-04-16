// <copyright file="EventScheduler.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Scheduling
{
    using System.Collections.Concurrent;

    public class EventScheduler : IEventScheduler
    {
        private bool isDisposed = false;
        private ConcurrentDictionary<Guid, IEventTimer> eventTimers = new ConcurrentDictionary<Guid, IEventTimer>();

        public int ActiveTimers
        {
            get => this.eventTimers.Count;
        }

        public void Dispose()
        {
            this.isDisposed = true;
            var timers = this.eventTimers.ToArray();
            this.eventTimers.Clear();

            foreach (var timer in timers)
            {
                timer.Value.Dispose();
            }
        }

        public IEventTimer ScheduleCallback(Action callback, TimeSpan timeout)
        {
            this.AssertNotDisposed();

            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be greater than zero.");
            }

            IEventTimer timer = new BasicEventTimer(callback, timeout);
            if (this.eventTimers.TryAdd(timer.Id, timer))
            {
                timer.OnDisposed += this.RemoveEventTimerHandler;
            }
            else
            {
                throw new InvalidOperationException("Failed to schedule callback.");
            }

            return timer;
        }

        public IEventTimer ScheduleAsyncCallback(Func<Task> callback, TimeSpan timeout)
        {
            this.AssertNotDisposed();

            if (timeout < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be greater than zero.");
            }

            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback), "Callback cannot be null.");
            }

            IEventTimer timer = new BasicEventTimer(callback, timeout);
            if (this.eventTimers.TryAdd(timer.Id, timer))
            {
                timer.OnDisposed += this.RemoveEventTimerHandler;
            }
            else
            {
                throw new InvalidOperationException("Failed to schedule async callback.");
            }

            return timer;
        }

        private void AssertNotDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(nameof(EventScheduler));
            }
        }

        private void RemoveEventTimerHandler(object source, EventArgs e)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            this.eventTimers.TryRemove((source as IEventTimer).Id, out IEventTimer _);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }
}

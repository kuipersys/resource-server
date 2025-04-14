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
        private ConcurrentDictionary<Guid, IEventTimer> eventTimers = new ConcurrentDictionary<Guid, IEventTimer>();

        public int ActiveTimers
        {
            get => this.eventTimers.Count;
        }

        public void Dispose()
        {
            foreach (var timer in this.eventTimers)
            {
                timer.Value.Dispose();
            }

            this.eventTimers.Clear();
        }

        public IEventTimer ScheduleCallback(Action callback, TimeSpan timeout)
        {
            IEventTimer timer = new BasicEventTimer(callback, timeout);
            timer.OnCancel += this.RemoveEventTimerHandler;
            this.eventTimers.TryAdd(timer.Id, timer);

            return timer;
        }

        public IEventTimer ScheduleAsyncCallback(Func<Task> callback, TimeSpan timeout)
        {
            IEventTimer timer = new BasicEventTimer(callback, timeout);
            timer.OnCancel += this.RemoveEventTimerHandler;
            this.eventTimers.TryAdd(timer.Id, timer);

            return timer;
        }

        private void RemoveEventTimerHandler(object source, EventArgs e)
        {
            this.eventTimers.TryRemove((source as IEventTimer).Id, out IEventTimer _);
        }
    }
}

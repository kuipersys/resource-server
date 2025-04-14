// <copyright file="IEventScheduler.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Scheduling
{
    public interface IEventScheduler : IDisposable
    {
        public int ActiveTimers { get; }

        IEventTimer ScheduleCallback(Action callback, TimeSpan timeout);

        IEventTimer ScheduleAsyncCallback(Func<Task> callback, TimeSpan timeout);
    }
}

// <copyright file="WatchdogTimer.cs" company="Kuiper Microsystems, LLC">
// Copyright (c) Kuiper Microsystems, LLC. All rights reserved.
// </copyright>

namespace Kuiper.Platform.Runtime.Scheduling
{
    using Kuiper.Platform.Extensions;

    public class WatchdogTimer : IEventTimer
    {
        private readonly TimeSpan timeout;
        private readonly Action callback;
        private readonly System.Timers.Timer timer;

        public WatchdogTimer(Action callback, TimeSpan timeout)
        {
            callback.ThrowIfArgumentNull(nameof(callback));
            this.timeout = timeout;
            this.callback = callback;
            this.timer = new System.Timers.Timer(this.timeout.TotalMilliseconds);
            this.timer.AutoReset = false;
            this.timer.Elapsed += this.TimerElapsed;
            this.timer.Enabled = true;
        }

        public event BasicEventHandler? OnCancel;

        public Guid Id { get; } = Guid.NewGuid();

        public void Reset()
        {
            this.timer.Stop();
            this.timer.Start();
        }

        public void Cancel()
        {
            try
            {
                this.timer.Enabled = false;
            }
            finally
            {
                this.OnCancel?.Invoke(this, new EventArgs());
            }
        }

        public void Dispose()
        {
            this.Cancel();

            this.timer.Dispose();
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.callback.Invoke();
        }
    }
}

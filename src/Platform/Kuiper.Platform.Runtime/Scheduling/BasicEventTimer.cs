// <copyright file="BasicEventTimer.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Scheduling
{
    using Kuiper.Platform.Extensions.Contracts;

    using static Kuiper.Platform.Runtime.Scheduling.IEventTimer;

    internal class BasicEventTimer : IEventTimer
    {
        private readonly TimeSpan timeout;
        private readonly Func<Task> callback;
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(0, 1);
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private volatile Timer timer;

        public BasicEventTimer(Action callback, TimeSpan timeout)
            : this(
                () =>
                {
                    callback.Invoke();
                    return Task.CompletedTask;
                }, timeout)
        {
            callback.ThrowIfArgumentNull(nameof(callback));
        }

        public BasicEventTimer(Func<Task> callback, TimeSpan timeout)
        {
            callback.ThrowIfArgumentNull(nameof(callback));
            this.timeout = timeout;
            this.callback = callback;
            this.timer = new Timer(
                new TimerCallback(async (state) =>
                    await this.Run(this.cancellationTokenSource.Token)),
                null,
                this.timeout,
                this.timeout);
        }

        public event OnDisposedEventHandler? OnDisposed;

        public Guid Id { get; } = Guid.NewGuid();

        public void Reset()
        {
            this.timer.Change(this.timeout, this.timeout);
        }

        public void Cancel()
        {
            try
            {
                if (!this.cancellationTokenSource.IsCancellationRequested)
                {
                    this.cancellationTokenSource.Cancel();
                }

                this.timer.Change(Timeout.Infinite, Timeout.Infinite);

                // We wait for ~4 times the timeout to ensure that any running task has time to finish.
                this.semaphoreSlim.Wait(new TimeSpan(this.timeout.Ticks * 4));
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            this.semaphoreSlim.Dispose();
            this.Cancel();
            this.OnDisposed?.Invoke(this, new EventArgs());
            this.OnDisposed = null;
        }

        private async Task Run(CancellationToken cancellationToken)
        {
            if (!await this.semaphoreSlim.WaitAsync(0))
            {
                // If we cannot get the semaphore, the previous task is still running.
                Console.WriteLine("Skipping callback to prevent overlap.");
                return;
            }

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Callback cancelled before starting.");
                    return;
                }

                await this.callback.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in TimerCallback: {ex.Message}");
            }
            finally
            {
                this.semaphoreSlim.Release();
            }
        }
    }
}

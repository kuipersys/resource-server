// <copyright file="IEventTimer.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Scheduling
{
    public interface IEventTimer : IDisposable
    {
        public delegate void OnDisposedEventHandler(object source, EventArgs e);

        event OnDisposedEventHandler OnDisposed;

        Guid Id { get; }

        void Cancel();

        void Reset();
    }
}

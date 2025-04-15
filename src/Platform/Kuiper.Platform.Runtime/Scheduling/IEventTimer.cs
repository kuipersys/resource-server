// <copyright file="IEventTimer.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Scheduling
{
    public delegate void BasicEventHandler(object source, EventArgs e);

    public interface IEventTimer : IDisposable
    {
        event BasicEventHandler OnCancel;

        Guid Id { get; }

        void Cancel();

        void Reset();
    }
}

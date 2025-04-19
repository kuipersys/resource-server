// <copyright file="FairReaderWriterLockSlim.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime
{
    // This will be needed to ensure that hot-swap plugins, commands, resources, etc. are
    // thread-safe and will not get dead-locked in the process of attempting to update
    // critical sections of the running system and allowing it to be hot-swapable.
    internal class FifoReaderWriterLockSlim : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

// <copyright file="PatchResourcePlugin.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Runtime.Plugins
{
    using System;
    using System.Threading.Tasks;

    using Kuiper.Platform.Runtime.Abstractions.Extensibility;

    internal sealed class PatchResourcePlugin : IPlugin
    {
        public Task ExecuteAsync(IServiceProvider serviceProvider)
        {
            throw new PlatformNotSupportedException();
        }
    }
}

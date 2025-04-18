// <copyright file="MutationPlugin.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Service.Plugins
{
    using System;
    using System.Threading.Tasks;

    using Kuiper.Platform.Runtime.Abstractions.Extensibility;

    public class MutationPlugin : IPlugin
    {
        public MutationPlugin()
        {
        }

        public Task ExecuteAsync(IServiceProvider serviceProvider)
        {
            return Task.CompletedTask;
        }
    }
}

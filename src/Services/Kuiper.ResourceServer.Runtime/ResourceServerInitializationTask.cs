// <copyright file="ResourceServerInitializationTask.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Runtime
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kuiper.Platform.Runtime.Abstractions.Initialization;
    using Kuiper.ResourceServer.Runtime.Core;

    using Microsoft.Extensions.DependencyInjection;

    internal sealed class ResourceServerInitializationTask : IInitializationTask
    {
        private readonly IServiceProvider services;

        public ResourceServerInitializationTask(IServiceProvider services)
        {
            this.services = services;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            await this.services.GetRequiredService<IResourceManager>()
                .InitializeAsync(cancellationToken);
        }
    }
}

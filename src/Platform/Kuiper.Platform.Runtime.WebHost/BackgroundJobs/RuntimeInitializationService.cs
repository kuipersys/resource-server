// <copyright file="RuntimeInitializationJob.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.WebHost.BackgroundJobs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kuiper.Platform.Runtime.Abstractions.Initialization;
    using Kuiper.Platform.Runtime.Errors;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// A background job that initializes the runtime environment.
    /// </summary>
    internal sealed class RuntimeInitializationService : IHostedService
    {
        private readonly IServiceProvider services;

        public RuntimeInitializationService(IServiceProvider services)
        {
            this.services = services;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var initializationTasks = this.services.GetServices<IInitializationTask>();

            foreach (var initializationTask in initializationTasks)
            {
                try
                {
                    await initializationTask.InitializeAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new PlatformRuntimeException($"Failed to initialize task {initializationTask.GetType().Name}", ex);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

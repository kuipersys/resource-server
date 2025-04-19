// <copyright file="DeleteResourcePlugin.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Runtime.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Kuiper.Platform.ManagementObjects;
    using Kuiper.Platform.Runtime.Abstractions.Extensibility;
    using Kuiper.Platform.Runtime.Errors;
    using Kuiper.Platform.Serialization.Serialization;
    using Kuiper.ServiceInfra.Abstractions.Persistence;

    using Microsoft.Extensions.DependencyInjection;

    internal class DeleteResourcePlugin : IPlugin
    {
        public async Task ExecuteAsync(IServiceProvider serviceProvider)
        {
            var store = serviceProvider.GetRequiredService<IKeyValueStore>();
            var context = serviceProvider.GetRequiredService<IRuntimeExecutionContext>();

            ResourceDescriptor resourceDescriptor = context
                .InputParameters["target"]
                .MarshalAs<ResourceDescriptor>();

            string resourceId = resourceDescriptor.ToResourceId();

            try
            {
                await store.DeleteAsync(resourceId, context.CancellationToken);
            }
            catch (KeyNotFoundException ex)
            {
                throw new PlatformRuntimeException(PlatformRuntimeErrorCodes.ResourceNotFound, ex);
            }
        }
    }
}

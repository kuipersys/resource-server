// <copyright file="DeleteResourcePlugin.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Service.Plugins
{
    using Kuiper.Platform.Framework.Errors;
    using Kuiper.Platform.Framework.Extensibility;
    using Kuiper.Platform.ManagementObjects;
    using Kuiper.Platform.Serialization.Serialization;
    using Kuiper.ServiceInfra.Abstractions.Persistence;

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
                throw new PlatformException(PlatformErrorCodes.ResourceNotFound, ex);
            }
        }
    }
}

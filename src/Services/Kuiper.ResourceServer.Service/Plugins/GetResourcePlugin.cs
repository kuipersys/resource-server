// <copyright file="GetResourcePlugin.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Service.Plugins
{
    using Kuiper.Platform.ManagementObjects;
    using Kuiper.Platform.Runtime.Abstractions.Extensibility;
    using Kuiper.Platform.Runtime.Errors;
    using Kuiper.Platform.Runtime.Execution.Attributes;
    using Kuiper.Platform.Serialization.Serialization;
    using Kuiper.ServiceInfra.Abstractions.Persistence;

    [RequiredInput("target", typeof(ResourceDescriptor))]
    internal class GetResourcePlugin : IPlugin
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
                var data = await store.GetAsync(resourceId, context.CancellationToken);

                if (data == null)
                {
                    throw new PlatformRuntimeException(PlatformRuntimeErrorCodes.ResourceNotFound);
                }

                context.OutputParameters["result"] = data.JsonBytesToObject<SystemObject>();
            }
            catch (KeyNotFoundException ex)
            {
                throw new PlatformRuntimeException(PlatformRuntimeErrorCodes.ResourceNotFound, ex);
            }
        }
    }
}

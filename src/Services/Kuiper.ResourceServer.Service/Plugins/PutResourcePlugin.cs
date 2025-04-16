// <copyright file="PutResourcePlugin.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Service.Plugins
{
    using Kuiper.Platform.Framework.Abstractions;
    using Kuiper.Platform.Framework.Errors;
    using Kuiper.Platform.Framework.Extensibility;
    using Kuiper.Platform.ManagementObjects;
    using Kuiper.Platform.Runtime.Execution.Attributes;
    using Kuiper.Platform.Serialization.Serialization;
    using Kuiper.ResourceServer.Service.Core;
    using Kuiper.ServiceInfra.Abstractions.Persistence;

    [RequiredInput("target", typeof(SystemObject))]
    internal class PutResourcePlugin : IPlugin
    {
        public async Task ExecuteAsync(IServiceProvider serviceProvider)
        {
            var resourceManager = serviceProvider.GetRequiredService<IResourceManager>();
            var store = serviceProvider.GetRequiredService<IKeyValueStore>();
            var context = serviceProvider.GetRequiredService<IExecutionContext>();

            SystemObject systemObject = context.InputParameters["target"].MarshalAs<SystemObject>();
            systemObject.NormalizeResource();

            if (string.IsNullOrWhiteSpace(systemObject.Kind))
            {
                throw new PlatformException("Kind Is Missing", PlatformErrorCodes.ValidationError);
            }

            if (string.IsNullOrWhiteSpace(systemObject.ApiVersion))
            {
                throw new PlatformException("Api Version Is Missing", PlatformErrorCodes.ValidationError);
            }

            if (string.IsNullOrWhiteSpace(systemObject.Metadata.Name))
            {
                throw new PlatformException("Name Is Missing", PlatformErrorCodes.ValidationError);
            }

            var descriptor = systemObject.AsResourceDescriptor();
            string resourceId = descriptor.ToResourceId();

            try
            {
                var definition = await resourceManager.GetResourceDefinitionAsync(descriptor.Group, descriptor.Kind);
                systemObject.Kind = definition.Spec.Names.Kind;

                // Change the resource namespace and regenerate the resourceId if the scope is system
                if (definition.Spec.Scope == Platform.ManagementObjects.v1alpha1.ResourceScope.System)
                {
                    systemObject.Metadata.Namespace = SystemConstants.Resources.GLOBAL_NAMESPACE;
                    descriptor = systemObject.AsResourceDescriptor();
                    resourceId = descriptor.ToResourceId();
                }

                await store.PutAsync(resourceId, systemObject.ObjectToJsonBytes(), context.CancellationToken);

                var data = await store.GetAsync(resourceId, context.CancellationToken);
                context.OutputParameters["result"] = data.JsonBytesToObject<SystemObject>();
            }
            catch (KeyNotFoundException ex)
            {
                throw new PlatformException(PlatformErrorCodes.InternalServerError, ex);
            }
        }
    }
}

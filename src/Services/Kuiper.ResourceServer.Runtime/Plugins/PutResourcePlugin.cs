// <copyright file="PutResourcePlugin.cs" company="Kuiper Microsystems, LLC">
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
    using Kuiper.Platform.ManagementObjects.v1alpha1.Resource;
    using Kuiper.Platform.Runtime.Abstractions.Extensibility;
    using Kuiper.Platform.Runtime.Errors;
    using Kuiper.Platform.Runtime.Execution.Attributes;
    using Kuiper.Platform.Serialization.Serialization;
    using Kuiper.ResourceServer.Runtime.Core;
    using Kuiper.ServiceInfra.Abstractions.Persistence;

    using Microsoft.Extensions.DependencyInjection;

    [RequiredInput("target", typeof(SystemObject))]
    internal sealed class PutResourcePlugin : IPlugin
    {
        public async Task ExecuteAsync(IServiceProvider serviceProvider)
        {
            var resourceManager = serviceProvider.GetRequiredService<IResourceManager>();
            var store = serviceProvider.GetRequiredService<IKeyValueStore>();
            var context = serviceProvider.GetRequiredService<IRuntimeExecutionContext>();

            SystemObject systemObject = context.InputParameters["target"].MarshalAs<SystemObject>();
            systemObject.NormalizeResource();

            if (string.IsNullOrWhiteSpace(systemObject.Kind))
            {
                throw new PlatformRuntimeException("Kind Is Missing", PlatformRuntimeErrorCodes.ValidationError);
            }

            if (string.IsNullOrWhiteSpace(systemObject.ApiVersion))
            {
                throw new PlatformRuntimeException("Api Version Is Missing", PlatformRuntimeErrorCodes.ValidationError);
            }

            if (string.IsNullOrWhiteSpace(systemObject.Metadata.Name))
            {
                throw new PlatformRuntimeException("Name Is Missing", PlatformRuntimeErrorCodes.ValidationError);
            }

            var descriptor = systemObject.AsResourceDescriptor();
            string resourceId = descriptor.ToResourceId();

            try
            {
                var definition = await resourceManager.GetResourceDefinitionAsync(descriptor.Group, descriptor.Kind);
                systemObject.Kind = definition.Spec.Names.Kind;

                // Change the resource namespace and regenerate the resourceId if the scope is system
                if (definition.Spec.Scope == ResourceScope.System)
                {
                    systemObject.Metadata.Namespace = SystemConstants.Resources.GLOBAL_NAMESPACE;
                    descriptor = systemObject.AsResourceDescriptor();
                    resourceId = descriptor.ToResourceId();
                }

                await store.PutAsync(resourceId, systemObject.ObjectToJsonBytes(), context.CancellationToken);

                var data = await store.GetAsync(resourceId, context.CancellationToken);
                context.OutputParameters["result"] = data.JsonBytesToObject<SystemObject>();

                if (systemObject.Kind == "ResourceDefinition")
                {
                    await resourceManager.ReloadAsync(context.CancellationToken);
                }
            }
            catch (KeyNotFoundException ex)
            {
                throw new PlatformRuntimeException(PlatformRuntimeErrorCodes.InternalServerError, ex);
            }
        }
    }
}

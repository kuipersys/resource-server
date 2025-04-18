// <copyright file="CoreResourceModule.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

using Kuiper.Platform.ManagementObjects;
using Kuiper.Platform.ManagementObjects.v1alpha1;

namespace Kuiper.Platform.Modules
{
    using System;
    using System.Threading.Tasks;

    using Kuiper.Platform.Framework.Abstractions;
    using Kuiper.Platform.ManagementObjects.v1alpha1.Resource;

    /// <summary>
    /// This class is used to register all of the core resources that NEED to be registered
    /// in order for the Kuiper platform to function. Yes - these are then used to create
    /// other resources and thereby solving the chicken and egg problem within the platoform.
    ///
    /// This includes the following resources:
    /// - <see cref="ResourceDefinition"/>
    /// </summary>
    internal static class CoreResourceModule
    {
        public static ResourceDefinition[] GetResourceDefinitions()
        {
            return new[]
            {
                CreateSystemResourceDefinition(),
            };
        }

        private static ResourceDefinition CreateSystemResourceDefinition()
        {
            ResourceDefinitionSpec spec = new ResourceDefinitionSpec()
            {
                Group = SystemConstants.Resources.SYSTEM_EXTENSION_GROUP,
                Scope = ResourceScope.System,
                Names = new ResourceDefinitionNames()
                {
                    Kind = nameof(ResourceDefinition),
                    Plural = $"{nameof(ResourceDefinition).ToLower()}s",
                    Singular = $"{nameof(ResourceDefinition).ToLower()}",
                    ShortNames = new string[] { $"resource" },
                },
                Versions = new ResourceDefinitionVersion[]
                {
                    new ResourceDefinitionVersion()
                    {
                        Name = VersionConstants.Version,
                        Enabled = true,
                        Schema = new ResourceDefinitionVersionSchema()
                        {
                            OpenAPIV3Schema = SystemSchema.GetSchemaAsJsonElement<ResourceDefinition>(),
                        },
                    },
                },
            };

            ResourceDefinition resourceDefinition = new ResourceDefinition()
            {
                ApiVersion = $"{SystemConstants.Resources.SYSTEM_EXTENSION_GROUP}/{VersionConstants.Version}",
                Kind = nameof(ResourceDefinition),
                Metadata = new SystemObjectMetadata()
                {
                    Name = VersionConstants.ResourceDefinitionName,
                    Namespace = SystemConstants.Resources.GLOBAL_NAMESPACE,
                    ResourceVersion = VersionConstants.Version,
                    Uid = Guid.NewGuid().ToString(),
                },
                Spec = spec,
            };

            return resourceDefinition;
        }
    }
}

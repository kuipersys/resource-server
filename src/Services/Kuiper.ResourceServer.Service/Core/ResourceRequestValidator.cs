// <copyright file="ResourceRequestValidator.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Service.Core
{
    using Kuiper.Platform.Framework;
    using Kuiper.Platform.Framework.Errors;
    using Kuiper.Platform.ManagementObjects.v1alpha1;
    using Kuiper.Platform.Serialization.Serialization;

    /// <summary>
    /// TODO: Abstract
    /// </summary>
    internal sealed class ResourceRequestValidator : IResourceRequestValidator
    {
        private readonly IResourceManager resourceManager;
        private readonly NJsonSchema.JsonSchema systemObjectSchema;

        public ResourceRequestValidator(IResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;
            this.systemObjectSchema = SystemSchema.GetSchema<SystemObject>()!;
        }

        public async Task ValidateAsync(ResourceDescriptor? resourceDescriptor, bool throwOnNull = true)
        {
            if (resourceDescriptor == null)
            {
                if (!throwOnNull)
                {
                    return;
                }

                throw new PlatformException(
                    "Invalid Resource Descriptor",
                    PlatformErrorCodes.InvalidResourceKind,
                    new ArgumentNullException($"{nameof(ResourceDescriptor)}"));
            }

            if (string.IsNullOrWhiteSpace(resourceDescriptor.Kind))
            {
                throw new PlatformException(
                    "Invalid Resource Descriptor Kind",
                    PlatformErrorCodes.InvalidResourceKind,
                    new ArgumentNullException($"{nameof(ResourceDescriptor)}.{nameof(ResourceDescriptor.Kind)}"));
            }

            var resourceVersionExists = await this.resourceManager
                .ResourceVersionExists(resourceDescriptor.Group, resourceDescriptor.Kind, resourceDescriptor.GroupVersion);

            if (!resourceVersionExists)
            {
                throw new PlatformException(
                    $"Invalid Resource Kind: {resourceDescriptor.ApiVersion}/{resourceDescriptor.Kind}",
                    PlatformErrorCodes.InvalidResourceKind);
            }
        }

        public async Task ValidateAsync(SystemObject resource)
        {
            if (string.IsNullOrWhiteSpace(resource.Kind))
            {
                throw new PlatformException(
                    "Invalid System Object Kind",
                    PlatformErrorCodes.InvalidResourceKind,
                    new ArgumentNullException($"{nameof(SystemObject)}.{nameof(SystemObject.Kind)}"));
            }

            //if (string.IsNullOrWhiteSpace(resource.Metadata!.Name))
            //{
            //    throw new PlatformException(
            //        "Invalid System Object Name",
            //        PlatformErrorCodes.InvalidResourceKind,
            //        new ArgumentNullException($"{nameof(SystemObject)}.{nameof(SystemObject.Metadata)}.{nameof(SystemObject.Metadata.Name)}"));
            //}

            var descriptor = resource.AsResourceDescriptor();

            ResourceDefinition? resourceDefinition = await this.resourceManager.GetResourceDefinitionAsync(
                descriptor.Group,
                resource.Kind);

            ResourceDefinitionVersion? resourceVersion = await this.resourceManager.GetResourceVersionAsync(
                descriptor.Group,
                resource.Kind,
                descriptor.GroupVersion);

            if (resourceVersion == null)
            {
                throw new PlatformException(
                    $"Invalid Resource Kind: {resource.ApiVersion}/{resource.Kind}",
                    PlatformErrorCodes.InvalidResourceKind);
            }

            string resourceJson = resource.ObjectToJson();

            // Validate as a system object first
            var errors = this.systemObjectSchema.Validate(resourceJson);

            AssertValidResourceJson(this.systemObjectSchema, resourceJson);

            // Then validate as a specific resource
            if (resourceVersion.Schema.OpenAPIV3Schema != null)
            {
                var resourceSchema = resourceVersion
                    .GetJsonSchema();

                // var upscaledJson = SystemSchema.UpscaleObject(resourceSchema, resourceDefinition.Spec.Names.Kind, resourceJson);

                AssertValidResourceJson(resourceSchema, resourceJson);
            }
        }

        private static void AssertValidResourceJson(NJsonSchema.JsonSchema schema, string resourceJson)
        {
            var errors = schema.Validate(resourceJson);

            if (errors.Count > 0)
            {
                AggregateException validationException = new AggregateException(
                    errors.Select(e => new PlatformException(
                        $"Validation Error: {e.ToString()}",
                        PlatformErrorCodes.ValidationError)));

                throw validationException;
            }
        }
    }
}

// <copyright file="ValidationPlugin.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Service.Plugins
{
    using Kuiper.Platform.ManagementObjects;
    using Kuiper.Platform.ManagementObjects.v1alpha1.Resource;
    using Kuiper.Platform.Runtime.Abstractions.Extensibility;
    using Kuiper.Platform.Runtime.Errors;
    using Kuiper.Platform.Serialization.Serialization;
    using Kuiper.ResourceServer.Service.Core;

    using Microsoft.CodeAnalysis;

    internal class ValidationPlugin : IPlugin
    {
        private readonly IResourceManager resourceManager;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly NJsonSchema.JsonSchema systemObjectSchema;

        public ValidationPlugin(IResourceManager resourceManager, IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
            this.resourceManager = resourceManager;
            this.systemObjectSchema = SystemSchema.GetSchema<SystemObject>()!;
        }

        public async Task ExecuteAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<IRuntimeExecutionContext>();

            switch (context.Message.ToLower())
            {
                case "get":
                case "list":
                    await this.ValidateRetrieveAsync(context);
                    break;
                case "put":
                case "update":
                case "patch":
                    await this.ValidateMutationAsync(context);
                    break;
                case "delete":
                    await this.ValidateMutationAsync(context, hasSystemObjectTarget: false);
                    break;
                default:
                    break;
            }
        }

        private static void AssertValidResourceJson(NJsonSchema.JsonSchema schema, string resourceJson)
        {
            var errors = schema.Validate(resourceJson);

            if (errors.Count > 0)
            {
                AggregateException validationException = new AggregateException(
                    errors.Select(e => new PlatformRuntimeException(
                        $"Validation Error: {e.ToString()}",
                        PlatformRuntimeErrorCodes.ValidationError)));

                throw validationException;
            }
        }

        private async Task ValidateRetrieveAsync(IRuntimeExecutionContext context)
        {
            var resourceDescriptor = context.InputParameters["target"]
                    .MarshalAs<ResourceDescriptor>();

            await this.ValidateAsync(resourceDescriptor, throwOnNull: true);
        }

        private async Task ValidateMutationAsync(IRuntimeExecutionContext context, bool hasSystemObjectTarget = true)
        {
            if (!context.InputParameters.ContainsKey("target") || context.InputParameters["target"] == null)
            {
                throw new ArgumentException("The request payload is mising", "target");
            }

            var allowSystemModification = context.InputParameters["allowSystemModification"]?.MarshalAs<bool>() ?? false;

            if (hasSystemObjectTarget)
            {
                var systemObject = context.InputParameters["target"]
                    .MarshalAs<SystemObject>();

                var resourceDescriptor = systemObject.AsResourceDescriptor();

                if (!allowSystemModification && resourceDescriptor.Name.StartsWith("$"))
                {
                    throw new PlatformRuntimeException(
                        "System resources cannot be modified",
                        PlatformRuntimeErrorCodes.Forbidden);
                }

                await this.ValidateAsync(resourceDescriptor, throwOnNull: true);
                await this.ValidateAsync(systemObject);
            }
            else
            {
                var resourceDescriptor = context.InputParameters["target"]
                    .MarshalAs<ResourceDescriptor>();

                if (!allowSystemModification && resourceDescriptor.Name.StartsWith("$"))
                {
                    throw new PlatformRuntimeException(
                        "System resources cannot be modified",
                        PlatformRuntimeErrorCodes.Forbidden);
                }

                await this.ValidateAsync(resourceDescriptor, throwOnNull: true);
            }
        }

        private async Task ValidateAsync(ResourceDescriptor? resourceDescriptor, bool throwOnNull = true)
        {
            if (resourceDescriptor == null)
            {
                if (!throwOnNull)
                {
                    return;
                }

                throw new PlatformRuntimeException(
                    "Invalid Resource Descriptor",
                    PlatformRuntimeErrorCodes.InvalidResourceKind,
                    new ArgumentNullException($"{nameof(ResourceDescriptor)}"));
            }

            if (string.IsNullOrWhiteSpace(resourceDescriptor.Kind))
            {
                throw new PlatformRuntimeException(
                    "Invalid Resource Descriptor Kind",
                    PlatformRuntimeErrorCodes.InvalidResourceKind,
                    new ArgumentNullException($"{nameof(ResourceDescriptor)}.{nameof(ResourceDescriptor.Kind)}"));
            }

            ResourceDefinition? resourceDefinition = await this.resourceManager.GetResourceDefinitionAsync(
                resourceDescriptor.Group,
                resourceDescriptor.Kind);

            if (resourceDefinition == null)
            {
                throw new PlatformRuntimeException(
                    $"Invalid Resource Kind: {resourceDescriptor.ApiVersion}/{resourceDescriptor.Kind}",
                    PlatformRuntimeErrorCodes.InvalidResourceKind);
            }

            await Task.CompletedTask;
        }

        private async Task ValidateAsync(SystemObject resource)
        {
            if (string.IsNullOrWhiteSpace(resource.Kind))
            {
                throw new PlatformRuntimeException(
                    "Invalid System Object Kind",
                    PlatformRuntimeErrorCodes.InvalidResourceKind,
                    new ArgumentNullException($"{nameof(SystemObject)}.{nameof(SystemObject.Kind)}"));
            }

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
                throw new PlatformRuntimeException(
                    $"Invalid Resource Kind: {resource.ApiVersion}/{resource.Kind}",
                    PlatformRuntimeErrorCodes.InvalidResourceKind);
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
    }
}

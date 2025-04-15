// <copyright file="ResourceManager.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Service.Core
{
    using Kuiper.Platform.Framework;
    using Kuiper.Platform.ManagementObjects.v1alpha1;
    using Kuiper.Platform.Serialization.Serialization;
    using Kuiper.ServiceInfra.Abstractions.Persistence;

    internal sealed class ResourceManager : IResourceManager
    {
        private readonly IKeyValueStore store;
        private readonly ResourceDescriptor resourceDescriptor = new ResourceDescriptor
        {
            Kind = nameof(ResourceDefinition),
            Group = Constants.Resources.SYSTEM_EXTENSION_GROUP,
            GroupVersion = Constants.Resources.SYSTEM_VERSION,
            Namespace = Constants.Resources.GLOBAL_NAMESPACE,
        };

        private readonly IDictionary<string, ResourceDefinition> resources
            = new Dictionary<string, ResourceDefinition>(StringComparer.OrdinalIgnoreCase);

        private readonly IDictionary<string, ResourceDefinitionVersion> resourceVersions
            = new Dictionary<string, ResourceDefinitionVersion>(StringComparer.OrdinalIgnoreCase);

        private bool isLoaded = false;

        public ResourceManager(IKeyValueStore store)
        {
            this.store = store;
        }

        public async Task<bool> ResourceVersionExists(string group, string kind, string groupVersion)
        {
            await this.LoadAsync();

            return this.resourceVersions.ContainsKey($"{group}/{kind}/{groupVersion}");
        }

        public async Task<ResourceDefinitionVersion?> GetResourceVersionAsync(string group, string kind, string groupVersion)
        {
            await this.LoadAsync();

            if (this.resourceVersions.TryGetValue($"{group}/{kind}/{groupVersion}", out var resourceDefinition))
            {
                return resourceDefinition;
            }

            return null;
        }

        public Task<ResourceDefinition?> GetResourceDefinitionAsync(string group, string kind)
        {
            if (this.resources.TryGetValue($"{group}/{kind}", out var resourceDefinition))
            {
                return Task.FromResult<ResourceDefinition?>(resourceDefinition);
            }
            else
            {
                return Task.FromResult<ResourceDefinition?>(null);
            }
        }

        private ResourceDefinition CreateResourceDefinitionResource()
        {
            ResourceDefinition resourceDefinition = new ResourceDefinition()
            {
                ApiVersion = $"{this.resourceDescriptor.ApiVersion}",
                Kind = $"{this.resourceDescriptor.Kind}",
                Metadata = new SystemObjectMetadata()
                {
                    Name = $"$self",
                    Namespace = $"{this.resourceDescriptor.Namespace}",
                    ResourceVersion = $"{this.resourceDescriptor.GroupVersion}",
                    Uid = Guid.NewGuid().ToString()
                },
                Spec = new ResourceDefinitionSpec()
                {
                    Group = $"{this.resourceDescriptor.Group}",
                    Scope = ResourceScope.System,
                    Names = new ResourceDefinitionNames()
                    {
                        Kind = $"{this.resourceDescriptor.Kind}",
                        Plural = $"{this.resourceDescriptor.Kind.ToLower()}s",
                        Singular = $"{this.resourceDescriptor.Kind.ToLower()}",
                        ShortNames = new string[] { $"resource" },
                    },
                    Versions = new ResourceDefinitionVersion[]
                    {
                        new ResourceDefinitionVersion()
                        {
                            Name = $"{this.resourceDescriptor.GroupVersion}",
                            Served = true,
                            Storage = true,
                            Schema = new ResourceDefinitionVersionSchema()
                            {
                                OpenAPIV3Schema = SystemSchema.GetSchemaAsJsonElement<ResourceDefinition>()
                            }
                        }
                    }
                }
            };

            return resourceDefinition;
        }

        private async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            if (this.isLoaded)
            {
                return;
            }

            this.isLoaded = true;

            // Save the Resource Definition
            var systemResourceDefinition = this.CreateResourceDefinitionResource();

            try
            {
                _ = await this.store.GetAsync(systemResourceDefinition.AsResourceDescriptor().ToResourceId(), cancellationToken);
            }
            catch (KeyNotFoundException)
            {
                // Ignore the exception if the resource definition does not exist and create it
                await this.store.PutAsync(
                    systemResourceDefinition.AsResourceDescriptor().ToResourceId(),
                    systemResourceDefinition.ObjectToJsonBytes(),
                    cancellationToken);
            }

            // NOTE: We can do the same thing by using the standard api runtime

            var objects = await this.store.ScanValuesAsync(this.resourceDescriptor.ToResourcePrefix(), cancellationToken);

            foreach (var objectData in objects)
            {
                try
                {
                    var resourceDefinition = objectData.Value.JsonBytesToObject<ResourceDefinition>();

                    if (resourceDefinition == null)
                    {
                        throw new InvalidOperationException("Resource definition is null.");
                    }

                    this.AddNamedResourceDefinition(resourceDefinition);
                }
                catch (Exception ex)
                {
                    // TODO: Error handling
                }
            }
        }

        private void AddNamedResourceDefinition(ResourceDefinition definition)
        {

            var key = definition.GetKey();

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException("Resource definition key is null or empty.");
            }

            if (this.resources.ContainsKey(key))
            {
                throw new InvalidOperationException($"Resource definition with key {key} already exists.");
            }

            this.resources.Add(key, definition);

            foreach (var version in definition.GetVersions())
            {
                if (this.resourceVersions.ContainsKey(version.Key))
                {
                    throw new InvalidOperationException($"Resource definition version with key {version.Key} already exists.");
                }

                this.resourceVersions.Add(version.Key, version.Value);
            }
        }
    }
}

// <copyright file="ResourceManager.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Service.Core
{
    using System.Threading;

    using Kuiper.Platform.Framework.Abstractions;
    using Kuiper.Platform.ManagementObjects;
    using Kuiper.Platform.ManagementObjects.v1alpha1;
    using Kuiper.Platform.Serialization.Serialization;
    using Kuiper.ServiceInfra.Abstractions.Persistence;

    internal sealed class ResourceManager : IResourceManager
    {
        private readonly IKeyValueStore store;

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

        internal static async Task PersistResourceDefinitionsAsync(IKeyValueStore store, ResourceDefinition resourceDefinition, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            if (resourceDefinition == null)
            {
                throw new ArgumentNullException(nameof(resourceDefinition));
            }

            try
            {
                _ = await store.GetAsync(resourceDefinition.AsResourceDescriptor().ToResourceId(), cancellationToken);
            }
            catch (KeyNotFoundException)
            {
                // Ignore the exception if the resource definition does not exist and create it
                await store.PutAsync(
                    resourceDefinition.AsResourceDescriptor().ToResourceId(),
                    resourceDefinition.ObjectToJsonBytes(),
                    cancellationToken);
            }
        }

        private async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            if (this.isLoaded)
            {
                return;
            }

            this.isLoaded = true;

            var resourceDescriptor = new ResourceDescriptor
            {
                Namespace = SystemConstants.Resources.GLOBAL_NAMESPACE,
                Group = SystemConstants.Resources.SYSTEM_EXTENSION_GROUP,
                Kind = nameof(ResourceDefinition),
            };

            var objects = await this.store.ScanValuesAsync(resourceDescriptor.ToResourcePrefix(), cancellationToken);

            foreach (var objectData in objects)
            {
                try
                {
                    var resourceDefinition = objectData.Value.JsonBytesToObject<ResourceDefinition>();

                    if (resourceDefinition == null)
                    {
                        throw new InvalidOperationException("Resource definition is null.");
                    }

                    this.AddResourceDefinition(resourceDefinition);
                }
                catch (Exception ex)
                {
                    // TODO: Error handling
                }
            }
        }

        private void AddResourceDefinition(ResourceDefinition definition)
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

            foreach (var version in definition.GetVersions<ResourceDefinitionVersion>())
            {
                if (this.resourceVersions.ContainsKey(version.Key))
                {
                    throw new InvalidOperationException($"Resource definition version with key {version.Key} already exists.");
                }

                this.resourceVersions.Add(version.Key, version.Value);
            }
        }

        internal async Task InitializeAsync(ResourceDefinitionManagerBuilder builder)
        {
            await builder.InitializeSystemResourceDefinitions(CancellationToken.None);
            await this.LoadAsync(CancellationToken.None);
        }
    }
}

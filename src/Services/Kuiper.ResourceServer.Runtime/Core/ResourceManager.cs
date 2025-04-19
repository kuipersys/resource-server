// <copyright file="ResourceManager.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kuiper.Platform.ManagementObjects;
    using Kuiper.Platform.ManagementObjects.v1alpha1.Resource;
    using Kuiper.Platform.Runtime.Abstractions.Initialization;
    using Kuiper.Platform.Serialization.Serialization;
    using Kuiper.ServiceInfra.Abstractions.Persistence;

    internal sealed class ResourceManager : IResourceManager, IInitializationTask, IDisposable
    {
        private readonly IKeyValueStore store;

        private readonly Dictionary<string, ResourceDefinition> resources
            = new Dictionary<string, ResourceDefinition>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, ResourceDefinitionVersion> resourceVersions
            = new Dictionary<string, ResourceDefinitionVersion>(StringComparer.OrdinalIgnoreCase);

        private bool isInitialized = false;
        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public ResourceManager(IKeyValueStore store)
        {
            this.store = store;
        }

        public Task<bool> ResourceVersionExists(string group, string kind, string groupVersion)
        {
            return Task.FromResult(this.resourceVersions.ContainsKey($"{group}/{kind}/{groupVersion}"));
        }

        public Task<ResourceDefinitionVersion?> GetResourceVersionAsync(string group, string kind, string groupVersion)
        {
            if (this.resourceVersions.TryGetValue($"{group}/{kind}/{groupVersion}", out var resourceDefinition))
            {
                return Task.FromResult<ResourceDefinitionVersion?>(resourceDefinition);
            }

            return Task.FromResult<ResourceDefinitionVersion?>(null);
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

        private static async Task PersistResourceDefinitionAsync(IKeyValueStore store, ResourceDefinition resourceDefinition, bool overwrite = false, CancellationToken cancellationToken = default)
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

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            await this.InitializeCoreResourceDefinitionsAsync();
            await this.ReloadAsync(cancellationToken);
        }

        private async Task InitializeCoreResourceDefinitionsAsync()
        {
            if (!this.semaphoreSlim.Wait(TimeSpan.FromSeconds(5)))
            {
                throw new TimeoutException("ResourceManager bootstrap semaphore timed out.");
            }

            if (this.isInitialized)
            {
                return;
            }

            try
            {
                var coreResourceDefinitions = CoreResourceModule.GetResourceDefinitions();
                foreach (var resourceDefinition in coreResourceDefinitions)
                {
                    await PersistResourceDefinitionAsync(this.store, resourceDefinition);

                    this.AddResourceDefinition(resourceDefinition);
                }

                this.isInitialized = true;
            }
            finally
            {
                this.semaphoreSlim.Release();
            }
        }

        public async Task ReloadAsync(CancellationToken cancellationToken = default)
        {
            await this.semaphoreSlim.WaitAsync(cancellationToken);

            this.resources.Clear();
            this.resourceVersions.Clear();

            try
            {
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
            finally
            {
                this.semaphoreSlim.Release();
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

            foreach (var version in definition.GetVersions())
            {
                if (this.resourceVersions.ContainsKey(version.Key))
                {
                    throw new InvalidOperationException($"Resource definition version with key {version.Key} already exists.");
                }

                this.resourceVersions.Add(version.Key, version.Value);
            }
        }

        public void Dispose()
        {
            this.semaphoreSlim?.Dispose();
        }
    }
}

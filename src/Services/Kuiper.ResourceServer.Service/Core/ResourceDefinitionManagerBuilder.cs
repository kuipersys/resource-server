// <copyright file="KuiperBuilder.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Service.Core
{
    using Kuiper.Platform.Framework.Abstractions;
    using Kuiper.Platform.ManagementObjects.v1alpha1;
    using Kuiper.ServiceInfra.Abstractions.Persistence;

    public sealed class ResourceDefinitionManagerBuilder : IResourceDefinitionManagerBuilder
    {
        private readonly IKeyValueStore store;
        private readonly IPlatformModule coreModule;
        private readonly List<IPlatformModule> modules = new();
        private readonly IDictionary<string, ResourceDefinition> resourceDefinitions = new Dictionary<string, ResourceDefinition>();
        private readonly IDictionary<string, ResourceDefinitionVersion> resourceDefinitionVersions = new Dictionary<string, ResourceDefinitionVersion>();

        public ResourceDefinitionManagerBuilder(IKeyValueStore store, IPlatformModule coreModule)
        {
            this.store = store ?? throw new ArgumentNullException(nameof(store));
            this.coreModule = coreModule ?? throw new ArgumentNullException(nameof(coreModule));
        }

        public IResourceDefinitionManagerBuilder RegisterModule(IPlatformModule module)
        {
            if (module is null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            if (this.modules.Contains(module))
            {
                throw new ArgumentException($"Module '{module.GetType().Name}' is already registered.", nameof(module));
            }

            this.modules.Add(module);

            return this;
        }

        public IResourceDefinitionManagerBuilder RegisterResourceDefinition<TResourceDefinition>(TResourceDefinition resourceDefinition)
            where TResourceDefinition : class, new()
        {
            var resourceDefinitionObject = resourceDefinition as ResourceDefinition;

            if (resourceDefinition is null)
            {
                throw new ArgumentNullException(nameof(resourceDefinition));
            }

            if (resourceDefinitionObject is null)
            {
                throw new ArgumentException($"Resource definition '{resourceDefinitionObject.GetType().Name}' is not a valid resource definition.", nameof(resourceDefinition));
            }

            if (!this.resourceDefinitions.TryAdd(resourceDefinitionObject.GetKey(), resourceDefinitionObject))
            {
                throw new ArgumentException($"Resource definition with key '{resourceDefinitionObject.GetKey()}' already exists.", nameof(resourceDefinition));
            }

            foreach (var version in resourceDefinitionObject.GetVersions())
            {
                if (!this.resourceDefinitionVersions.TryAdd(version.Key, version.Value))
                {
                    throw new ArgumentException($"Resource definition version '{version.Key}' already exists.", nameof(resourceDefinition));
                }
            }

            return this;
        }

        public async Task InitializeSystemResourceDefinitions(CancellationToken cancellationToken)
        {
            this.resourceDefinitions.Clear();
            this.resourceDefinitionVersions.Clear();

            // Ensure the core module is registered first to prevent potential circular dependencies
            // and prevent resource overlap issues with the core module.
            await this.coreModule.RegisterAsync(this);

            // Register all other modules if any.
            foreach (var module in this.modules)
            {
                await module.RegisterAsync(this);
            }

            // Saves the resource definitions to the store so that the resource manager can use them.
            foreach (var resourceDefinition in this.resourceDefinitions.Values)
            {
                await ResourceManager.PersistResourceDefinitionsAsync(this.store, resourceDefinition, false, cancellationToken);
            }
        }
    }
}

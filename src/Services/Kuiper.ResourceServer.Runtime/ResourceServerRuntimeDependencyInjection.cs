// <copyright file="ResourceServerRuntimeDependencyInjection.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    using System;
    using System.Threading.Tasks;

    using Kuiper.Platform.Runtime;
    using Kuiper.Platform.Runtime.Abstractions.Extensibility;
    using Kuiper.Platform.Runtime.Abstractions.Initialization;
    using Kuiper.Platform.Runtime.Extensibility;
    using Kuiper.ResourceServer.Runtime;
    using Kuiper.ResourceServer.Runtime.Core;
    using Kuiper.ResourceServer.Runtime.Plugins;
    using Kuiper.ServiceInfra.Abstractions.Persistence;

    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class ResourceServerRuntimeDependencyInjection
    {
        public static IServiceCollection AddResourceServerRuntime(this IServiceCollection services)
        {
            services.AddPluginRuntime();
            services.AddPlatformRuntime();
            services.AddResourceManager();

            return services;
        }

        private static IServiceCollection AddResourceManager(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IInitializationTask, ResourceServerInitializationTask>());
            return services.AddSingleton<IResourceManager, ResourceManager>();
        }

        private static IServiceCollection AddPlatformRuntime(this IServiceCollection services)
        {
            return services
                .AddSingleton(services =>
                {
                    var pluginRuntime = services.GetRequiredService<IPluginRuntime>();
                    var platformRuntime = new PlatformRuntime(pluginRuntime, async runtimeServices =>
                    {
                        await Task.CompletedTask;

                        runtimeServices.AddSingleton(services.GetRequiredService<IResourceManager>());
                        runtimeServices.AddSingleton(services.GetRequiredService<IKeyValueStore>());
                    });

                    return platformRuntime;
                });
        }

        private static IServiceCollection AddPluginRuntime(this IServiceCollection services)
        {
            return services.AddSingleton(services =>
            {
                var pluginManager = new PluginManager();
                pluginManager.ConfigureSystemPlugins(services);

                return pluginManager;
            })
            .AddSingleton<IPluginProvider>(services => services.GetRequiredService<PluginManager>())
            .AddSingleton<IPluginRuntime>(services =>
            {
                var pluginProvider = services.GetRequiredService<IPluginProvider>();
                var runtime = new PluginRuntime(pluginProvider);
                return runtime;
            });
        }

        private static void ConfigureSystemPlugins(this PluginManager pluginManager, IServiceProvider services)
        {
            // Add system plugins here

            pluginManager.RegisterPlugin(
                "*",
                OperationStep.PreOperation,
                new ValidationPlugin(
                    services.GetRequiredService<IResourceManager>()),
                order: 1000);

            pluginManager.RegisterPlugin("Put", OperationStep.PreOperation, new MutationPlugin(), order: 100);
            pluginManager.RegisterPlugin("Put", OperationStep.Operation, new PutResourcePlugin());

            // pluginManager.RegisterPlugin("Put", Platform.Framework.Extensibility.OperationStep.PostOperation, __notify__);

            pluginManager.RegisterPlugin("Get", OperationStep.Operation, new GetResourcePlugin());
            pluginManager.RegisterPlugin("List", OperationStep.Operation, new ListResourcePlugin());

            // Even though the delete operation is going to request the resource to be deleted,
            // it will need to be an asynchronous operation to allow for the resource to notify consmers of the deletion
            pluginManager.RegisterPlugin("Delete", OperationStep.Operation, new DeleteResourcePlugin());

            // pluginManager.RegisterPlugin("Delete", Platform.Framework.Extensibility.OperationStep.PostOperation, __finalizer__);
            // pluginManager.RegisterPlugin("Delete", Platform.Framework.Extensibility.OperationStep.PostOperation, __notify__);

            pluginManager.RegisterPlugin("Patch", OperationStep.PreOperation, new MutationPlugin(), order: 100);
            pluginManager.RegisterPlugin("Patch", OperationStep.Operation, new PatchResourcePlugin());

            // pluginManager.RegisterPlugin("Patch", Platform.Framework.Extensibility.OperationStep.PostOperation, __notify__);
            // pluginManager.RegisterPlugin("Post", Sdk.Extensibility.OperationStep.Operation, new ExecutePlugin());
        }
    }
}

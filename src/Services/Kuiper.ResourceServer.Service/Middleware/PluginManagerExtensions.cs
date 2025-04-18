// <copyright file="PluginManagerExtensions.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Service.Middleware
{
    using Kuiper.Platform.Runtime;
    using Kuiper.Platform.Runtime.Abstractions.Extensibility;
    using Kuiper.Platform.Runtime.Extensibility;
    using Kuiper.ResourceServer.Service.Core;
    using Kuiper.ResourceServer.Service.Plugins;
    using Kuiper.ServiceInfra.Abstractions.Persistence;

    internal static class PluginManagerExtensions
    {
        public static IServiceCollection AddPluginRuntime(this IServiceCollection services)
        {
            services
                .AddSingleton(services =>
                {
                    var pluginRuntime = services.GetRequiredService<IPluginRuntime>();
                    var platformRuntime = new PlatformRuntime(pluginRuntime, async descriptors =>
                    {
                        descriptors.ConfigureRuntimeExecutionServices(services);
                    });

                    return platformRuntime;
                })
                .AddSingleton<IPluginProvider>(services => services.GetRequiredService<PluginManager>())
                .AddSingleton<IPluginRuntime>(services =>
                {
                    var pluginProvider = services.GetRequiredService<IPluginProvider>();
                    var runtime = new PluginRuntime(pluginProvider);
                    return runtime;
                });

            services.AddSingleton(services =>
            {
                var pluginManager = new PluginManager();
                pluginManager.ConfigureSystemPlugins(services);

                return pluginManager;
            });

            return services;
        }

        private static void ConfigureRuntimeExecutionServices(this IServiceCollection descriptors, IServiceProvider parentServices)
        {
            // Add runtime execution services here
            // NOTE: The execution runtime shouldn't have and direct database access ... figure another way out!
            descriptors.AddSingleton(parentServices.GetRequiredService<IKeyValueStore>());
            descriptors.AddSingleton(parentServices.GetRequiredService<IResourceManager>());
        }

        private static void ConfigureSystemPlugins(this PluginManager pluginManager, IServiceProvider services)
        {
            // Add system plugins here

            pluginManager.RegisterPlugin(
                "*",
                OperationStep.PreOperation,
                new ValidationPlugin(
                    services.GetRequiredService<IResourceManager>(),
                    services.GetRequiredService<IHttpClientFactory>()),
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
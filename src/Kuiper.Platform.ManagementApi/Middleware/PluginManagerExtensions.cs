// <copyright file="PluginManagerExtensions.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.ManagementApi.Middleware
{
    using Kuiper.Platform.ManagementApi.Core;
    using Kuiper.Platform.ManagementApi.Plugins;
    using Kuiper.Platform.Runtime;
    using Kuiper.Platform.Runtime.Extensibility;
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
                        ConfigureRuntimeExecutionServices(descriptors, services);
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
            pluginManager.RegisterPlugin("Get", Sdk.Extensibility.OperationStep.Operation, new GetResourcePlugin());
            pluginManager.RegisterPlugin("Put", Sdk.Extensibility.OperationStep.Operation, new PutResourcePlugin());
            pluginManager.RegisterPlugin("List", Sdk.Extensibility.OperationStep.Operation, new ListResourcePlugin());
            pluginManager.RegisterPlugin("Delete", Sdk.Extensibility.OperationStep.Operation, new DeleteResourcePlugin());
            pluginManager.RegisterPlugin("Patch", Sdk.Extensibility.OperationStep.Operation, new PatchResourcePlugin());
            // pluginManager.RegisterPlugin("Post", Sdk.Extensibility.OperationStep.Operation, new ExecutePlugin());
        }
    }
}
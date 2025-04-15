// <copyright file="PluginManagerExtensions.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Service.Middleware
{
    using Kuiper.Platform.Runtime;
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
            pluginManager.RegisterPlugin("Get", Platform.Framework.Extensibility.OperationStep.Operation, new GetResourcePlugin());
            pluginManager.RegisterPlugin("Put", Platform.Framework.Extensibility.OperationStep.Operation, new PutResourcePlugin());
            pluginManager.RegisterPlugin("List", Platform.Framework.Extensibility.OperationStep.Operation, new ListResourcePlugin());
            pluginManager.RegisterPlugin("Delete", Platform.Framework.Extensibility.OperationStep.Operation, new DeleteResourcePlugin());
            pluginManager.RegisterPlugin("Patch", Platform.Framework.Extensibility.OperationStep.Operation, new PatchResourcePlugin());
            // pluginManager.RegisterPlugin("Post", Sdk.Extensibility.OperationStep.Operation, new ExecutePlugin());
        }
    }
}
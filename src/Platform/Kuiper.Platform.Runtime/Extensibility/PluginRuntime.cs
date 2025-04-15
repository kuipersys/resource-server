// <copyright file="PluginRuntime.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Extensibility
{
    using Kuiper.Platform.Framework.Extensibility;
    using Kuiper.Platform.Runtime.Execution.Attributes;

    using Microsoft.Extensions.DependencyInjection;

    public class PluginRuntime : IPluginRuntime
    {
        private readonly IPluginProvider pluginProvider;

        public PluginRuntime(IPluginProvider pluginProvider)
        {
            this.pluginProvider = pluginProvider;
        }

        public async Task ExecuteAsync(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var executionContext = serviceProvider.GetRequiredService<IExecutionContext>();

            foreach (var plugin in this.pluginProvider.ResolvePlugins(executionContext.Step, executionContext.Message))
            {
                // Cancellation Requested
                if (executionContext.CancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var pluginType = plugin.GetType();
                PluginAssertionAttribute.ExecuteAssertions(ExecutionPhase.Pre, pluginType, executionContext);
                await plugin.ExecuteAsync(serviceProvider);
                PluginAssertionAttribute.ExecuteAssertions(ExecutionPhase.Post, pluginType, executionContext);
            }
        }
    }
}

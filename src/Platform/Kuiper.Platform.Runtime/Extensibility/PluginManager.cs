// <copyright file="PluginManager.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Extensibility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kuiper.Platform.Runtime.Abstractions.Extensibility;

    /// <summary>
    /// Defines the <see cref="PluginManager" />
    /// </summary>
    public class PluginManager : IPluginManager, IPluginProvider
    {
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly Dictionary<OperationStep, Dictionary<string, List<IPlugin>>> pluginOperations = new Dictionary<OperationStep, Dictionary<string, List<IPlugin>>>();
        private readonly HotSwapPluginProvider hotSwapPluginProvider = new HotSwapPluginProvider();

        public PluginManager()
        {
            this.pluginOperations.Add(OperationStep.PreOperation, new Dictionary<string, List<IPlugin>>());
            this.pluginOperations.Add(OperationStep.Operation, new Dictionary<string, List<IPlugin>>());
            this.pluginOperations.Add(OperationStep.PostOperation, new Dictionary<string, List<IPlugin>>());
        }

        public void RegisterPlugin(string message, OperationStep step, IPlugin plugin, int order = 0)
        {
            if (!this.semaphoreSlim.Wait(TimeSpan.FromMinutes(5)))
            {
                throw new TimeoutException();
            }

            try
            {
                if (plugin == null)
                {
                    throw new ArgumentNullException(nameof(plugin));
                }

                if (message.Count(c => c == '*') > 1)
                {
                    throw new Exception("You cannot specify more than one wild-card in the message name.");
                }

                if (!this.pluginOperations[step].ContainsKey(message))
                {
                    this.pluginOperations[step].Add(message, new List<IPlugin>());
                }

                var stepOperations = this.pluginOperations[step][message];
                stepOperations.Insert(0, plugin);

                // TODO: fix-me - this will likely work long term as plugins will not change
                // frequently during normal runtime conditions but this is not the most elegant
                // solution.
                this.hotSwapPluginProvider.UpdateOperations(this.GetCopyOfPluginOperations());
            }
            finally
            {
                this.semaphoreSlim.Release();
            }
        }

        public void Register<T>(string message, OperationStep step, int order = 0)
            where T : IPlugin, new()
        {
            if (!typeof(IPlugin).IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException($"{typeof(T)} does not inhert from {typeof(IPlugin)}");
            }

            this.RegisterPlugin(message, step, (IPlugin)Activator.CreateInstance(typeof(T)), order);
        }

        public IEnumerable<IPlugin> Resolve(OperationStep operationStep, string message)
            => this.hotSwapPluginProvider.Resolve(operationStep, message);

        private Dictionary<OperationStep, Dictionary<string, List<IPlugin>>> GetCopyOfPluginOperations()
        {
            var copyOfPluginOperations = new Dictionary<OperationStep, Dictionary<string, List<IPlugin>>>();

            foreach (var step in this.pluginOperations.Keys)
            {
                var stepOperations = this.pluginOperations[step];
                var newStepOperations = new Dictionary<string, List<IPlugin>>();

                foreach (var operation in stepOperations)
                {
                    newStepOperations.Add(operation.Key, operation.Value.ToList());
                }

                copyOfPluginOperations.Add(step, newStepOperations);
            }

            return copyOfPluginOperations;
        }
    }
}

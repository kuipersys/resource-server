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

    using Kuiper.Platform.Framework.Extensibility;

    /// <summary>
    /// Defines the <see cref="PluginManager" />
    /// </summary>
    public class PluginManager : IPluginManager, IPluginProvider
    {
        private readonly IDictionary<string, IDictionary<OperationStep, IList<IPlugin>>> operations = new Dictionary<string, IDictionary<OperationStep, IList<IPlugin>>>();

        public PluginManager()
        {
        }

        public void RegisterPlugin(string message, OperationStep step, IPlugin plugin, int order = 0)
        {
            if (plugin == null)
            {
                throw new ArgumentNullException(nameof(plugin));
            }

            if (message.Count(c => c == '*') > 1)
            {
                throw new Exception("You cannot specify more than one wild-card in the message name.");
            }

            if (!this.operations.ContainsKey(message))
            {
                var ops = new Dictionary<OperationStep, IList<IPlugin>>()
                {
                    [OperationStep.PreOperation] = new List<IPlugin>(),
                    [OperationStep.Operation] = new List<IPlugin>(),
                    [OperationStep.PostOperation] = new List<IPlugin>(),
                };

                this.operations.Add(message, ops);
            }

            switch (step)
            {
                case OperationStep.PreOperation:
                case OperationStep.PostOperation:
                case OperationStep.Operation:
                    var stepOperations = this.operations[message][step];
                    stepOperations.Insert(order, plugin);
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported operation type: {step}.");
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

        public IEnumerable<IPlugin> ResolvePlugins(OperationStep step, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.Contains("*"))
            {
                throw new InvalidOperationException("You cannot specify a wild-card for the message name");
            }

            if (this.operations.TryGetValue(message, out var plugins))
            {
                return plugins[step].ToArray();
            }

            return Array.Empty<IPlugin>();
        }

        private IEnumerable<IPlugin> ResolvePlugins2(OperationStep step, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.Contains("*"))
            {
                throw new InvalidOperationException("You cannot specify a wild-card for the message name");
            }

            switch (step)
            {
                case OperationStep.PreOperation:
                case OperationStep.PostOperation:
                case OperationStep.Operation:
                    // TODO: Ordering
                    var stepOperations = this.operations.Where(v =>
                        v.Key.Equals("*") ||
                        v.Key.Contains("*") &&
                        message.StartsWith(v.Key.TrimEnd('*')) ||
                        v.Key.Equals(message)).SelectMany(v => v.Value[step]);

                    if (stepOperations.Count() == 0)
                    {
                        return Array.Empty<IPlugin>();
                    }

                    return stepOperations.ToArray();

                default:
                    throw new InvalidOperationException($"Unsupported operation type: {step}.");
            }
        }
    }
}

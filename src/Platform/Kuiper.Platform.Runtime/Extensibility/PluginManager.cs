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
        private readonly IDictionary<OperationStep, IDictionary<string, IList<IPlugin>>> operations = new Dictionary<OperationStep, IDictionary<string, IList<IPlugin>>>();

        public PluginManager()
        {
            this.operations.Add(OperationStep.PreOperation, new Dictionary<string, IList<IPlugin>>());
            this.operations.Add(OperationStep.Operation, new Dictionary<string, IList<IPlugin>>());
            this.operations.Add(OperationStep.PostOperation, new Dictionary<string, IList<IPlugin>>());
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

            if (!this.operations[step].ContainsKey(message))
            {
                this.operations[step].Add(message, new List<IPlugin>());
            }

            switch (step)
            {
                case OperationStep.PreOperation:
                case OperationStep.PostOperation:
                case OperationStep.Operation:
                    var stepOperations = this.operations[step][message];
                    stepOperations.Insert(0, plugin);
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

            var stepOperations = this.operations[step].Where(v => (v.Key.Equals("*") || (v.Key.Contains("*") && message.StartsWith(v.Key.TrimEnd('*')))) || v.Key.Equals(message));

            if (stepOperations.Count() == 0)
            {
                return Enumerable.Empty<IPlugin>();
            }

            return stepOperations.SelectMany(v => v.Value).ToList();
        }
    }
}

// <copyright file="HotSwapPluginProvider.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Extensibility
{
    using System;
    using System.Collections.Generic;

    using Kuiper.Platform.Runtime.Abstractions.Extensibility;

    internal class HotSwapPluginProvider : IPluginProvider
    {
        private Dictionary<OperationStep, Dictionary<string, List<IPlugin>>> pluginOperations = new Dictionary<OperationStep, Dictionary<string, List<IPlugin>>>();
        private object lockObject = new object();

        public void UpdateOperations(Dictionary<OperationStep, Dictionary<string, List<IPlugin>>> newOperations)
        {
            // For now until we have a better way to do this, we will just use a lock
            Interlocked.Exchange(ref this.pluginOperations, newOperations);
        }

        public IEnumerable<IPlugin> Resolve(OperationStep step, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.Contains('*', StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("You cannot specify a wild-card for the message name");
            }

            var stepOperations = this.pluginOperations[step].Where(v => (
                v.Key.Equals("*", StringComparison.OrdinalIgnoreCase) ||
                (v.Key.Contains('*', StringComparison.OrdinalIgnoreCase) && message.StartsWith(v.Key.TrimEnd('*'), StringComparison.OrdinalIgnoreCase))) ||
                v.Key.Equals(message, StringComparison.OrdinalIgnoreCase));

            if (!stepOperations.Any())
            {
                return [];
            }

            return stepOperations.SelectMany(v => v.Value).ToList();
        }
    }
}

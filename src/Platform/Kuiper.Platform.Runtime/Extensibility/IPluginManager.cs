// <copyright file="IPluginManager.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

using Kuiper.Platform.Runtime.Abstractions.Extensibility;

namespace Kuiper.Platform.Runtime.Extensibility
{
    /// <summary>
    /// Defines the <see cref="IPluginManager" />
    /// </summary>
    public interface IPluginManager
    {
        void Register<T>(string message, OperationStep step, int order = 0)
            where T : IPlugin, new();

        void RegisterPlugin(string message, OperationStep step, IPlugin plugin, int order = 0);
    }
}

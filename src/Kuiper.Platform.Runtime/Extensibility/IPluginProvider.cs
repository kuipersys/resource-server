// <copyright file="IPluginProvider.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Extensibility
{
    using Kuiper.Platform.Sdk.Extensibility;

    public interface IPluginProvider
    {
        IEnumerable<IPlugin> ResolvePlugins(OperationStep step, string message);
    }
}

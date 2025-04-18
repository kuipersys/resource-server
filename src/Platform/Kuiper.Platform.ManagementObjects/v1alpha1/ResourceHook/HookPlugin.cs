// <copyright file="HookPlugin.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.ManagementObjects.v1alpha1.ResourceHook
{
    using System.Collections.Generic;

    public class HookPlugin
    {
        /// <summary>The fully qualified type name of the plugin</summary>
        public string Type { get; init; } = default!;

        /// <summary>Optional assembly name if dynamically loaded</summary>
        public string? Assembly { get; init; }

        /// <summary>Optional constructor parameters or config</summary>
        public Dictionary<string, object>? Parameters { get; init; }
    }

}

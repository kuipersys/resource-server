// <copyright file="HookExecutionMode.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.ManagementObjects.v1alpha1.ResourceHook
{
    public enum HookExecutionMode
    {
        /// <summary>Execute using a registered or DI-resolved in-process plugin.</summary>
        Plugin,

        /// <summary>Execute by sending an HTTP request to a remote webhook endpoint.</summary>
        Webhook
    }
}

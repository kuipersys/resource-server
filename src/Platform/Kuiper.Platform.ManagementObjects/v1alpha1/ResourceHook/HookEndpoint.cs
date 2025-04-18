// <copyright file="HookEndpoint.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.ManagementObjects.v1alpha1.ResourceHook
{
    using System;
    using System.Collections.Generic;

    public class HookEndpoint
    {
        public Uri Url { get; init; } = default!;

        /// <summary>Optional timeout for the HTTP request (in seconds)</summary>
        public int TimeoutSeconds { get; init; } = 5;

        /// <summary>Optional HTTP method (default: POST)</summary>
        public string Method { get; init; } = "POST";

        /// <summary>Optional headers to include with the request</summary>
        public Dictionary<string, string>? Headers { get; init; }

        /// <summary>Indicates whether to retry on failure (for async notifications)</summary>
        public bool RetryOnFailure { get; init; } = false;
    }
}
